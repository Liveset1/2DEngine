using FlexileEngine.engine.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FlexileEngine.engine.Renderer
{
    public class RenderBatch
    {
        private const string VertexPath = "../../../assets/Shaders/DefaultVS.glsl";
        private const string FragmentPath = "../../../assets/Shaders/DefaultFS.glsl";

        // NOTE: Important that this is 3. Your vertex shader has:
        //   layout(location = 0) in vec*3*
        // So this needs to match
        // Additionally, C# provides sizeof() operator, so no need to do this hack which
        // I had to do in Java. See below for better ways to do this stuff
        // private const int POS_SIZE = 3;
        // private const int COLOR_SIZE = 4;
        // private const int POS_OFFSET = 0;
        // private const int COLOR_OFFSET = POS_OFFSET + POS_SIZE * sizeof(float);
        // private const int VERTEX_SIZE = 6;
        // private const int VERTEX_SIZE_BYTES = VERTEX_SIZE * sizeof(float);
        private const int numVertsPerQuad = 4;
        private const int numElementsPerQuad = 6;

        // Use structs in C#. It simplifies a lot of stuff
        // You do have to be a bit careful with sizes and stuff though
        // So we tell the compiler to lay out the members sequentially in memory and
        // to tightly pack it. That way, the binary will look something like
        // {
        //   float[3]
        //   float[4]
        // }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct QuadVertex
        {
            public Vector3 Position;
            public Vector4 Color;
        }

        private Sprite2D[] sprites;
        private int numSprites;
        public bool hasRoom { get; set; }
        // Swapping this to struct to get type safety not available in Java
        private QuadVertex[] vertices;

        private int vaoID, vboID;
        private int maxBatchSize;
        private Shader Shader;

        public RenderBatch(int maxBatchSize)
        {
            Shader = new Shader(VertexPath, FragmentPath);
            Shader.Use();
            sprites = new Sprite2D[maxBatchSize];
            this.maxBatchSize = maxBatchSize;

            vertices = new QuadVertex[maxBatchSize * numVertsPerQuad];

            numSprites = 0;
            hasRoom = true;
        }

        public void Start()
        {
            // Generate and Bind Vertex Array Object
            vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);

            // Allocate space for vertices
            vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            // NOTE: Use sizeof here to be type safe
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<QuadVertex>(), vertices, BufferUsageHint.DynamicDraw);

            // Create and upload Indices buffer
            int eboID = GL.GenBuffer();
            UInt32[] indices = GenIndices();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            // NOTE: These are uint32 not floats
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(UInt32), indices, BufferUsageHint.StaticDraw);

            var vertexLocation = Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(
                vertexLocation, 
                // NOTE: Vertex position has 3 components x, y, z
                3, 
                VertexAttribPointerType.Float, 
                false,
                // NOTE: Using sizeof here means you don't have to manually calculate it
                Marshal.SizeOf<QuadVertex>(), 
                // NOTE: This gets the variable name of QuadVertex.Position to avoid typos
                // So it will return "Position" as a string
                // Using offsetof here will also prevent typos in the number of bytes to offset this data member
                Marshal.OffsetOf<QuadVertex>(nameof(QuadVertex.Position))
            );
            GL.EnableVertexAttribArray(vertexLocation);

            var colorLocation = Shader.GetAttribLocation("aColor");
            GL.VertexAttribPointer(
                colorLocation,
                // NOTE: 4 Components: r, g, b, a
                4,
                VertexAttribPointerType.Float,
                false,
                // NOTE: Same as above
                Marshal.SizeOf<QuadVertex>(),
                // NOTE: Same as above
                Marshal.OffsetOf<QuadVertex>(nameof(QuadVertex.Color))
            );
            GL.EnableVertexAttribArray(colorLocation);
        }

        public void AddSprite(Sprite2D spr)
        {
            // Get index and add renderObject
            int index = numSprites;
            sprites[index] = spr;
            numSprites++;

            // Add properties to local vertices array
            LoadVertexProperties(index);


            if (numSprites >= maxBatchSize)
            {
                hasRoom = false;
            }
        }

        public void Render()
        {
            Window window = Window.get();

            // For now, we will rebuffer all data every frame
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
            // NOTE: No typos here when using sizeof
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, Marshal.SizeOf<QuadVertex>() * numSprites * numVertsPerQuad, vertices);

            // Use shader
            Camera Camera = window.activeScene.Camera;
            Shader.Use();
            Shader.SetMatrix4("uProjection", Camera.GetProjectionMatrix());
            Shader.SetMatrix4("uView", Camera.GetViewMatrix());

            // NOTE: This is bad for performance and unecessary which I didn't know when I made this tutorial
            //
            // int vertexLocation = Shader.GetAttribLocation("aPos");
            // int colorLocation = Shader.GetAttribLocation("aColor");
            // GL.EnableVertexAttribArray(vertexLocation);
            // GL.EnableVertexAttribArray(colorLocation);

            GL.BindVertexArray(vaoID);
            GL.DrawElements(PrimitiveType.Triangles, numSprites * numElementsPerQuad, DrawElementsType.UnsignedInt, 0);

            // NOTE: This is bad for performance which I didn't know at the time of making this tutorial
            // 
            // GL.DisableVertexAttribArray(vertexLocation);
            // GL.DisableVertexAttribArray(colorLocation);
            // GL.BindVertexArray(0);
            // Shader.Detach();
        }

        // NOTE: Better to be explicit here since in C# you can define
        // the exact type of integer
        private UInt32[] GenIndices()
        {
            // 6 indices per quad (3 per triangle)
            var elements = new UInt32[maxBatchSize * numElementsPerQuad];
            for (UInt32 i = 0; i < maxBatchSize; i++)
            {
                LoadElementIndices(elements, i);
            }

            return elements;
        }

        private void LoadElementIndices(UInt32[] elements, UInt32 index)
        {
            UInt32 offsetArrayIndex = numElementsPerQuad * index;
            UInt32 offset = numVertsPerQuad * index;

            // 3, 2, 0, 0, 2, 1        7, 6, 4, 4, 6, 5
            // Triangle 1
            elements[offsetArrayIndex] = offset + 3;
            elements[offsetArrayIndex + 1] = offset + 2;
            elements[offsetArrayIndex + 2] = offset + 0;

            // Triangle 2
            elements[offsetArrayIndex + 3] = offset + 0;
            elements[offsetArrayIndex + 4] = offset + 2;
            elements[offsetArrayIndex + 5] = offset + 1;
        }

        private void LoadVertexProperties(int index)
        {
            Sprite2D sprite = this.sprites[index];

            // Find offset within array (4 vertices per sprite)
            int offset = index * numVertsPerQuad;

            Vector4 color = sprite.Color;

            // Add vertices with the appropriate properties
            float xAdd = 1.0f;
            float yAdd = 1.0f;
            for (int i = 0; i < 4; i++)
            {
                if (i == 1)
                {
                    yAdd = 0.0f;
                }
                else if (i == 2)
                {
                    xAdd = 0.0f;
                }
                else if (i == 3)
                {
                    yAdd = 1.0f;
                }

                // Load position
                // NOTE: Since we're using structs, you can use type safety
                vertices[offset + i].Position.X = sprite.GameObject.Transform.Position.X + (xAdd * sprite.GameObject.Transform.Scale.X);
                vertices[offset + i].Position.Y = sprite.GameObject.Transform.Position.Y + (yAdd * sprite.GameObject.Transform.Scale.Y);

                // Load color
                vertices[offset + i].Color = color;
            }
        }
    }
}
