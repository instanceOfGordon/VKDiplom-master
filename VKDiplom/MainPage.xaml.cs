using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.HermiteSpline;
using HermiteInterpolation.SplineKnots;
using HermiteInterpolation.Utils;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;
using VKDiplom.Utilities;
using Color = Microsoft.Xna.Framework.Color;
using Point = System.Windows.Point;
using UIColor = System.Windows.Media.Color;

namespace VKDiplom
{
    // If this code works, it is written by Viliam Kačala.
    // If it does not work, I don't know who wrote it.
    public partial class MainPage
    {
        private const float RotationFactor = .01f;
        private const float ZoomFactor = .1f;
        //GraphicsDevice _graphicsDevice;
        //private SceneTime _sceneTime;
        private readonly RotateCamera _camera = new RotateCamera();
        // Distance

        private static readonly Color DefaultColor = Color.FromNonPremultiplied(128, 128, 128, 208);
        private readonly Color _color = DefaultColor;
        private bool _canDrag;
        private DrawStyle _drawStyle = DrawStyle.Surface;

        // Scenes
        private Scene _firstDerScene;
        private Scene _functionScene; //= new Scene();
        private Scene _secondDerScene;
        private Point _mouseDownPosition;

        private readonly ColorWheel _colors = new ColorWheel();

        private Dictionary<string, HermiteSurfaceFactory> _hermiteChoices;
        private Dictionary<string, KnotsGeneratorFactory> _knotsChoices;


        //private readonly InterpolatedFunction _aproximationFunction = new InterpolatedFunction();

        private TextureStyle _textureStyle = TextureStyle.HeightColored;

        public MainPage()
        {
            if (VkDiplomGraphicsInitializationUtils.Is3DBlocked())
                VkDiplomGraphicsInitializationUtils.ShowReport();
                         
            InitializeComponent();
            InittializeComboBoxes();

            // var 
        }

        private void ScenesAction(Action<Scene> action)
        {
            action(_functionScene);
            action(_firstDerScene);
            action(_secondDerScene);
        }


        private void ColorSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //ScenesAction(scene => scene.Shapes.La);
        }

        private readonly SolidColorBrush _disabledColorBrush = new SolidColorBrush(Colors.DarkGray);

        private void ColorCheckBox_OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null) return;
            ColorSlider.IsEnabled = checkBox.IsEnabled;
            //if (checkBox.IsEnabled)
            //{
            //    ColorStackPanel.Background = _disabledColorBrush;
            //}
            //else
            //{
            //    ColorUtils.
            //    ColorStackPanel.Background = new SolidColorBrush(new UIColor());
            //}
        }
    }
}