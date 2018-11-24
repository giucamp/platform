using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Platform
{
    class Mario
    {
        private Body m_body;
        private Sprite m_sprite;
        private float m_walk_speed = 2.0f;
        private float m_walk_force = 80.0f;
        private float m_walk_anim_iterator = 0;
        private float m_walk_anim_speed = 0.1f;
        private float m_walk_still_vel = 0.3f;
        private bool m_walk_forward = false;
        private bool m_walk_backward = false;

        public Mario(World i_world)
        {
            m_body = i_world.AddBody();
            m_sprite = m_body.AddSprite(Utils.DataFile("textures/mario-3.GIF"), 5, 9);

            m_body.BeforeUpdate += Update;

            m_body.PositionY = 4;
        }

        void Update(Body i_body, float i_dt)
        {
            float scalar_dir = m_walk_forward ? 1 : 0;
            scalar_dir += m_walk_backward ? -1 : 0;

            Vector2 dir = new Vector2(1, 0) * scalar_dir;

            float curr_velocity = Vector2.Dot(m_body.Velocity, dir);

            Vector2 force = dir * ((m_walk_speed - curr_velocity) * m_walk_force);

            m_walk_anim_iterator += m_body.VelocityX * m_walk_anim_speed;
            if (m_walk_anim_iterator >= 3)
                m_walk_anim_iterator += 3;
            if (m_walk_anim_iterator <= 3)
                m_walk_anim_iterator -= 3;
            if (Math.Abs(m_body.VelocityX) < m_walk_still_vel)
                m_walk_anim_iterator = 0;
            else
                m_sprite.FlipX = m_body.VelocityX < 0;
            
            int anim_index = Math.Abs((int)m_walk_anim_iterator) % 3;
            m_sprite.AnimationX = anim_index;
            
            m_body.AddForce(force);
        }

        public bool WalkForward { get => m_walk_forward; set => m_walk_forward = value; }
        public bool WalkBackward { get => m_walk_backward; set => m_walk_backward = value; }

        void ResetActions()
        {
            WalkForward = false;
            WalkForward = false;
        }
    }
}
