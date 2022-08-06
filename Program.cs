using FlexileEngine.engine;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

int Width = 1000;
int Height = 800;

GameWindowSettings gameWindowSettings = new GameWindowSettings();
NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();

// GameWindowSettings
gameWindowSettings = new GameWindowSettings();
gameWindowSettings.UpdateFrequency = 60.0;
gameWindowSettings.RenderFrequency = 60.0;

// NativeWindowSettings
nativeWindowSettings.APIVersion = Version.Parse("3.3");
nativeWindowSettings.WindowState = WindowState.Maximized;
nativeWindowSettings.StartFocused = true;
nativeWindowSettings.StartVisible = true;
nativeWindowSettings.MinimumSize = new Vector2i(Width, Height);
nativeWindowSettings.Size = new Vector2i(Width, Height);
nativeWindowSettings.Title = "Flexile Engine";

Window window = Window.get(gameWindowSettings, nativeWindowSettings);

window.Run();