using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JA
{
    public static class Geometry
    {
        public const float pi = (float)Math.PI;
        public static Vector2 Polar(float magnitude, float angle)
        {
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);

            return new Vector2(magnitude * c, magnitude * s);
        }
        public static Vector2 PolarDegrees(float magnitude, float angleDegrees)
            => Polar(magnitude, angleDegrees * pi/ 180);

        public static Vector2 GetDirection(this Vector2 vector)
            => Vector2.Normalize(vector);

        public static Vector2 GetNormal(this Vector2 vector)
        {
            return Vector2.Normalize(new Vector2(-vector.Y, vector.X));
        }
        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            float c = (float)Math.Cos(angle), s = (float)Math.Sin(angle);
            return new Vector2(
                c * vector.X - s * vector.Y,
                s * vector.X + c * vector.Y);
        }
        public static Vector2 RotateAbout(this Vector2 point, float angle, Vector2 pivot)
        {
            return pivot + (point - pivot).Rotate(angle);
        }
        public static (float w_A, float w_B) GetBarycentricCoord(Vector2 point, Vector2 a, Vector2 b, bool clamped = false)
        {
            Vector2 e = Vector2.Normalize(b - a);
            Vector2 Q = a + e * Vector2.Dot(e, point - a);
            // Q = w_A*a+ w_B*b

            float w_B = Cross(Q, a) / Cross(b, a);
            float w_A = Cross(Q, b) / Cross(a, b);

            if (clamped)
            {
                (w_A, w_B) = (Math.Max(0, Math.Min(1, w_A)), Math.Max(0, Math.Min(1, w_B)));
            }
            return (w_A, w_B);
        }
        public static Vector2 GetPointOnLine((float w_A, float w_B) coord, Vector2 a, Vector2 b)
        {
            return coord.w_A * a + coord.w_B * b;
        }
        public static Vector2 ProjectPointOnLine(Vector2 point, Vector2 a, Vector2 b)
        {
            return GetPointOnLine(GetBarycentricCoord(point, a, b), a, b);
        }

        public static float Cross(Vector2 a, Vector2 b)
            => a.X * b.Y - a.Y * b.X;

        public static float Clamp(this float x, float minValue = 0, float maxValue = 1)
        {
            return x < minValue ? minValue :
                x > maxValue ? maxValue : x;
        }
        public static bool IsClamped(this float x, float minValue = 0, float maxValue = 1, bool exclusive = false)
            => exclusive ? x > minValue && x < maxValue :  x >= minValue && x <= maxValue;

        public static bool IsNaN(this Vector2 vector)
        {
            return float.IsNaN(vector.X) || float.IsNaN(vector.Y);
        }
    }
}
