using System.Windows.Input;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void DrawingSurface_OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
           
            _camera.Distance -= 0.02f*mouseWheelEventArgs.Delta;
            DebugBox.Text = _camera.Distance + ": " + _camera.VerticalAngle + ": " + _camera.HorizontalAngle;
            //DrawingSurface_OnMouseWheel();
        }
    }
}