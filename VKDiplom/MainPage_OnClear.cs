using System.Windows;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _functionScene.Shapes.Clear();
            _firstDerScene.Shapes.Clear();
            _secondDerScene.Shapes.Clear();
        }
    }
}