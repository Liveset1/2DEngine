using FlexileEngine.engine.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FlexileEngine.engine
{
    public class LevelEditorScene : Scene
    {
        private Window Window = Window.get();

        public LevelEditorScene()
        {
           
        }

        public override void Init()
        {
            Camera = new Camera(Vector3.UnitZ * 3, Window.Size.X / (float) Window.Size.Y);

            int xOffset = 10;
            int yOffset = 10;

            float totalWidth = (float)(600 - xOffset * 2);
            float totalHeight = (float)(300 - yOffset * 2);
            float sizeX = totalWidth / 100.0f;
            float sizeY = totalHeight / 100.0f;
            float padding = 3;

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    float xPos = xOffset + (x * sizeX) + (padding * x);
                    float yPos = yOffset + (y * sizeY) + (padding * y);

                    GameObject go = new GameObject("Obj" + x + "" + y, new Transform(new Vector3(xPos, yPos, 0), new Vector3(sizeX, sizeY, 0)));
                    go.AddComponent(new Sprite2D(new Vector4(xPos / totalWidth, yPos / totalHeight, 1, 1)));
                    AddGameObject(go);
                }
            }
        }


        private bool _firstMove = true;
        private Vector2 _lastPos;
        private double _time;
        private bool _clicked = false;
        public override void Update(FrameEventArgs e, float dt)
        {
            // NOTE: Not sure why you want to skip updating and rendering if the window is focused
            // it prevented me from screenshotting the app :)
            //if (!Window.IsFocused) // Check to see if the window is focused
            //{
            //    return;
            //}

            var input = Window.KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Window.Close();
            }

            const float cameraSpeed = 100.5f;
            const float sensitivity = 0.2f;

            Vector3 right = new Vector3(1, 0, 0);
            Vector3 up = new Vector3(0, 1, 0);
            if (input.IsKeyDown(Keys.W))
            {
                Camera.Position += up * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                Camera.Position -= up * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                Camera.Position -= right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                Camera.Position += right * cameraSpeed * (float)e.Time; // Right
            }
            // if (input.IsKeyDown(Keys.E))
            // {
            //     Camera.Position += Camera.Up * cameraSpeed * (float)e.Time; // Up
            // }
            // if (input.IsKeyDown(Keys.Q))
            // {
            //     Camera.Position -= Camera.Up * cameraSpeed * (float)e.Time; // Down
            // }

            // Get the mouse state
            var mouse = Window.MouseState;

            if (mouse.IsButtonDown(MouseButton.Button2) == true)
            {
                Console.WriteLine("Pressing right mouse");
                if (_firstMove) // This bool variable is initially set to true.
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    int maxX = 1905;
                    if (mouse.X >= maxX)
                    {
                        Window.MousePosition = new Vector2(-maxX + 1, _lastPos.Y);
                    }
                    else if (mouse.X < maxX)
                    {
                        Console.WriteLine(mouse.X);
                        if (mouse.X < 3)
                        {
                            Window.MousePosition = new Vector2(maxX, _lastPos.Y);
                        }
                        else
                        {
                            if (!_clicked)
                            {
                                _clicked = true;
                            }
                            else
                            {
                                // Calculate the offset of the mouse position
                                var deltaX = mouse.X - _lastPos.X;
                                var deltaY = mouse.Y - _lastPos.Y;
                                _lastPos = new Vector2(mouse.X, mouse.Y);

                                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                                Camera.Yaw += deltaX * sensitivity;
                                Camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
                            }
                        }
                    }


                }
            }
            else
            {
                _clicked = false;
            }

            foreach (GameObject go in gameObjects)
            {
                go.Update(dt);
            }

            Renderer.Render();
        }
    } 
}


/*
 if (!Window.IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = Window.KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Window.Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                Camera.Position += Camera.Front * cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                Camera.Position -= Camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                Camera.Position -= Camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                Camera.Position += Camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.E))
            {
                Camera.Position += Camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.Q))
            {
                Camera.Position -= Camera.Up * cameraSpeed * (float)e.Time; // Down
            }

            // Get the mouse state
            var mouse = Window.MouseState;
            
            if (mouse.IsButtonDown(MouseButton.Button2) == true) 
            {
                Console.WriteLine("Pressing right mouse");
                if (_firstMove) // This bool variable is initially set to true.
                {
                    _lastPos = new Vector2(mouse.X, mouse.Y);
                    _firstMove = false;
                }
                else
                {
                    int maxX = 1905;
                    if (mouse.X >= maxX)
                    {
                        Window.MousePosition = new Vector2(-maxX + 1, _lastPos.Y);
                    }
                    else if (mouse.X < maxX)
                    {
                        Console.WriteLine(mouse.X);
                        if (mouse.X < 3)
                        {
                            Window.MousePosition = new Vector2(maxX, _lastPos.Y);
                        }
                        else
                        {
                            if (!_clicked)
                            {
                                _clicked = true;
                            } 
                            else
                            {
                                // Calculate the offset of the mouse position
                                var deltaX = mouse.X - _lastPos.X;
                                var deltaY = mouse.Y - _lastPos.Y;
                                _lastPos = new Vector2(mouse.X, mouse.Y);

                                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                                Camera.Yaw += deltaX * sensitivity;
                                Camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
                            }
                        }
                    }

                    
                }
            }
            else
            {
                _clicked = false;
            }
 
 
 
 */