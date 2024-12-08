using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using static System.Formats.Asn1.AsnWriter;

namespace ScreensaverTests
{
    internal class SpriteRenderer
    {

        public static Shader SpriteShader;
        public static Camera SpriteCamera = new Camera((0, 0, 1), -Vector3.UnitZ, Vector3.UnitY, CameraType.Orthographic, 0.0f);
        private static int _ibo, _vbo, _vao;
        private static List<int> _indices = new List<int>();
        private static List<float> _vertices = new List<float>();
        public static void Start()
        {

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ibo = GL.GenBuffer();

        }

        public static void Stop()
        {

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * sizeof(float), _vertices.ToArray(), BufferUsage.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count * sizeof(int), _indices.ToArray(), BufferUsage.StaticDraw);

            GL.BindVertexArray(0);

        }

    }
}
