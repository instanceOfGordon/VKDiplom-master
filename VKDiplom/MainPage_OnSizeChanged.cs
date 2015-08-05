using System.Windows;
using System.Windows.Controls;
using VKDiplom.Engine;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            //            var drawingSurface = sender as DrawingSurface;
            //            if (drawingSurface != null && _functionScene != null)
            //                _functionScene.Camera.AspectRatio = (float) drawingSurface.ActualWidth/(float) drawingSurface.ActualHeight;
            //            _mouseDeltaScale = 300.0 / Math.Min(drawingSurface.ActualWidth, drawingSurface.ActualHeight);
            SizeChanged(sender as DrawingSurface, _functionScene);
        }

        private void FirstDerivationDrawingSurface_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChanged(sender as DrawingSurface, _firstDerScene);
        }

        private void SecondDerivationDrawingSurface_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SizeChanged(sender as DrawingSurface, _secondDerScene);
        }

        private new void SizeChanged(DrawingSurface drawingSurface, Scene scene)
        {
            if (drawingSurface == null || _functionScene == null) return;

            scene.Camera.AspectRatio = (float) drawingSurface.ActualWidth/
                                       (float) drawingSurface.ActualHeight;

            //_mouseDeltaScale = 300.0/Math.Min(drawingSurface.ActualWidth, drawingSurface.ActualHeight);
        }
    }
}