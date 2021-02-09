using System;
using Client.Base;
using Client.Base.UI;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Serilog;

namespace Client
{
    internal sealed class ClientGame : Game
    {
        private readonly Color4 _clearColor;
        private ImGuiController _imGuiController;

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
            _imGuiController = new ImGuiController(Window.Size.X, Window.Size.Y);
        }

        protected override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, Window.Size.X, Window.Size.Y);
            GL.ClearColor(_clearColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (ImGui.Begin("TestWindow"))
            {
                if (ImGui.Button("Close"))
                {
                    Window.Close();
                }

                ImGui.End();
            }
            _imGuiController.Render();
            base.Render(e);
        }

        protected override void Unload()
        {
            _imGuiController.Dispose();
            base.Unload();
        }

        protected override void Update(FrameEventArgs e)
        {
            _imGuiController.Update(Window, (float)e.Time);
            base.Update(e);
        }
    }
}
