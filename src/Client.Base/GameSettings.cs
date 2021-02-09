using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Client.Base
{
    public class GameSettings
    {
        internal readonly NativeWindowSettings _nativeWindowSettings;

        public GameSettings(int width, int height, bool isFullscreen)
        {
            Width = width;
            Height = height;
            IsFullscreen = isFullscreen;

            _nativeWindowSettings = NativeWindowSettings.Default;
            _nativeWindowSettings.Profile = ContextProfile.Core;
            _nativeWindowSettings.API = ContextAPI.OpenGL;

            _nativeWindowSettings.Size = new Vector2i(width, height);
            _nativeWindowSettings.IsFullscreen = isFullscreen;
            _nativeWindowSettings.AutoLoadBindings = true;
        }

        public int Width { get; }

        public int Height { get; }

        public bool IsFullscreen { get; }
    }
}
