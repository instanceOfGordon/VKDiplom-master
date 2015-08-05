using System.Windows;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _functionScene.Clear();
            _firstDerScene.Clear();
            _secondDerScene.Clear();
        }
    }
}