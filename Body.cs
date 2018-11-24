using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Platform
{
    public delegate void UpdateDelegate(Body i_body, float i_dt);

    public class Body
    {
        private List<Sprite> m_sprites;
        private IReadOnlyCollection<Sprite> m_readonly_sprites;
        private float m_mass = 1.0f;
        private float m_inertia = 1.0f;
        private float m_restitution = 0.5f;
        private float m_linear_damping = 5.0f;
        private float m_angular_damping = 1.0f;

        private Vector2 m_position = Vector2.Zero;
        private float m_rotation = 0.0f;
        private Vector2 m_velocity = Vector2.Zero;
        private float m_angular_velocity = 0.0f;
        private Vector2 m_force_accumulator = Vector2.Zero;
        private float m_torque_accumulator = 0.0f;

        public event UpdateDelegate BeforeUpdate;
        public event UpdateDelegate AfterUpdate;

        public Body()
        {
            m_sprites = new List<Sprite>();
            m_readonly_sprites = m_sprites.AsReadOnly();
        }

        public IReadOnlyCollection<Sprite> Sprites { get => m_readonly_sprites; }

        public float Mass { get => m_mass; set => m_mass = value; }
        public float Inertia { get => m_inertia; set => m_inertia = value; }
        public float Restitution { get => m_restitution; set => m_restitution = value; }
        public float LinearDamping { get => m_linear_damping; set => m_linear_damping = value; }
        public float AngularDamping { get => m_angular_damping; set => m_angular_damping = value; }

        public Vector2 Position { get => m_position; set => m_position = value; }
        public float PositionX { get => m_position.X; set => m_position.X = value; }
        public float PositionY { get => m_position.Y; set => m_position.Y = value; }

        public float Rotation { get => m_rotation; set => m_rotation = value; }

        public Vector2 Velocity { get => m_velocity; set => m_velocity = value; }
        public float VelocityX { get => m_velocity.X; set => m_velocity.X = value; }
        public float VelocityY { get => m_velocity.Y; set => m_velocity.Y = value; }
        
        public float AngularVelocity { get => m_angular_velocity; set => m_angular_velocity = value; }
        
        public Sprite AddSprite(Sprite i_sprite)
        {
            m_sprites.Add(i_sprite);
            return i_sprite;
        }

        public Sprite AddSprite(string i_file_name, int i_animation_grid_width = 1, int i_animation_grid_height = 1)
        {
            return AddSprite(new Sprite(i_file_name, i_animation_grid_width, i_animation_grid_height));
        }

        public void AddForce(Vector2 i_force)
        {
            m_force_accumulator += i_force;
        }

        public void AddForce(float i_force_x, float i_force_y)
        {
            m_force_accumulator.X += i_force_x;
            m_force_accumulator.Y += i_force_y;
        }

        public void AddTorque(float i_torque)
        {
            m_torque_accumulator += i_torque;
        }

        public void Update(float i_dt)
        {
            if (BeforeUpdate != null)
                BeforeUpdate(this, i_dt);

            m_velocity += m_force_accumulator * i_dt / m_mass;
            m_angular_velocity += m_torque_accumulator * i_dt / m_inertia;

            m_velocity -= m_velocity * (m_linear_damping * i_dt);
            m_angular_velocity -= m_angular_velocity * (m_angular_damping * i_dt);

            m_position += m_velocity * i_dt;
            m_rotation += m_angular_velocity * i_dt;

            m_force_accumulator = Vector2.Zero;
            m_torque_accumulator = 0.0f;

            foreach (Sprite sprite in m_sprites)
                sprite.Position = m_position;

            if (AfterUpdate != null)
                AfterUpdate(this, i_dt);
        }

        public void Draw()
        {
            foreach (Sprite sprite in m_sprites)
                sprite.Draw();
        }
    }
}
