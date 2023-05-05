using System.Drawing;
using System.Numerics;

namespace JA.Game
{
    using System;
    using System.Drawing.Drawing2D;
    using static Geometry;

    public class Paddle : GraphicsObject
    {
        public Paddle() : base(Color.Magenta)
        {
            Size = new Vector2(18f, 3f);
            Pen.Width = 2;
        }
        public float Left { get => Center.X - Size.X / 2; }
        public float Right { get => Center.X + Size.X / 2; }
        public float Top { get => Center.Y - Size.Y / 2; }
        public float Bottom { get => Center.Y + Size.Y / 2; }

        public Vector2 Center { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Size { get; set; }

        public Vector2[] GetPolygon()
        {
            Vector2 a = Center + new Vector2(-Size.X / 2, -Size.Y / 2);
            Vector2 b = Center + new Vector2(Size.X / 2, -Size.Y / 2);
            Vector2 c = Center + new Vector2(-Size.X / 1.5f , Size.Y / 2);
            Vector2 d = Center + new Vector2(Size.X / 1.5f, Size.Y / 2);
            return new[] { a, b, d, c };
        }

        public bool Intersects(Ball ball, out Vector2 point, out Vector2 normal)
        {
            point = GetClosestPointTo(ball.Center, out normal);
            float d = Vector2.Distance(point, ball.Center);
            return d <= ball.Radius;
        }
        /// <summary>
        /// Gets the closest point on the paddle to the target point.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="normal">The normal vector at the resulting point.</param>
        /// <returns>A point.</returns>
        public override Vector2 GetClosestPointTo(Vector2 target, out Vector2 normal)
        {
            // a --- b
            // |     |
            // c --- d
            Vector2 a = Center + new Vector2(-Size.X / 2, -Size.Y / 2);
            Vector2 b = Center + new Vector2(Size.X / 2, -Size.Y / 2);
            Vector2 c = Center + new Vector2(-Size.X / 1.5f , Size.Y / 2);
            Vector2 d = Center + new Vector2(Size.X / 1.5f, Size.Y / 2);

            Vector2 n_ab = (a - b).GetNormal();
            Vector2 n_ca = (c - a).GetNormal();
            Vector2 n_dc = (d - c).GetNormal();
            Vector2 n_bd = (b - d).GetNormal();

            var w_ab = GetBarycentricCoord(target, a, b, true);
            var w_dc = GetBarycentricCoord(target, c, d, true);
            var w_ca = GetBarycentricCoord(target, a, c, true);
            var w_bd = GetBarycentricCoord(target, b, d, true);

            var p_ab = GetPointOnLine(w_ab, a, b);
            var p_dc = GetPointOnLine(w_dc, c, d);
            var p_ca = GetPointOnLine(w_ca, a, c);
            var p_bd = GetPointOnLine(w_bd, b, d);

            var d_ab = Vector2.Distance(target, p_ab);
            var d_dc = Vector2.Distance(target, p_dc);
            var d_ca = Vector2.Distance(target, p_ca);
            var d_bd = Vector2.Distance(target, p_bd);

            var d_min = Math.Min(Math.Min(d_ab, d_dc), Math.Min(d_ca, d_bd));
            if (d_min == d_ab)
            {
                if (w_ab.w_A.IsClamped(0,1,true) && w_ab.w_B.IsClamped(0,1,true))
                {
                    normal = n_ab;
                }
                else
                {
                    normal = Vector2.Normalize(target - p_ab);
                }
                return p_ab;
            }

            if (d_min == d_dc)
            {
                if (w_dc.w_A.IsClamped(0,1,true) && w_dc.w_B.IsClamped(0,1,true))
                {
                    normal = n_dc;
                }
                else
                {
                    normal = Vector2.Normalize(target - p_dc);
                }
                return p_dc;
            }

            if (d_min == d_ca)
            {
                if (w_ca.w_A.IsClamped(0,1,true) && w_ca.w_B.IsClamped(0,1,true))
                {
                    normal = n_ca;
                }
                else
                {
                    normal = Vector2.Normalize(target - p_ca);
                }
                return p_ca;
            }

            if (d_min == d_bd)
            {
                if (w_bd.w_A.IsClamped(0,1,true) && w_bd.w_B.IsClamped(0,1,true))
                {
                    normal = n_bd;
                }
                else
                {
                    normal = Vector2.Normalize(target - p_bd);
                }
                return p_bd;
            }
            normal = Vector2.Zero;
            return Vector2.Zero;
        }

        /// <summary>
        /// Draws the paddle.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="game">The game object.</param>
        public override void Draw(Graphics graphics, DxGame game)
        {
            Vector2 a = Center + new Vector2(-Size.X / 2, -Size.Y / 2);
            Vector2 b = Center + new Vector2(Size.X / 2, -Size.Y / 2);
            Vector2 c = Center + new Vector2(-Size.X / 1.5f , Size.Y / 2);
            Vector2 d = Center + new Vector2(Size.X / 1.5f, Size.Y / 2);

            PointF pa = game.GetPixel(a);
            PointF pb = game.GetPixel(b);
            PointF pc = game.GetPixel(c);
            PointF pd = game.GetPixel(d);

            var poly = new[] { pa, pb, pd, pc };

            graphics.FillPolygon(Fill, poly);
            graphics.DrawPolygon(Pen, poly);

            //graphics.FillEllipse(Brushes.DarkBlue,
            //    pa.X - 4, pa.Y - 4,
            //    8, 8);
            //graphics.FillEllipse(Brushes.DarkBlue,
            //    pb.X - 4, pb.Y - 4,
            //    8, 8);
            //graphics.FillEllipse(Brushes.DarkBlue,
            //    pc.X - 4, pc.Y - 4,
            //    8, 8);
            //graphics.FillEllipse(Brushes.DarkBlue,
            //    pd.X - 4, pd.Y - 4,
            //    8, 8);

            //var hit = GetClosestPointTo(game.Ball.Center, out var n);
            //PointF pixel_hit = game.GetPixel(hit);
            //graphics.FillEllipse(Brushes.YellowGreen,
            //    pixel_hit.X - 4, pixel_hit.Y - 4,
            //    8, 8);

            //SizeF pixel_n = game.GetSpan(5 * n);
            //using (var pen = new Pen(Color.Red, 2))
            //{
            //    pen.EndCap = LineCap.ArrowAnchor;
            //    graphics.DrawLine(pen,
            //        pixel_hit.X, pixel_hit.Y,
            //        pixel_hit.X + pixel_n.Width,
            //        pixel_hit.Y + pixel_n.Height);
            //}


        }
    }
}
