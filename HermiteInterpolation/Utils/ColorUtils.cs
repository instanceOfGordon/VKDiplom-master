using System;
using HermiteInterpolation.Numerics;
using Microsoft.Xna.Framework;

namespace HermiteInterpolation.Utils
{
    public class ColorUtils
    {
        public static Color FromHsv(float h, float s, float v, float a)
        {
//            double H = h;
//            while (H < 0) { H += 360; };
//            while (H >= 360) { H -= 360; };
            float R, G, B;
            if (v <= 0)
            {
                R = G = B = 0;
            }
            else if (s <= 0)
            {
                R = G = B = v;
            }
            else
            {
                float hf = h/60.0f;
                var i = (int) Math.Floor(hf);
                float f = hf - i;
                float pv = v*(1 - s);
                float qv = v*(1 - s*f);
                float tv = v*(1 - s*(1 - f));
                switch (i)
                {
                        // Red is the dominant color

                    case 0:
                        R = v;
                        G = tv;
                        B = pv;
                        break;

                        // Green is the dominant color

                    case 1:
                        R = qv;
                        G = v;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = v;
                        B = tv;
                        break;

                        // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = v;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = v;
                        break;

                        // Red is the dominant color

                    case 5:
                        R = v;
                        G = pv;
                        B = qv;
                        break;

                        // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = v;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = v;
                        G = pv;
                        B = qv;
                        break;

                        // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = v; // Just pretend its black/white
                        break;
                }
            }
            var r = (int) MathHelper.Clamp(R*255, 0, 255);
            var g = (int) MathHelper.Clamp(G*255, 0, 255);
            var b = (int) MathHelper.Clamp(B*255, 0, 255);
            var al = (int) MathHelper.Clamp(a*255, 0, 255);

            return Color.FromNonPremultiplied(r, g, b, al);
        }

        public static Color Random()
        {
            var random = RandomNumber.Instance;
            var r = random.Next(64, 164);
            var g = random.Next(64, 164);
            var b = random.Next(64, 164);
            return Color.FromNonPremultiplied(r, g, b, 255);
        }

    }
}