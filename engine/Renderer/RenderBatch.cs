using FlexileEngine.engine.Components;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexileEngine.engine.Renderer
{
    public class RenderBatch
    {
        private const string VertexPath = "../../../assets/Shaders/DefaultVS.glsl";
        private const string FragmentPath = "../../../assets/Shaders/DefaultFS.glsl";

        private const int POS_SIZE = 2;
        private const int COLOR_SIZE = 4;
        private const int POS_OFFSET = 0;
        private const int COLOR_OFFSET = POS_OFFSET + POS_SIZE * sizeof(float);
        private const int VERTEX_SIZE = 6;
        private const int VERTEX_SIZE_BYTES = VERTEX_SIZE * sizeof(float);

        private Sprite2D[] sprites;
        private int numSprites;
        public bool hasRoom { get; set; }
        private float[] vertices;

        private int vaoID, vboID;
        private int maxBatchSize;
        private Shader Shader;

        public RenderBatch(int maxBatchSize)
        {
            Shader = new Shader(VertexPath, FragmentPath);
            Shader.Use();
            sprites = new Sprite2D[maxBatchSize];
            this.maxBatchSize = maxBatchSize;

            // 4 vertices quads
            vertices = new float[maxBatchSize * 4 *  VERTEX_SIZE];

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
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            // Create and upload Indices buffer
            int eboID = GL.GenBuffer();
            int[] indices = GenIndices();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(float), indices, BufferUsageHint.StaticDraw);

            var vertexLocation = Shader.GetAttribLocation("aPos");
            GL.VertexAttribPointer(vertexLocation, POS_SIZE, VertexAttribPointerType.Float, false, VERTEX_SIZE_BYTES, POS_OFFSET);
            GL.EnableVertexAttribArray(vertexLocation);

            var colorLocation = Shader.GetAttribLocation("aColor");
            GL.VertexAttribPointer(colorLocation, COLOR_SIZE, VertexAttribPointerType.Float, false, VERTEX_SIZE_BYTES, COLOR_OFFSET);
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
            GL.BufferSubData(BufferTarget.ArrayBuffer, (IntPtr)0, VERTEX_SIZE_BYTES, vertices);

            // Use shader
            Camera Camera = window.activeScene.Camera;
            Shader.Use();
            Shader.SetMatrix4("uProjection", Camera.GetProjectionMatrix());
            Shader.SetMatrix4("uView", Camera.GetViewMatrix());

            GL.BindVertexArray(vaoID);
            int vertexLocation = Shader.GetAttribLocation("aPos");
            int colorLocation = Shader.GetAttribLocation("aColor");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.EnableVertexAttribArray(colorLocation);

            GL.DrawElements(PrimitiveType.Triangles, numSprites * 6, DrawElementsType.UnsignedInt, 0);

            GL.DisableVertexAttribArray(vertexLocation);
            GL.DisableVertexAttribArray(colorLocation);
            GL.BindVertexArray(0);

            Shader.Detach();
        }


        private int[] GenIndices()
        {
            // 6 indices per quad (3 per triangle)
            int[] elements = new int[6 * maxBatchSize];
            for(int i = 0; i < maxBatchSize; i++) {
                LoadElementIndices(elements, i);
            }

            return elements;
        }

        private void LoadElementIndices(int[] elements, int index)
        {
            int offsetArrayIndex = 6 * index;
            int offset = 4 * index;

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
            int offset = index * 4 * VERTEX_SIZE;

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
                vertices[offset] = sprite.GameObject.Transform.Position.X + (xAdd * sprite.GameObject.Transform.Scale.X);
                vertices[offset + 1] = sprite.GameObject.Transform.Position.Y+ (yAdd * sprite.GameObject.Transform.Scale.Y);
                
                // Load color
                vertices[offset + 2] = color.X;
                vertices[offset + 3] = color.Y;
                vertices[offset + 4] = color.Z;
                vertices[offset + 5] = color.W;

                offset += VERTEX_SIZE;
            }
        }
    }
}
