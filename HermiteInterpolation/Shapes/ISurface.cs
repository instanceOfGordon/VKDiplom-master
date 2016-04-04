using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Shapes
{
    public interface ISurface : IDrawable
    {
        DrawStyle DrawStyle { get; set; }
        float MinHeight { get; }
        float MaxHeight { get; }
        void ColoredSimple(Color color);
        void ColoredHeight();
        void ColoredHeight(float fromHue, float toHue);
    }
}