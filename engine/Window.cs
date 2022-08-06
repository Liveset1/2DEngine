using System;
using System.Diagnostics;
using FlexileEngine.Runtime.utils;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace FlexileEngine.engine
{
    public sealed class Window : GameWindow
    {
        public int Width { get; set; }
        public int Height { get; set; }

        static private Window window;
        private GameWindowSettings gameWindowSettings;
        private NativeWindowSettings nativeWindowSettings;
        public float frameStarted = Time.getTime();
        public float frameEnded;
        public float deltaTime = -1.0f;

        public int activeSceneIndex = -1;
        public Scene activeScene { get; set; } = null!;

        private Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            this.gameWindowSettings = gameWindowSettings;
            this.nativeWindowSettings = nativeWindowSettings;
            this.Width = nativeWindowSettings.Size.X;
            this.Height = nativeWindowSettings.Size.Y;
        }

        public static Window get(GameWindowSettings gameWindowSettings1, NativeWindowSettings nativeWindowSettings1)
        {
            if (window == null)
            {
                window = new Window(gameWindowSettings1, nativeWindowSettings1);
            }
            return window;
        }

        public static Window get()
        {
            if (window == null)
            {
                window = new Window(new GameWindowSettings(), new NativeWindowSettings());
            }

            return window;
        }

        public static void activateScene(int scene)
        {
            Window window = get();
            switch(scene)
            {
                case 0:
                    window.activeScene = new LevelEditorScene();
                    window.activeScene.Init();
                    window.activeScene.Start();
                    break;
                case 1:
                    window.activeScene = new LevelScene();
                    window.activeScene.Init();
                    window.activeScene.Start();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Unknown Scene");
            }
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.3f, 0.3f, 0.3f, 1f);
            Window.activateScene(0);
            base.OnLoad();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            if (deltaTime >= 0)
            {
                if (activeScene != null)
                {
                    activeScene.Update(args, deltaTime);
                }
            }

           // Console.WriteLine("FPS: " + 1.0f / deltaTime);

            // Delta time
            frameEnded = Time.getTime();
            deltaTime = frameEnded - frameStarted;
            frameStarted = frameEnded;
            
            SwapBuffers();
        }

        protected override void OnRenderThreadStarted()
        {
            base.OnRenderThreadStarted();
        }
    }
}
