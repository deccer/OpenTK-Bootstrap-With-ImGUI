using System;
using Client.Base;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Serilog;

namespace Client
{
    internal sealed class ClientGame : Game
    {
        private readonly Color4 _clearColor;

        public ClientGame(
            ILogger logger,
            IServiceProvider serviceProvider,
            GameSettings gameSettings)
            : base(logger, serviceProvider, gameSettings)
        {
            Window.Title = "Client Game";

            _clearColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
        }

        protected override void Load()
        {
            base.Load();
        }

        protected override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Window.Size.X, Window.Size.Y);
            GL.ClearColor(_clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            base.Render(e);
        }

        protected override void Unload()
        {
            base.Unload();
        }

        protected override void Update(FrameEventArgs e)
        {
            base.Update(e);
        }
    }
}
