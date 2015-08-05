using System;
using System.Windows;
using System.Windows.Controls;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.HermiteSpline;
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
            double umin, umax, vmin, vmax;
            int ucount, vcount;
            try
            {
                umin = double.Parse(HermiteUMinTextBox.Text);
                umax = double.Parse(HermiteUMaxTextBox.Text);
                ucount = int.Parse(HermiteUCountTextBox.Text);
                vmin = double.Parse(HermiteVMinTextBox.Text);
                vmax = double.Parse(HermiteVMaxTextBox.Text);
                vcount = int.Parse(HermiteVCountTextBox.Text);
                function = InterpolatedFunction.FromString(FunctionExpressionTextBox.Text, XVariableTextBox.Text,
                    YVariableTextBox.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid input.");
                return;
            }
            HermiteSurface shape, fdshape, sdshape;
            var selectedItem = (HermiteType) HermiteTypeComboBox.SelectedIndex;
            switch (selectedItem)
            {
                case HermiteType.Bicubic:
                    shape = HermiteSurfaceFactory.CreateBicubic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function);
                    fdshape = HermiteSurfaceFactory.CreateBicubic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function, Derivation.First);
                    sdshape = HermiteSurfaceFactory.CreateBicubic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function, Derivation.Second);
                    break;

                case HermiteType.Biquartic:
                    shape = HermiteSurfaceFactory.CreateBiquartic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function);
                    fdshape = HermiteSurfaceFactory.CreateBiquartic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function, Derivation.First);
                    sdshape = HermiteSurfaceFactory.CreateBiquartic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function, Derivation.Second);
                    break;
                default:
                    shape = HermiteSurfaceFactory.CreateBicubic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function);
                    fdshape = HermiteSurfaceFactory.CreateBicubic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function, Derivation.First);
                    sdshape = HermiteSurfaceFactory.CreateBicubic(umin, umax, ucount, vmin,
                        vmax, vcount,
                        function, Derivation.Second);
                    break;
            }
            shape.ColoredBySegment();
            shape.DrawStyle = DrawStyle.Surface;
            _functionScene.Add(shape);

            fdshape.ColoredBySegment();
            fdshape.DrawStyle = DrawStyle.Surface;
            _firstDerScene.Add(fdshape);
            sdshape.ColoredBySegment();
            sdshape.DrawStyle = DrawStyle.Surface;
            _secondDerScene.Add(sdshape);
        }

        private void DrawHermiteSurface(Scene scene, HermiteSurface surface)
        {
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
            scene.Add(surface);
        }
    }
}