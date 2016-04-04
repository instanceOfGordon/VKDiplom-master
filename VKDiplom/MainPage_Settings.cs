using System.Windows.Controls;
using System.Windows.Graphics;
using Microsoft.Xna.Framework.Graphics;
using VKDiplom.Engine;

namespace VKDiplom
{
    public partial class MainPage
    {
        public void SetMultiSampleAntialiasing(int samples)
        {
            SetMultiSampleAntialiasing(FunctionDrawingSurface, samples);
        }

        private static void SetMultiSampleAntialiasing(DrawingSurface drawingSurface, int samples)
        {
            drawingSurface.CompositionMode = new OffscreenCompositionMode
            {
                PreferredMultiSampleCount = samples,
                RenderTargetUsage = RenderTargetUsage.DiscardContents,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            };
        }

        public void SetLighting(Scene.LightingQuality lightingQuality)
        {
            _functionScene.SetLighting(lightingQuality);
        }

        public void SetPerPixelLighting(bool value)
        {
            _functionScene.PreferPerPixelLighting = value;
        }
    }
}