using System;
using System.Windows;

namespace VKDiplom
{
    public partial class MainPage
    {
        private void ClearButton_OnClick(object sender, RoutedEventArgs e)
        {
            _colorWheel.Reset();
            ShapesComboBox.SelectedIndex = -1;
            ScenesAction(scene=> scene.Clear());
            ShapesComboBox.ItemsSource = _functionScene;
            
        }

        private void DeleteShapeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var count = _functionScene.Count;
            var selIdx = ShapesComboBox.SelectedIndex;
            if (selIdx >= count || selIdx < 0) return;
            if (selIdx == 0 && count > 1)
                ShapesComboBox.SelectedIndex = 0;
            else if (selIdx == 0 && count == 1)
                ShapesComboBox.SelectedIndex = -1;
            else
                ShapesComboBox.SelectedIndex = selIdx-1;
            ScenesAction(scene=>scene.RemoveAt(selIdx));
            ShapesComboBox.ItemsSource = null;        
            ShapesComboBox.ItemsSource = _functionScene;
            //MessageBox.Show("STOP");

        }
    }
}