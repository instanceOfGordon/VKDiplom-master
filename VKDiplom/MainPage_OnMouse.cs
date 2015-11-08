using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using VKDiplom.Engine;
using VKDiplom.Utilities;
using Keyboard = VKDiplom.Utilities.Keyboard;
using Point = System.Windows.Point;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnMouseLeftButtonDown(object sender,
            MouseButtonEventArgs mouseButtonEventArgs)
        {
            _previousMousePosition = mouseButtonEventArgs.GetPosition(null);

            _canDrag = true;
            (sender as UIElement).CaptureMouse();
        }

        private void FunctionDrawingSurface_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {

            _canDrag = false;
            (sender as UIElement).ReleaseMouseCapture();//CaptureMouse();
        }


        /////////////

        //private Point _downPosition;
        private void FirstDerivationDrawingSurface_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //_downPosition = e.GetPosition(null);
            //_previousMousePosition = _downPosition;
        }

        private void FirstDerivationDrawingSurface_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        ////////////


        private void SecondDerivationDrawingSurface_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void SecondDerivationDrawingSurface_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
        ////////////

        private void FunctionDrawingSurface_OnMouseRightButtonDown(object sender,
           MouseButtonEventArgs mouseButtonEventArgs)
        {
        }

        private void FunctionDrawingSurface_OnMouseRightButtonUp(object sender,
            MouseButtonEventArgs mouseButtonEventArgs)
        {
        }

        ///////////////////////

        private void FirstDerivationDrawingSurface_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void FirstDerivationDrawingSurface_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        /////////////////////

        private void SecondDerivationDrawingSurface_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void SecondDerivationDrawingSurface_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        ////////////
        //private Point _center;
        private void DrawingSurface_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_canDrag) return;

            var currentPosition = e.GetPosition(null);

            //var diffX = (float)(currentPosition.X - _previousMousePosition.X);
            //var diffY = (float)(currentPosition.Y - _previousMousePosition.Y);
            ////var moveVertical = diffY > diffX;
            ////DebugBox.Text = _camera.Distance + ": " + _camera.VerticalAngle + ": " + _camera.HorizontalAngle;
            //var vector = new Vector2((float)Math.Abs(currentPosition.X - _downPosition.X), (float)Math.Abs(currentPosition.Y - _downPosition.Y));
            //vector.Normalize();
            //var angle = VectorUtils.Angle(new Vector2((float)_downPosition.X,(float)_d), vector); //Math.Atan(Math.Abs(diffY / diffX));
            //                                                      //ShapeInfoTextBox.Text = angle.ToString();
            //                                                      //if (angle<MathHelper.PiOver4||angle>(MathHelper.TwoPi-MathHelper.PiOver4)||(angle>MathHelper.PiOver2+MathHelper.PiOver4&&angle< MathHelper.Pi+MathHelper.PiOver4))//Keyboard.IsKeyDown(Key.V))
            //if (angle < MathHelper.PiOver4 || angle > MathHelper.Pi - MathHelper.PiOver4)
            ////if(Math.Abs(diffY)>Math.Abs(diffX))
            //{
            //    ShapeInfoTextBox.Text = "HOR"+"  "+angle;
            //    var rotation = 0.001f * (diffX);
            //    _camera.HorizontalAngle += rotation;

            //}
            //else
            //{
            //    var rotation = -0.001f * diffY;
            //    ShapeInfoTextBox.Text = "VER"+"  " + angle;
            //    _camera.VerticalAngle += rotation;

            //    //            
            //}



            //            var diffX = (float) (currentPosition.X - _previousMousePosition.X);
            //            var diffY = (float)(currentPosition.Y - _previousMousePosition.Y);
            //            //var moveVertical = diffY > diffX;
            //            //DebugBox.Text = _camera.Distance + ": " + _camera.VerticalAngle + ": " + _camera.HorizontalAngle;
            //            var vector = new Vector2((float)(currentPosition.X-0.5*FunctionDrawingSurface.ActualWidth), (float)(currentPosition.Y-0.5*FunctionDrawingSurface.ActualHeight));
            //            //vector.Normalize();
            //            var angle = VectorUtils.Angle(Vector2.UnitY,vector); //Math.Atan(Math.Abs(diffY / diffX));
            //            //ShapeInfoTextBox.Text = angle.ToString();
            //            //if (angle<MathHelper.PiOver4||angle>(MathHelper.TwoPi-MathHelper.PiOver4)||(angle>MathHelper.PiOver2+MathHelper.PiOver4&&angle< MathHelper.Pi+MathHelper.PiOver4))//Keyboard.IsKeyDown(Key.V))
            //            if(angle<MathHelper.PiOver4||angle>MathHelper.Pi-MathHelper.PiOver4)
            //            //if(Math.Abs(diffY)>Math.Abs(diffX))
            //            {
            //                var rotation = -0.02f*diffY;
            //                ShapeInfoTextBox.Text = "VER";
            //                _camera.VerticalAngle += rotation;

            //            }
            //            else
            //            {
            //                ShapeInfoTextBox.Text = "HOR";
            //                var rotation=0.02f*(diffX);
            //                _camera.HorizontalAngle += rotation;
            ////            
            //            }

            if (Keyboard.IsKeyDown(Key.V))
            {
                var rotation = 0.02f * (float)(_previousMousePosition.Y - currentPosition.Y);

                _camera.VerticalAngle += rotation;

            }
            else
            {
                var rotation = 0.02f * (float)(currentPosition.X - _previousMousePosition.X);
                _camera.HorizontalAngle += rotation;
                //            
            }

            _previousMousePosition = currentPosition;
        }

    }
}