using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScreensaverTests
{
    internal class Texture
    {

        public int Id;
        ImageResult Image;
        public string Path;
        public string FileName;

        public byte[] Data;

        public int Width, Height;

        public Texture(byte[] textureBytes, int width, int height)
        {

            Id = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2d, Id);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, textureBytes);

            GL.BindTexture(TextureTarget.Texture2d, 0);

            Width = width;
            Height = height;
            Data = textureBytes;

        }

        public Texture(string imageFile)
        {

            Id = GL.GenTexture();

            FileName = imageFile;
            Path = FileName;

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2d, Id);

            StbImage.stbi_set_flip_vertically_on_load(1);
         
            using (FileStream stream = File.OpenRead(Path))
            {

                Image = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

            }

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, Image.Width, Image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Image.Data);

            GL.BindTexture(TextureTarget.Texture2d, 0);

            Width = Image.Width;
            Height = Image.Height;
            Data = Image.Data;

        }

        public void Dispose()
        {

            GL.DeleteTexture(Id);

        }

    }
}
