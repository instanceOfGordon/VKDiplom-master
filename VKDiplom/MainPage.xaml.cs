using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows.Input;
using System.Windows.Media;
using HermiteInterpolation;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;
using VKDiplom.Utilities;
using Color = Microsoft.Xna.Framework.Color;
using UIColor = System.Windows.Media.Color;

namespace VKDiplom
{
    // If this code works, it is written by Viliam Kačala.
    // If it does not work, I don't know who wrote it.
    public partial class MainPage
    {
        private const float RotationFactor = .01f;
        private const float ZoomFactor = .1f;
        // Distance

        private static readonly Color DefaultColor = Color.FromNonPremultiplied(128, 128, 128, 208);
        private static readonly SolidColorBrush TransparentBrush = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush PurpleBrush = new SolidColorBrush(Colors.Purple);
        private readonly RotateCamera _camera = new RotateCamera();
        private readonly Color _color = DefaultColor;
        private readonly ColorWheel _colorWheel = new ColorWheel();
        private readonly SolidColorBrush _disabledColorBrush = new SolidColorBrush(Colors.DarkGray);
        private readonly DrawStyle _drawStyle = DrawStyle.Surface;
        private readonly TextureStyle _textureStyle = TextureStyle.HeightColored;
        private bool _isLeftMouseButtonDown;
        // Scenes
        private Derivation _focusedDrawingSurface = Derivation.Zero;
        private Scene _functionScene; //= new Scene();
        private Dictionary<string, SplineFactory> _hermiteChoices;
        private Dictionary<string, KnotsGeneratorFactory> _knotsChoices;
        private Point _previousMousePosition;

        private readonly bool _isSoftwareRendered = false;
        public List<ShapeComboBoxItem> ShapesComboBoxItems { get; } 
        //private readonly double _scaleTresholdToDefault;

        public MainPage()
        {
            ShapesComboBoxItems = new List<ShapeComboBoxItem>();
            _isSoftwareRendered = GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware;
              
            if (_isSoftwareRendered)
                VkDiplomGraphicsInitializationUtils.ShowReport();

            InitializeComponent();
            InittializeComboBoxes();
            
        }

        private void ScenesAction(Action<Scene> action)
        {
            action(_functionScene);

        }

        private void ColorCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox ==null ||ColorSlider == null) return;
            if (checkBox.IsChecked.HasValue) ColorSlider.IsEnabled = false;         
        }


        private void ColorCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null || ColorSlider == null) return;
            if (checkBox.IsChecked.HasValue) ColorSlider.IsEnabled = true;       
        }

        private void InterpolationTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var iBox = sender as ComboBox;
            if (iBox == null) return;
            //var selValue = (KeyValuePair<string, SplineFactory>) iBox.;//Dictionary<>
            KnotsGeneratorComboBox.IsEnabled = iBox.SelectedIndex != 0;
        }

      
    }
}