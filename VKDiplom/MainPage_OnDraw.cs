using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HermiteInterpolation;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;
using HermiteInterpolation.Utils;
using Microsoft.Xna.Framework;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;
using static VKDiplom.Constants;
using Color = Microsoft.Xna.Framework.Color;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnDraw(object sender, DrawEventArgs e)
        {
            DrawScene(_functionScene, e);
        }

        private void DrawScene(Scene scene, DrawEventArgs e)
        {
            if (_isSoftwareRendered) return;

            ProcessKeyboardInput(scene);
            scene.Draw();
            // invalidate to get a callback next frame
            e.InvalidateSurface();
        }


        private void DrawButton_OnClick(object sender, RoutedEventArgs e)
        {
           
            InterpolativeMathFunction mathFunction;
            SurfaceDimension uDim, vDim;
          
            try
            {
                uDim = new SurfaceDimension(
                    double.Parse(HermiteUMinTextBox.Text), 
                    double.Parse(HermiteUMaxTextBox.Text),
                    int.Parse(HermiteUCountTextBox.Text)
                    );

                vDim = new SurfaceDimension(
               double.Parse(HermiteVMinTextBox.Text),
                double.Parse(HermiteVMaxTextBox.Text),
                int.Parse(HermiteVCountTextBox.Text));
                mathFunction = InterpolativeMathFunction.FromExpression(MathExpressionTextBox.Text,"x","y");
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input.");
                return;
            }

            SplineFactory createSpline;
            KnotsGeneratorFactory createKnotsGenerator;

            if (_splineSubtractionClicked)
            {
                //createSpline = (x);
                var minuendGenerator = _minuendKnotsFactory(mathFunction);
                var subtrahendGenerator = ((KnotsGeneratorFactory) KnotsGeneratorComboBox.SelectedValue)(mathFunction);
                createKnotsGenerator = (mf)=>minuendGenerator-subtrahendGenerator;
                createSpline = (u,v,k,d)=>new BicubicHermiteSpline(u,v,k,d);
                //_splineSubtractionClicked = false;
            }
            else
            {
                createSpline = (SplineFactory)InterpolationTypeComboBox.SelectedValue;
                createKnotsGenerator = (KnotsGeneratorFactory)KnotsGeneratorComboBox.SelectedValue;
            }
     
            var shape = createSpline(uDim, vDim, createKnotsGenerator(mathFunction));
           // var fdshape = createSpline(uDim, vDim, createKnotsGenerator(mathFunction), Derivation.XY);
           // var sdshape = createSpline(uDim, vDim, createKnotsGenerator(mathFunction), Derivation.SecondXY);

            if (_splineSubtractionClicked)
            {
                shape.Name += "DIFF";
                DisableSplineChaining();
            }

            Color color;
            if (ColorCheckBox.IsChecked.HasValue && !ColorCheckBox.IsChecked.Value)
                color = ColorUtils.RandomShade((float)ColorSlider.Value);
            else
                color = _colorWheel.Next;
             //ShapesComboBox.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, color.R, color.G, color.B));
            shape.ColoredByShades(color);
            shape.DrawStyle = DrawStyle.Surface;
            shape.Name = MathExpressionTextBox.Text;
            var currentSelectedIndex = ShapesComboBox.SelectedIndex;
            _functionScene.Add(shape,false);
            ShapesComboBoxItems.Add(new ShapeComboBoxItem(shape.Name,color));
            ShapesComboBox.ItemsSource = null;
            ShapesComboBox.ItemsSource = ShapesComboBoxItems;
            ShapesComboBox.SelectedIndex = currentSelectedIndex;
            ShapesComboBox_OnSelectedItem(ShapesComboBox,null);

            if (ShapesComboBox.SelectedIndex < 0)
                ShapesComboBox.SelectedIndex = 0;

            AutoScale();

        }

        private void AutoScale()
        {
            var b = !AutoScaleCheckBox.IsChecked;
            if(b != null && (bool) b)
                return;
            
            var min = float.MaxValue;
            foreach (var shape in _functionScene)
            {
                var surface = shape as ISurface;
                if (surface == null) continue;
                var contestant = surface.MinHeight;
                min = contestant < min ? contestant : min;
            }

            var max = float.MinValue;
            foreach (var shape in _functionScene)
            {
                var surface = shape as ISurface;
                if (surface == null) continue;
                var contestant = surface.MaxHeight;
                max = contestant > max ? contestant : max;
            }
            float zScale = 1f;
            //var hdiff = Math.Abs(max - min);
            if (max > MaxDrawableHeight)// || min< MinDrawableHeight)
            {
                //var extremum = Math.Max(Math.Abs(min), Math.Abs(max));
                zScale = 1 / (max / MaxDrawableHeight);
            }
            if (Math.Abs(min) > Math.Abs(max) && min < MinDrawableHeight)
            {
                //var extremum = Math.Max(Math.Abs(min), Math.Abs(max));
                zScale = 1 / (max / MaxDrawableHeight);
            }
            ScenesAction(scene => scene.Scale = new Vector3(1, 1, zScale));

        }

        private void AutoScaleCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ZScaleSlider.IsEnabled = true;
            ScaleSlider_OnValueChanged(ZScaleSlider, new RoutedPropertyChangedEventArgs<double>(ZScaleSlider.Value, ZScaleSlider.Value));
        }

        private void AutoScaleCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            ZScaleSlider.IsEnabled = false;
        }
    }
}