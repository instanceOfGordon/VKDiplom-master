using System.Windows.Input;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void DrawingSurface_OnMouseWheel(object sender, MouseWheelEventArgs mouseWheelEventArgs)
        {
            _camera.Distance -= 0.02f*mouseWheelEventArgs.Delta;
            //DrawingSurface_OnMouseWheel();
        }
    }
}