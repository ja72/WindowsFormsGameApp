﻿using System;
using System.Drawing;
using System.Numerics;

namespace JA.Game
{
    using static Geometry;

    public class Brick : PhysicsObject
    {
        private int _strength;

        public Brick(Color color, int strength=1) : base(color)
        {
            Size = new Vector2(12f, 4f);
            Fixed = true;
            AngleDeg = 0;
            Omega = 0;
            Strength = strength;
        }
        public override float Mmoi => Mass / 12 * (Size.X * Size.X + Size.Y * Size.Y);
        public Vector2 Size { get; set; }
        public int Strength
        {
            get => _strength;
            set
            {
                _strength = Math.Max(0, value);
                Pen.Width = _strength + 1;
                Color = Color.FromKnownColor((KnownColor)(KnownColor.DarkBlue + _strength));
            }
        }

        public bool Intersects(Ball ball, out Vector2 point, out Vector2 normal)
        {
            point = GetClosestPointTo(ball.Center, out normal);
            float d = Vector2.Distance(point, ball.Center);
            return d <= ball.Radius;
        }
        /// <summary>
        /// Gets the closest point on the brick to the target point.
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="normal">The normal vector at the resulting point.</param>
        /// <returns>A point.</returns>
        public override Vector2 GetClosestPointTo(Vector2 target, out Vector2 normal)
        {
            // a --- b
            // |     |
            // c --- d
            Vector2 a = Center + new Vector2(-Size.X / 2, -Size.Y / 2).Rotate(AngleDeg*pi/180);
            Vector2 b = Center + new Vector2(Size.X / 2, -Size.Y / 2).Rotate(AngleDeg*pi/180);
            Vector2 c = Center + new Vector2(-Size.X / 2, Size.Y / 2).Rotate(AngleDeg*pi/180);
            Vector2 d = Center + new Vector2(Size.X / 2, Size.Y / 2).Rotate(AngleDeg*pi/180);

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
                if (w_ab.w_A.IsClamped(0, 1, true) && w_ab.w_B.IsClamped(0, 1, true))
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
                if (w_dc.w_A.IsClamped(0, 1, true) && w_dc.w_B.IsClamped(0, 1, true))
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
                if (w_ca.w_A.IsClamped(0, 1, true) && w_ca.w_B.IsClamped(0, 1, true))
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
                if (w_bd.w_A.IsClamped(0, 1, true) && w_bd.w_B.IsClamped(0, 1, true))
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

        public bool Impact(Ball ball, float cor, float friction)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws the brick.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="game">The game object.</param>
        public override void Draw(Graphics graphics, DxGame game)
        {
            SizeF pixel_dia = game.GetSpan(Size);
            PointF pixel_cen = game.GetPixel(Center);

            var state = graphics.Save();

            graphics.TranslateTransform(pixel_cen.X, pixel_cen.Y);
            graphics.RotateTransform(-AngleDeg);

            graphics.FillRectangle(Fill,
                - pixel_dia.Width / 2,
                - pixel_dia.Height / 2,
                pixel_dia.Width, pixel_dia.Height);

            graphics.DrawRectangle(Pen,
                - pixel_dia.Width / 2,
                - pixel_dia.Height / 2,
                pixel_dia.Width, pixel_dia.Height);

            //var hit = GetClosestPointTo(game.Ball.Center, out var n);
            //PointF pixel_hit = game.GetPixel(hit);
            //graphics.FillEllipse(Brushes.YellowGreen,
            //    pixel_hit.X - 4, pixel_hit.Y - 4,
            //    8, 8);

            graphics.Restore(state);
        }
    }
}
