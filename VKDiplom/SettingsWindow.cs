using System;
using System.Windows;
using System.Windows.Controls;
using VKDiplom.Engine;

namespace VKDiplom
{
    public class SettingsWindow : ChildWindow
    {
        private readonly Grid _grid;
        private ComboBox _lightingChooser;
        private ComboBox _msaaChooser;
        private Button _okButton;
        private MainPage _page;

        public SettingsWindow(MainPage page)
        {
            _page = page;
            _grid = new Grid();
            Title = "Settings";
            InitializeFields();
        }


        private void InitializeFields()
        {
            Content = _grid;
            //_grid.Margin = new Thickness(5);
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(80)});
            _grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(120)});

            var msaaLabel = new Label {Content = "Antialiasing: "};
            Grid.SetRow(msaaLabel, 0);
            Grid.SetColumn(msaaLabel, 0);
            _grid.Children.Add(msaaLabel);
            var lightingLabel = new Label {Content = "Quality: "};
            Grid.SetRow(lightingLabel, 1);
            Grid.SetColumn(lightingLabel, 0);
            _grid.Children.Add(lightingLabel);

            _msaaChooser = new ComboBox {DataContext = new[] {0, 2, 4, 8}, Margin = new Thickness(5)};
            Grid.SetRow(_msaaChooser, 0);
            Grid.SetColumn(_msaaChooser, 1);
            _grid.Children.Add(_msaaChooser);

            _lightingChooser = new ComboBox
            {
                DataContext = Enum.GetNames(typeof (Scene.LightingQuality)),
                Margin = new Thickness(5)
            };
            Grid.SetRow(_lightingChooser, 1);
            Grid.SetColumn(_lightingChooser, 1);
            _grid.Children.Add(_lightingChooser);

            _okButton = new Button {Content = "Ok", Margin = new Thickness(5)};

            _okButton.Click += OButton_Click;
            Grid.SetRow(_okButton, 2);
            Grid.SetColumn(_okButton, 1);
            _grid.Children.Add(_okButton);
        }

        private void OButton_Click(object sender, RoutedEventArgs args)
        {
            Close();
        }
    }
}