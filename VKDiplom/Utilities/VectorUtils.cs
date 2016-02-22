using System;
using Microsoft.Xna.Framework;

namespace VKDiplom.Utilities
{
    public static class VectorUtils
    {
        public static float Angle(Vector2 a, Vector2 b)
        {
            var dot = Vector2.Dot(a, b);
            var mulLength = a.Length()*b.Length();
            return (float) Math.Acos(dot/mulLength);
        }
    }
}