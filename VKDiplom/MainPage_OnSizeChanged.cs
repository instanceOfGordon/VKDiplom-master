using System;
using System.Windows;
using System.Windows.Controls;
using HermiteInterpolation.Shapes;
using Microsoft.Xna.Framework;
using VKDiplom.Engine;
using Point = System.Windows.Point;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var drawingSurface = sender as DrawingSurface;
            if(drawingSurface==null) return;
            SizeChanged(drawingSurface, _functionScene);
            //var transform = drawingSurface.TransformToVisual(Application.Current.RootVisual);
            //var surfacePosittion = transform.Transform(new Point(0, 0));

            //_center = new Point(surfacePosittion.X + 0.5*FunctionDrawingSurface.ActualWidth,
            //    surfacePosittion.Y + 0.5*FunctionDrawingSurface.ActualHeight);
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
        }

        private void ScaleSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scaler = sender as Slider;
            if (scaler == null) return;
            if (_functionScene == null) return;

            var tresholdToDefault = 0.025 * (scaler.Maximum - scaler.Minimum);
            //if ((e.OldValue < 1-tresholdToDefault && e.NewValue > 1-tresholdToDefault) || (e.OldValue > 1+tresholdToDefault && e.NewValue < 1+tresholdToDefault))
            if (e.NewValue > 1 - tresholdToDefault && e.NewValue < 1 + tresholdToDefault)
                scaler.Value = 1;
            ScenesAction(scene=>scene.Scale = new Vector3(1,1,(float)Math.Pow(scaler.Value,2)));
        }

       

        
    }
}