using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using VKDiplom.Engine;
using Keyboard = VKDiplom.Utilities.Keyboard;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnMouseLeftButtonDown(object sender,
            MouseButtonEventArgs mouseButtonEventArgs)
        {
            _mouseDownPosition = mouseButtonEventArgs.GetPosition(null);

            _canDrag = true;
            (sender as UIElement).CaptureMouse();
        }

        private void FunctionDrawingSurface_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {

            _canDrag = false;
            (sender as UIElement).ReleaseMouseCapture();//CaptureMouse();
        }


        /////////////

        private void FirstDerivationDrawingSurface_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        private void DrawingSurface_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_canDrag) return;

            var currentPosition = e.GetPosition(null);
            //DebugBox.Text = _camera.Distance + ": " + _camera.VerticalAngle + ": " + _camera.HorizontalAngle;
            if (Keyboard.IsKeyDown(Key.V))
            {
                var rotation = 0.02f*(float) (_mouseDownPosition.Y - currentPosition.Y);

                _camera.VerticalAngle += rotation;
               
            }
            else
            {
                var rotation=0.02f*(float) (currentPosition.X - _mouseDownPosition.X);
                _camera.HorizontalAngle += rotation;
//            
            }

            _mouseDownPosition = currentPosition;
        }

      }
}