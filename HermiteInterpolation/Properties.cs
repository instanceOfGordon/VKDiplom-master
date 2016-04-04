using Microsoft.Xna.Framework;

namespace HermiteInterpolation
{
    public static class Properties
    {
        public static float MeshDensity { get; } = 0.2f;
        public static float MaxSceneZDifference { get; } = 40f;

        public static class Constants
        {
            public static readonly Color DefaultColor =
                Color.FromNonPremultiplied(128, 128, 128, 255);

            public static readonly Vector3 DefaultNormal = Vector3.Zero;
        }
    }
}