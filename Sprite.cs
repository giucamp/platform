using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Platform
{
    public class Sprite
    {
        private int m_texture;
        private Vector2 m_position = Vector2.Zero;
        private Vector2 m_local_pivot = Vector2.Zero;
        private Vector2 m_size = new Vector2(1, 1);
        private float m_angle_deg = 0;
        private Color4 m_modulation = Color4.White;
        private int m_animation_grid_width;
        private int m_animation_grid_height;
        private int m_animation_x = 0;
        private int m_animation_y = 0;

        public Vector2 Position { get => m_position; set => m_position = value; }

        public Vector2 LocalPivot { get => m_local_pivot; set => m_local_pivot = value; }

        public float X { get => m_position.X; set => m_position.X = value; }

        public float Y { get => m_position.Y; set => m_position.Y = value; }

        public Vector2 Size { get => m_size; set => m_size = value; }

        public float AngleDeg { get => m_angle_deg; set => m_angle_deg = value; }

        public Color4 Modulation
        {
            get { return m_modulation; }
            set { m_modulation = value; }
        }

        public float ModulationAlpha
        {
            get { return m_modulation.A; }
            set { m_modulation.A = value; }
        }

        public Vector3 ModulationColor
        {
            get { return new Vector3(m_modulation.R, m_modulation.G, m_modulation.B); }
            set { m_modulation = new Color4(value.X, value.Y, value.Z, m_modulation.A); }
        }

        bool m_uv_dirty = true;
        bool m_flip_x = false;
        bool m_flip_y = false;
        float m_u0, m_u1, m_v0, m_v1;

        public int AnimationX
        { 
            get => m_animation_x;
            set 
            { 
                m_animation_x = value;
                m_uv_dirty = true;
            }
        }

        public int AnimationY
        {
            get => m_animation_x;
            set
            {
                m_animation_x = value;
                m_uv_dirty = true;
            }
        }

        public bool FlipX
        {
            get => m_flip_x;
            set
            {
                m_flip_x = value;
                m_uv_dirty = true;
            }
        }

        public bool FlipY
        {
            get => m_flip_y;
            set
            {
                m_flip_y = value;
                m_uv_dirty = true;
            }
        }

        public Sprite(string i_file_name, int i_animation_grid_width = 1, int i_animation_grid_height = 1)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.ClearColor(0,0,0,1);

            m_animation_grid_width = i_animation_grid_width;
            m_animation_grid_height = i_animation_grid_height;

            m_texture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, m_texture);


            Bitmap bmp = new Bitmap(i_file_name);

            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), 
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            bmp.UnlockBits(bmp_data);
        }

        public void Draw()
        {
            Vector2 half_size = m_size * 0.5f;
            /*
                0   1
                3   2
             
             */

            if (m_uv_dirty)
            {
                m_uv_dirty = false;

                float u_size = 1.0f / m_animation_grid_width;                
                m_u0 = u_size * m_animation_x;
                m_u1 = m_u0 + u_size;

                float v_size = 1.0f / m_animation_grid_height;
                m_v0 = v_size * m_animation_y;
                m_v1 = m_v0 + v_size;

                if (m_flip_x)
                    Utils.Swap(ref m_u0, ref m_u1);

                if (m_flip_y)
                    Utils.Swap(ref m_v0, ref m_v1);
            }

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BindTexture(TextureTarget.Texture2D, m_texture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);

            GL.PushMatrix();
            GL.Translate(m_position.X, m_position.Y, 0);
            GL.Rotate(m_angle_deg, 0, 0, 1);

            GL.Begin(PrimitiveType.Quads);

            GL.Color4(m_modulation);


            // 0
            GL.TexCoord2(m_u0, m_v1);
            GL.Vertex2((m_local_pivot.X + 1) * -half_size.X, (m_local_pivot.Y + 1) * -half_size.Y);

            // 1
            GL.TexCoord2(m_u1, m_v1);
            GL.Vertex2((m_local_pivot.X + 1) * half_size.X, (m_local_pivot.Y + 1) * -half_size.Y);

            // 2
            GL.TexCoord2(m_u1, m_v0);
            GL.Vertex2((m_local_pivot.X + 1) * half_size.X, (m_local_pivot.Y + 1) * half_size.Y);
            
            // 3
            GL.TexCoord2(m_u0, m_v0);
            GL.Vertex2((m_local_pivot.X + 1) * -half_size.X, (m_local_pivot.Y + 1) * half_size.Y);

            GL.End();

            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);
        }
    }
}
