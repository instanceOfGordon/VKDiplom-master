using System.Windows;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            ScenesAction(scene=> scene.Clear());

        }
    }
}