using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace Client.Base
{
    public enum TextureCoordinate
    {
        S = TextureParameterName.TextureWrapS,
        T = TextureParameterName.TextureWrapT,
        R = TextureParameterName.TextureWrapR
    }

    public class Texture : IDisposable
    {
        public const SizedInternalFormat Srgb8Alpha8 = (SizedInternalFormat)All.Srgb8Alpha8;
        public const SizedInternalFormat RGB32F = (SizedInternalFormat)All.Rgb32f;

        public const GetPName MAX_TEXTURE_MAX_ANISOTROPY = (GetPName)0x84FF;

        public static readonly float MaxAnisotrophy;

        static Texture()
        {
            MaxAnisotrophy = GL.GetFloat(MAX_TEXTURE_MAX_ANISOTROPY);
        }

        public readonly string Name;
        public readonly int GLTexture;
        public readonly int Width, Height;
        public readonly int MipmapLevels;
        public readonly SizedInternalFormat InternalFormat;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CreateTexture(TextureTarget target, string name, out int texture)
        {
            GL.CreateTextures(target, 1, out texture);
            GL.ObjectLabel(ObjectLabelIdentifier.Texture, texture, name.Length, $"T: {name}");
        }

        public Texture(string name, Bitmap image, bool generateMipmaps, bool srgb)
        {
            Name = name;
            Width = image.Width;
            Height = image.Height;
            InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;

            MipmapLevels = generateMipmaps
                ? (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2))
                : 1;

            Debug.CheckGLError("Clear");

            CreateTexture(TextureTarget.Texture2D, Name, out GLTexture);
            GL.TextureStorage2D(GLTexture, MipmapLevels, InternalFormat, Width, Height);
            Debug.CheckGLError("Storage2d");

            var data = image.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TextureSubImage2D(GLTexture, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            Debug.CheckGLError("SubImage");

            image.UnlockBits(data);

            if (generateMipmaps)
            {
                GL.GenerateTextureMipmap(GLTexture);
            }

            GL.TextureParameter(GLTexture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            Debug.CheckGLError("WrapS");
            GL.TextureParameter(GLTexture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            Debug.CheckGLError("WrapT");

            GL.TextureParameter(GLTexture, TextureParameterName.TextureMinFilter, (int)(generateMipmaps ? TextureMinFilter.Linear : TextureMinFilter.LinearMipmapLinear));
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            Debug.CheckGLError("Filtering");

            GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
            Debug.CheckGLError("MipLevels");
        }

        public Texture(string name, int nativeTexture, int width, int height, int mipmapLevels, SizedInternalFormat internalFormat)
        {
            Name = name;
            GLTexture = nativeTexture;
            Width = width;
            Height = height;
            MipmapLevels = mipmapLevels;
            InternalFormat = internalFormat;
        }

        public Texture(string name, int width, int height, IntPtr data, bool generateMipmaps = false, bool srgb = false)
        {
            Name = name;
            Width = width;
            Height = height;
            InternalFormat = srgb ? Srgb8Alpha8 : SizedInternalFormat.Rgba8;
            MipmapLevels = generateMipmaps == false ? 1 : (int)Math.Floor(Math.Log(Math.Max(Width, Height), 2));

            CreateTexture(TextureTarget.Texture2D, Name, out GLTexture);
            GL.TextureStorage2D(GLTexture, MipmapLevels, InternalFormat, Width, Height);

            GL.TextureSubImage2D(GLTexture, 0, 0, 0, Width, Height, PixelFormat.Bgra, PixelType.UnsignedByte, data);

            if (generateMipmaps) GL.GenerateTextureMipmap(GLTexture);

            SetWrap(TextureCoordinate.S, TextureWrapMode.Repeat);
            SetWrap(TextureCoordinate.T, TextureWrapMode.Repeat);

            GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLevel, MipmapLevels - 1);
        }

        public void SetMinFilter(TextureMinFilter filter)
        {
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMinFilter, (int)filter);
        }

        public void SetMagFilter(TextureMagFilter filter)
        {
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMagFilter, (int)filter);
        }

        public void SetAnisotropy(float level)
        {
            const TextureParameterName TEXTURE_MAX_ANISOTROPY = (TextureParameterName)0x84FE;
            GL.TextureParameter(GLTexture, TEXTURE_MAX_ANISOTROPY, Math.Clamp(level, 1, MaxAnisotrophy));
        }

        public void SetLod(int @base, int min, int max)
        {
            GL.TextureParameter(GLTexture, TextureParameterName.TextureLodBias, @base);
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMinLod, min);
            GL.TextureParameter(GLTexture, TextureParameterName.TextureMaxLod, max);
        }

        public void SetWrap(TextureCoordinate coord, TextureWrapMode mode)
        {
            GL.TextureParameter(GLTexture, (TextureParameterName)coord, (int)mode);
        }

        public void Dispose()
        {
            GL.DeleteTexture(GLTexture);
        }
    }
}
