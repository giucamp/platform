using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public class World
    {
        private List<Body> m_bodies;
        private IReadOnlyCollection<Body> m_readonly_bodies;
        private float m_floor = 0.0f;
        private float m_gravity = 9.81f;

        public World()
        {
            m_bodies = new List<Body>();
            m_readonly_bodies = m_bodies.AsReadOnly();
        }
        
        public Body AddBody()
        {
            Body body = new Body();
            m_bodies.Add(body);
            return body;
        }

        public void Update(float i_time_step)
        {
            foreach (Body body in m_bodies)
            {
                body.AddForce(0, -m_gravity);
                body.Update(i_time_step);

                if(body.PositionY < m_floor)
                {
                    body.PositionY = m_floor + (m_floor - body.PositionY) * body.Restitution;
                    body.VelocityY = -body.VelocityY * body.Restitution;
                }
            }
        }

        public void Draw()
        {
            foreach (Body body in m_bodies)
                body.Draw();
        }
    }
}
