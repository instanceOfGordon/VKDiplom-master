using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VKDiplom.Engine;
using Keyboard = VKDiplom.Utilities.Keyboard;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void MainPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.SetKeyDown(e.Key, true);
            //if(e.Key==Key)
            //switch (e.Key)
            //{
            //    case Key.Add:
            //    case Key.P:
            //        _camera.Distance -= ZoomFactor;
            //        break;
            //    case Key.Subtract:
            //    case Key.O:
            //        _camera.Distance -= ZoomFactor;
            //        break;
            //    case Key.Up:
            //        _camera.VerticalAngle += RotationFactor;
            //        break;
            //    case Key.Down:
            //        _camera.VerticalAngle -= RotationFactor;
            //        break;
            //    case Key.Right:
            //        _camera.HorizontalAngle += RotationFactor;
            //        break;
            //    case Key.Left:
            //        _camera.HorizontalAngle -= RotationFactor;
            //        break;
            //}
            //DebugBox.Text = _camera.Distance + ": " + _camera.VerticalAngle + ": " + _camera.HorizontalAngle;
        }

        private void MainPage_OnKeyUp(object sender, KeyEventArgs e)
        {
            Keyboard.SetKeyDown(e.Key, false);
        }

        private void ProcessKeyboardInput(Scene scene)
        {
            #region Zoom

            if (Keyboard.IsKeyDown(Key.Add) || Keyboard.IsKeyDown(Key.P))

                if (Keyboard.IsKeyDown(Key.Subtract) || Keyboard.IsKeyDown(Key.O))
                    _camera.Distance += ZoomFactor;

            #endregion Zoom

            #region Rotation

            if (Keyboard.IsKeyDown(Key.Up))
                _camera.VerticalAngle += RotationFactor;

            if (Keyboard.IsKeyDown(Key.Down))
                _camera.VerticalAngle -= RotationFactor;

            if (Keyboard.IsKeyDown(Key.Right))
                _camera.HorizontalAngle += RotationFactor;


            if (Keyboard.IsKeyDown(Key.Left))
                _camera.HorizontalAngle -= RotationFactor;

            #endregion Rotation

            //if (DebugBox != null)
            //    DebugBox.Text = _camera.Distance + ": " + _camera.VerticalAngle + ": " + _camera.HorizontalAngle;
        }


    }
}