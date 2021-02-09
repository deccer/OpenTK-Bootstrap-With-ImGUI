using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;

namespace Client.Base
{
    public class Debug
    {
        [Conditional("DEBUG")]
        public static void CheckGLError(string category)
        {
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
            {
                System.Diagnostics.Debug.Print($"{category}: {error}");
            }
        }
    }
}
