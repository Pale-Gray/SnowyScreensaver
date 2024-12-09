using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using static System.Formats.Asn1.AsnWriter;
using System.Drawing;

namespace ScreensaverTests
{
    internal class Sprite
    {

        public Shader SpriteShader;
        public static Camera SpriteCamera = new Camera((0, 0, 1), -Vector3.UnitZ, Vector3.UnitY, CameraType.Orthographic, 0.0f);
        private static int _vao, _vbo, _ibo;
        private static int[] indices = { 0, 1, 2, 2, 3, 0 };

        private Texture _texture;
        public Vector2 Size;
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Origin;
        public Vector3 Rotation;
        public float Scale;
        public float Alpha = 1.0f;

        public static string vertexShaderString =
            """
            #version 400 core
            layout (location=0) in vec2 position;
            layout (location=1) in vec2 texcoord;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            uniform mat4 rotation;

            out vec2 vposition;
            out vec2 vworldposition;
            out vec3 vnormal;
            out vec2 vtexcoord;
            out mat4 vmodel;

            out mat4 vrotation;

            void main()
            {

            	vposition = position;
            	vworldposition = (vec4(position, 0, 0) * model).xy;
            	vtexcoord = texcoord;

            	gl_Position = vec4(position, -1.0, 1.0) * model * view * projection;

            }
            """;
        public static string fragmentShaderString = """
            #version 400 core
            out vec4 FragColor;

            in vec2 vposition;
            in vec2 vtexcoord;
            in vec2 vworldposition;

            uniform sampler2D tex;
            uniform vec2 origin;
            uniform vec2 size;
            uniform float nullTexture;
            uniform vec2 screenResolution;
            uniform vec2 screenPosition;
            uniform float spriteAlpha;
            uniform float time;

            void main()
            {

                float height = screenPosition.y / screenResolution.y;

                if (nullTexture == 0.0) {
                    vec4 texColor = texture(tex, vtexcoord);
                    FragColor = vec4(texColor.rgb,(texColor.a * spriteAlpha) * min(1.0, (height * 4.0)));
                } else {
                    FragColor = vec4(vec3(1),1.0 - (height/2.0));
                }

            }
            """;

        public Sprite(Texture texture, Vector2 size, Vector2 position, Vector2 origin, Vector3 rotation, float scale, float alpha)
        {

            SpriteShader = new Shader(vertexShaderString, fragmentShaderString);

            _texture = texture;
            Size = size;
            Position = position;
            Origin = origin;
            Rotation = rotation;
            Scale = scale;
            Alpha = alpha;

            float[] vertices =
            {

                -(size.X * origin.X), -(size.Y * origin.Y), 0.0f, 1.0f,
                -(size.X * origin.X), size.Y - (size.Y * origin.Y), 0.0f, 0.0f,
                size.X - (size.X * origin.X), size.Y - (size.Y * origin.Y), 1.0f, 0.0f,
                size.X - (size.X * origin.X), -(size.Y * origin.Y), 1.0f, 1.0f

            };

            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_vao);
            GL.DeleteBuffer(_ibo);

            _vao = GL.GenVertexArray();

            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _ibo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsage.StaticDraw);

            GL.BindVertexArray(0);

        }

        public void Draw()
        {

            SpriteShader.Use();
            GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "tex"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            if (_texture != null)
            {

                GL.BindTexture(TextureTarget.Texture2d, _texture.Id);
                GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "nullTexture"), 0);

            }
            else
            {

                GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "nullTexture"), 1);

            }

            Matrix4 modelMatrix = Matrix4.CreateScale(Scale);
            modelMatrix *= Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z);
            modelMatrix *= Matrix4.CreateTranslation((Position.X, Position.Y, 0));

            GL.UniformMatrix4f(GL.GetUniformLocation(SpriteShader.id, "model"), 1, true, in modelMatrix);
            GL.UniformMatrix4f(GL.GetUniformLocation(SpriteShader.id, "view"), 1, true, in SpriteCamera.ViewMatrix);
            GL.UniformMatrix4f(GL.GetUniformLocation(SpriteShader.id, "projection"), 1, true, in SpriteCamera.ProjectionMatrix);
            GL.Uniform2f(GL.GetUniformLocation(SpriteShader.id, "origin"), Origin.X, Origin.Y);
            GL.Uniform2f(GL.GetUniformLocation(SpriteShader.id, "screenPosition"), Position.X, Position.Y);
            GL.Uniform2f(GL.GetUniformLocation(SpriteShader.id, "size"), Size.X, Size.Y);
            GL.Uniform2f(GL.GetUniformLocation(SpriteShader.id, "screenResolution"), GlobalValues.WIDTH, GlobalValues.HEIGHT);
            GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "time"), (float)GlobalValues.Time);
            GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "spriteAlpha"), Alpha);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            SpriteShader.UnUse();

            if (_texture != null)
            {

                GL.BindTexture(TextureTarget.Texture2d, 0);

            }

        }

        /*
        public static void Draw(Texture texture, Vector2 size, Vector2 position, Vector2 origin, Vector3 rotation, float scale)
        {

            float[] vertices =
            {

                -(size.X * origin.X), -(size.Y * origin.Y), 0.0f, 1.0f,
                -(size.X * origin.X), size.Y - (size.Y * origin.Y), 0.0f, 0.0f,
                size.X - (size.X * origin.X), size.Y - (size.Y * origin.Y), 1.0f, 0.0f,
                size.X - (size.X * origin.X), -(size.Y * origin.Y), 1.0f, 1.0f

            };

            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_vao);
            GL.DeleteBuffer(_ibo);

            _vao = GL.GenVertexArray();

            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            _ibo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsage.StaticDraw);

            GL.BindVertexArray(0);
            
            SpriteShader.Use();
            GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "tex"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            if (texture != null)
            {

                GL.BindTexture(TextureTarget.Texture2d, texture.Id);
                GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "nullTexture"), 0);

            }
            else
            {

                GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "nullTexture"), 1);

            }

            Matrix4 modelMatrix = Matrix4.CreateScale(scale);
            modelMatrix *= Matrix4.CreateRotationX(rotation.X) * Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationZ(rotation.Z);
            modelMatrix *= Matrix4.CreateTranslation((position.X, position.Y, 0));

            GL.UniformMatrix4f(GL.GetUniformLocation(SpriteShader.id, "model"), 1, true, in modelMatrix);
            GL.UniformMatrix4f(GL.GetUniformLocation(SpriteShader.id, "view"), 1, true, in SpriteCamera.ViewMatrix);
            GL.UniformMatrix4f(GL.GetUniformLocation(SpriteShader.id, "projection"), 1, true, in SpriteCamera.ProjectionMatrix);
            GL.Uniform2f(GL.GetUniformLocation(SpriteShader.id, "size"), size.X, size.Y);
            GL.Uniform2f(GL.GetUniformLocation(SpriteShader.id, "screenResolution"), GlobalValues.WIDTH, GlobalValues.HEIGHT);
            GL.Uniform1f(GL.GetUniformLocation(SpriteShader.id, "time"), (float)GlobalValues.Time);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            SpriteShader.UnUse();

            if (texture != null)
            {

                GL.BindTexture(TextureTarget.Texture2d, 0);

            }

            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_vao);
            GL.DeleteBuffer(_ibo);

        }
        */

        public static void Init()
        {

            // SpriteShader = new Shader("default.vert", "default.frag");

        }

    }
}
