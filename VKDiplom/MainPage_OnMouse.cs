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
            rotImage.Opacity = 0.8;
        }

        private void RotatorImage_OnMouseLeave(object sender, MouseEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            rotImage.Opacity = 0.6;
        }

        private void RotatorImage_OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            _previousMousePosition = e.GetPosition(null);
            rotImage.Opacity = 0.95;
            _canDrag = true;
            rotImage.CaptureMouse();
        }

        private void RotatorImage_OnLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rotImage = sender as Image;
            if (rotImage == null) return;
            rotImage.Opacity = 0.6;
            _canDrag = false;
            rotImage.ReleaseMouseCapture();
        }

        private void HorizontalRotatorImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_canDrag) return;
            var currentPosition = e.GetPosition(null);
            var rotation = 0.02f * (float)(_previousMousePosition.X - currentPosition.X);
            _camera.HorizontalAngle += rotation;
        }

        private void VerticalRotatorImage_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_canDrag) return;
            var currentPosition = e.GetPosition(null);
            var rotation = 0.02f * (float)(_previousMousePosition.Y - currentPosition.Y);
            _camera.VerticalAngle += rotation;
        }
    }
}