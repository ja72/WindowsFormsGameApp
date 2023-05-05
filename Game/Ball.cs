using System.Drawing;
using System.Numerics;

namespace JA.Game
{
    public class Ball : PhysicsObject
    {

        public Ball() : base(Color.LightGray)
        {
            Diameter = 6;
            Fixed = false;
        }
        public override float Mmoi => Mass / 8 * Diameter * Diameter;
        public float Diameter { get; set; }
        public float Radius { get => Diameter / 2; set => Diameter = 2 * value; }

        public override Vector2 GetClosestPointTo(Vector2 target, out Vector2 normal)
        {
            normal = Vector2.Normalize(target - Center);
            return Center + Radius * normal;
        }

        public Vector2 ImpactPlane(Vector2 point, Vector2 normal, Vector2 velocity, float cor, float friction)
        {
            float d = Vector2.Dot(normal, Center - point) - Radius;
            float v_imp = Vector2.Dot(normal, Velocity - velocity);
            Vector2 impulse = Vector2.Zero;
            if (d <= 0 && v_imp < 0)
            {
                float J = -(1 + cor) * v_imp;
                Vector2 dv = Velocity - velocity - normal * v_imp;
                float v_slip = dv.Length();
                if (v_slip > 0)
                {
                    Vector2 e_slip = Vector2.Normalize(dv);
                    float Jf = -v_slip;
                    Jf = Jf.Clamp(-friction * J, friction * J);
                    impulse = normal * J + e_slip * Jf;
                }
                else
                {
                    impulse = normal * J;
                }
                Velocity += impulse;
                Center -= d * normal;
            }
            return impulse;
        }

        public override void Draw(Graphics graphics, DxGame game)
        {
            float pixel_dia = game.Scale * Diameter;
            PointF pixel_cen = game.GetPixel(Center);


            if (Radius > 3)
            {
                Pen.Color = Color.RosyBrown;
            }
            else
            {
                Pen.Color = Color.White;
            }

            graphics.FillEllipse(Fill,
                pixel_cen.X - pixel_dia / 2,
                pixel_cen.Y - pixel_dia / 2,
                pixel_dia, pixel_dia);
            graphics.DrawEllipse(Pen,
                pixel_cen.X - pixel_dia / 2,
                pixel_cen.Y - pixel_dia / 2,
                pixel_dia, pixel_dia);
        }
    }
}
