using System.Collections.Generic;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Client.Base
{
    public class Shader
    {
        public readonly string Name;
        public int Program { get; private set; }

        private readonly Dictionary<string, int> _uniformToLocationMap = new Dictionary<string, int>();

        private bool _initialized = false;

        private readonly (ShaderType Type, string Path)[] _files;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateProgram(string name, out int program)
        {
            program = GL.CreateProgram();
            GL.ObjectLabel(ObjectLabelIdentifier.Program, program, name.Length, $"Program: {name}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateShader(ShaderType type, string name, out int shader)
        {
            shader = GL.CreateShader(type);
            GL.ObjectLabel(ObjectLabelIdentifier.Shader, shader, name.Length, $"Shader: {type}: {name}");
        }

        public Shader(string name, string vertexShader, string fragmentShader)
        {
            Name = name;
            _files = new[]
            {
                (ShaderType.VertexShader, vertexShader),
                (ShaderType.FragmentShader, fragmentShader),
            };
            Program = CreateProgram(name, _files);
        }

        public void UseShader()
        {
            GL.UseProgram(Program);
        }

        public void Dispose()
        {
            if (_initialized)
            {
                GL.DeleteProgram(Program);
                _initialized = false;
            }
        }

        private UniformFieldInfo[] GetUniforms()
        {
            GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out var uniformCount);

            var uniforms = new UniformFieldInfo[uniformCount];

            for (var i = 0; i < uniformCount; i++)
            {
                var uniformName = GL.GetActiveUniform(Program, i, out var Size, out var Type);

                UniformFieldInfo FieldInfo;
                FieldInfo.Location = GetUniformLocation(uniformName);
                FieldInfo.Name = uniformName;
                FieldInfo.Size = Size;
                FieldInfo.Type = Type;

                uniforms[i] = FieldInfo;
            }

            return uniforms;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUniformLocation(string uniform)
        {
            if (_uniformToLocationMap.TryGetValue(uniform, out var location))
            {
                return location;
            }

            location = GL.GetUniformLocation(Program, uniform);
            _uniformToLocationMap.Add(uniform, location);

            if (location == -1)
            {
                System.Diagnostics.Debug.Print($"The uniform '{uniform}' does not exist in the shader '{Name}'!");
            }

            return location;
        }

        private int CreateProgram(string name, params (ShaderType Type, string Source)[] shaderPaths)
        {
            CreateProgram(name, out var program);

            var shaders = new int[shaderPaths.Length];
            for (var i = 0; i < shaderPaths.Length; i++)
            {
                shaders[i] = CompileShader(name, shaderPaths[i].Type, shaderPaths[i].Source);
            }

            foreach (var shader in shaders)
            {
                GL.AttachShader(program, shader);
            }

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var Success);
            if (Success == 0)
            {
                var info = GL.GetProgramInfoLog(program);
                System.Diagnostics.Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");
            }

            foreach (var shader in shaders)
            {
                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);
            }

            _initialized = true;

            return program;
        }

        private int CompileShader(string name, ShaderType type, string source)
        {
            CreateShader(type, name, out var Shader);
            GL.ShaderSource(Shader, source);
            GL.CompileShader(Shader);

            GL.GetShader(Shader, ShaderParameter.CompileStatus, out var success);
            if (success == 0)
            {
                var Info = GL.GetShaderInfoLog(Shader);
                System.Diagnostics.Debug.WriteLine($"GL.CompileShader for shader '{Name}' [{type}] had info log:\n{Info}");
            }

            return Shader;
        }

        private struct UniformFieldInfo
        {
            public int Location;
            public string Name;
            public int Size;
            public ActiveUniformType Type;
        }
    }
}
