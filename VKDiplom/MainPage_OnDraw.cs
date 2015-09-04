using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.HermiteSpline;
using HermiteInterpolation.Shapes.HermiteSpline.Bicubic;
using HermiteInterpolation.Shapes.HermiteSpline.Biquartic;
using HermiteInterpolation.SplineKnots;
using VKDiplom.Engine;

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
            if (scene == null) return;

            ProcessKeyboardInput(scene);
            //_functionScene.Camera.SetViewToZero(new Vector3(State.SliderYawVal, State.SliderPitchVal, State.SliderRollVal));
            scene.Draw();

            // invalidate to get a callback next frame
            e.InvalidateSurface();
        }


        private void DrawButton_OnClick(object sender, RoutedEventArgs e)
        {
            InterpolatedFunction function;
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
                function = InterpolatedFunction.FromString(FunctionExpressionTextBox.Text, XVariableTextBox.Text,
                    YVariableTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input.");
                return;
            }
            //Spline shape, fdshape, sdshape;
            //var selectedItem = (HermiteType) HermiteTypeComboBox.SelectedIndex;
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
            var createSpline = (HermiteSurfaceFactory) HermiteTypeComboBox.SelectedValue;
            var createKnotsGenerator = (KnotsGeneratorFactory) KnotsGeneratorComboBox.SelectedValue;
            var shape = createSpline(uDim, vDim, createKnotsGenerator(function));
            var fdshape = createSpline(uDim, vDim, createKnotsGenerator(function), Derivation.First);
            var sdshape = createSpline(uDim, vDim, createKnotsGenerator(function), Derivation.Second);

            var color = _colors.Next();
            shape.ColoredByShades(color);
            shape.DrawStyle = DrawStyle.Surface;
            _functionScene.Shapes.Add(shape);

            fdshape.ColoredByShades(color);
            fdshape.DrawStyle = DrawStyle.Surface;
            _firstDerScene.Shapes.Add(fdshape);
            sdshape.ColoredByShades(color);
            sdshape.DrawStyle = DrawStyle.Surface;
            _secondDerScene.Shapes.Add(sdshape);
        }

        private void DrawHermiteSurface(Scene scene, CompositeSurface surface)
        {
            new Thread(() => { });

            surface.ColoredHeight();
            surface.DrawStyle = _drawStyle;
            switch (_textureStyle)
            {
                case TextureStyle.SingleColor:
                    surface.ColoredSimple(_color);
                    break;
                case TextureStyle.HeightColored:
                    surface.ColoredHeight();
                    break;
            }
            scene.Shapes.Add(surface);
        }
    }
}