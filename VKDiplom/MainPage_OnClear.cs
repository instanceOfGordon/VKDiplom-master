using System;
using System.Windows;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _colorWheel.Reset();
            ScenesAction(scene=> scene.Clear());
            ShapesComboBox.ItemsSource = _functionScene;
        }

        private void DeleteShapeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var selIdx = ShapesComboBox.SelectedIndex;
            ScenesAction(scene=>scene.RemoveAt(selIdx));
            ShapesComboBox.SelectedIndex = Math.Max(0,selIdx-1);
            ShapesComboBox.ItemsSource = _functionScene;
        }
    }
}