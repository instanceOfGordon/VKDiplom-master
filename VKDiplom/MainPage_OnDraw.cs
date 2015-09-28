using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using HermiteInterpolation;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnDraw(object sender, DrawEventArgs e)
        {
            DrawScene(_functionScene, e);
        }

        private void SecondDerivationDrawingSurface_OnDraw(object sender, DrawEventArgs e)
        {
            DrawScene(_firstDerScene, e);
        }

        private void FirstDerivationDrawingSurface_OnDraw(object sender, DrawEventArgs e)
        {
            DrawScene(_secondDerScene, e);
        }


        private void DrawScene(Scene scene, DrawEventArgs e)
        {
            //ProcessKeyboardInput();                              
            // _functionScene.SetupView(_graphicsDevice);
            if (_isSoftwareRendered) return;

            ProcessKeyboardInput(scene);
            //_functionScene.Camera.SetViewToZero(new Vector3(State.SliderYawVal, State.SliderPitchVal, State.SliderRollVal));
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
                mathFunction = InterpolativeMathFunction.FromMathExpression(MathExpressionTextBox.Text,"x","y");
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input.");
                return;
            }
            //SplineSurface shape, fdshape, sdshape;
            //var selectedItem = (HermiteType) InterpolationTypeComboBox.SelectedIndex;
            //switch (selectedItem)
            //{
            //    case HermiteType.Bicubic:
            //        shape = new BicubicHermiteSurface(uDim, vDim,
            //            function);
            //        fdshape = new BicubicHermiteSurface(uDim, vDim,
            //            function, Derivation.First);
            //        sdshape = new BicubicHermiteSurface(uDim, vDim,
            //            function, Derivation.Second);
            //        break;

            //    case HermiteType.Biquartic:
            //        shape = new BiquarticHermiteSurface(uDim, vDim,
            //            function);
            //        fdshape = new BiquarticHermiteSurface(uDim, vDim,
            //            function, Derivation.First);
            //        sdshape = new BiquarticHermiteSurface(uDim, vDim,
            //            function, Derivation.Second);
            //        break;
            //    default:
            //        shape = new BicubicHermiteSurface(uDim, vDim,
            //            function);
            //        fdshape = new BicubicHermiteSurface(uDim, vDim,
            //            function, Derivation.First);
            //        sdshape = new BicubicHermiteSurface(uDim, vDim,
            //            function, Derivation.Second);
            //        break;
            //}

            var startTime = DateTime.Now;
            
            var createSpline = (SplineFactory) InterpolationTypeComboBox.SelectedValue;
            var createKnotsGenerator = (KnotsGeneratorFactory) KnotsGeneratorComboBox.SelectedValue;
            var shape = createSpline(uDim, vDim, createKnotsGenerator(mathFunction));
            var fdshape = createSpline(uDim, vDim, createKnotsGenerator(mathFunction), Derivation.First);
            var sdshape = createSpline(uDim, vDim, createKnotsGenerator(mathFunction), Derivation.Second);

            var color = _colorWheel.Next;
            shape.ColoredByShades(color);
            shape.DrawStyle = DrawStyle.Surface;
            shape.Name = MathExpressionTextBox.Text;

            _functionScene.Add(shape,false);

            //if (ShapesComboBox.SelectedIndex == _functionScene.Count - 2)
            //{
            ShapesComboBox.ItemsSource = null;
            ShapesComboBox.ItemsSource = _functionScene;
           // ShapesComboBox.SelectedIndex = _functionScene.HighlightedShapeIndex;
            //    ++ShapesComboBox.SelectedIndex;

            //}       

            fdshape.ColoredByShades(color);
            fdshape.DrawStyle = DrawStyle.Surface;
            _firstDerScene.Add(fdshape,false);
            sdshape.ColoredByShades(color);
            sdshape.DrawStyle = DrawStyle.Surface;
            _secondDerScene.Add(sdshape,false);
            var endTime = DateTime.Now;
            SetCalcLabelContent((endTime - startTime).Milliseconds);
            if (ShapesComboBox.SelectedIndex < 0)
                ShapesComboBox.SelectedIndex = 0;

        }

        private void SetCalcLabelContent(double time)
        {
            CalcTimeLabel.Content = "Function rendered in: " + $"{time:0.00}" + " ms";
        }

        //private void DrawHermiteSurface(Scene scene, CompositeSurface surface)
        //{
        //    new Thread(() => { });

        //    surface.ColoredHeight();
        //    surface.DrawStyle = _drawStyle;
        //    switch (_textureStyle)
        //    {
        //        case TextureStyle.SingleColor:
        //            surface.ColoredSimple(_color);
        //            break;
        //        case TextureStyle.HeightColored:
        //            surface.ColoredHeight();
        //            break;
        //    }
        //    scene.Add(surface);
        //}
    }
}