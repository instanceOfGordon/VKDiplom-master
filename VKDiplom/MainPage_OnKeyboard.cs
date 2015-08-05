using System.Windows.Input;
using Microsoft.Xna.Framework;
using VKDiplom.Engine;
using Keyboard = VKDiplom.Utils.Keyboard;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void MainPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            Keyboard.SetKeyDown(e.Key, true);
        }
        
        private void MainPage_OnKeyUp(object sender, KeyEventArgs e)
        {
            Keyboard.SetKeyDown(e.Key, false);
        }

        private void ProcessKeyboardInput(Scene scene)
        {
            #region Zoom
            if (Keyboard.IsKeyDown(Key.Add) || Keyboard.IsKeyDown(Key.P))
                _camera.Distance -= ZoomFactor;

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
        }
    }
}