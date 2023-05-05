using System.Drawing;
using System.Numerics;

namespace JA.Game
{
    using static Geometry;

    public abstract class PhysicsObject : GraphicsObject
    {
        protected PhysicsObject(Color color) : base(color)
        {
            Mass = 1;
            AngleDeg = 0;
            Omega = 0;
        }
        public float Mass { get; set; }
        public abstract float Mmoi { get; }
        public Vector2 Center { get; set; }
        public Vector2 Velocity { get; set; }
        public float AngleDeg { get; set; }
        public float Omega { get; set; }

        public void OnUpdate(float elapsed, Vector2 acceleration, float alpha = 0)
        {
            Omega += elapsed * alpha;
            AngleDeg += elapsed * (Omega * 180 / pi);
            Velocity += elapsed * acceleration;
            Center += elapsed * Velocity;
        }

        public Vector2 GetVelocityAt(Vector2 point)
        {
            Vector2 delta = point - Center;
            return Velocity + new Vector2(-delta.Y * Omega, delta.X * Omega);
        }

        public void ApplyImpulse(Vector2 impulse, Vector2 point)
        {
            Vector2 delta = point - Center;
            Velocity += impulse / Mass;
            Omega += (delta.X * impulse.Y - delta.Y * impulse.X) / Mmoi;
        }

        public float GetInverseMass(Vector2 direction, Vector2 point)
        {
            Vector2 c = Center - point;
            float nxc = direction.X * c.Y - direction.Y * c.X;
            return 1 / Mass + (nxc * nxc) / Mmoi;
        }
    }
}
