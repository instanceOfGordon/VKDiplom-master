using System;
using System.Windows;
using System.Windows.Controls;
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
        private void RotatorImage_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            if(!_isLeftMouseButtonDown)
                rotImage.Opacity = 0.8;
        }

        private void RotatorImage_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            if (!_isLeftMouseButtonDown)
                rotImage.Opacity = 0.6;
        }

        private void RotatorImage_OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            rotImage.Opacity = 0.95;
            _isLeftMouseButtonDown = true;
            _previousMousePosition = e.GetPosition(null);
            rotImage.CaptureMouse();
        }

        private void RotatorImage_OnLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            rotImage.Opacity = 0.6;
            _isLeftMouseButtonDown = false;
            rotImage.ReleaseMouseCapture();
        }

        private void HorizontalRotatorImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isLeftMouseButtonDown) return;
            var currentPosition = e.GetPosition(null);
            var rotation = 0.005f * (float)(_previousMousePosition.X - currentPosition.X);
            _camera.HorizontalAngle -= rotation;
            _previousMousePosition = e.GetPosition(null);

        }

        private void VerticalRotatorImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isLeftMouseButtonDown) return;
            var currentPosition = e.GetPosition(null);
            var rotation = 0.005f * (float)(_previousMousePosition.Y - currentPosition.Y);
            _camera.VerticalAngle += rotation;
            _previousMousePosition = e.GetPosition(null);
        }
    }
}