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
        private Scene _firstDerScene;
        private Derivation _focusedDrawingSurface = Derivation.Zero;
        private Scene _functionScene; //= new Scene();
        private Dictionary<string, SplineFactory> _hermiteChoices;
        private Dictionary<string, KnotsGeneratorFactory> _knotsChoices;
        private Point _previousMousePosition;
        private Scene _secondDerScene;
        private readonly bool _isSoftwareRendered = false;
        //private readonly double _scaleTresholdToDefault;

        public MainPage()
        {
            _isSoftwareRendered = GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware;
              
            if (_isSoftwareRendered)
                VkDiplomGraphicsInitializationUtils.ShowReport();

            InitializeComponent();
            InittializeComboBoxes();
            
        }

        private void ScenesAction(Action<Scene> action)
        {
            action(_functionScene);
            action(_firstDerScene);
            action(_secondDerScene);
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

        private void FocusDrawingSurface_OnClick(object sender, MouseEventArgs e)
        {
            var icon = sender as Image;
            if (icon == null) return;
            if (icon.Equals(FDxDyIcon))
            {
                FocusFunctionDrawingSurface();
            }
            else if (icon.Equals(DxFDyIcon))
            {             
                FocusFirstDerivationDrawingSurface();
            }
            else if (icon.Equals(DyDxFIcon))
            {
                FocusSecondDerivation();
            }
               
            // MessageBox.Show("Aleluja");
        }

        // FDxDy
        private void FocusFunctionDrawingSurface()
        {
            FxBackground.Background = PurpleBrush;
            DFxBackground.Background = TransparentBrush;
            DDFxBackground.Background = TransparentBrush;

            switch (_focusedDrawingSurface)
            {
                case Derivation.First: // DxFDy
                    FunctionDrawingSurface.Draw -= FirstDerivationDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw -= FunctionDrawingSurface_OnDraw;

                    FunctionDrawingSurface.Draw += FunctionDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw += FirstDerivationDrawingSurface_OnDraw;
                    break;
                case Derivation.Second: // DyDxF
                    FunctionDrawingSurface.Draw -= SecondDerivationDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw -= FunctionDrawingSurface_OnDraw;

                    FunctionDrawingSurface.Draw += FunctionDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw += SecondDerivationDrawingSurface_OnDraw;
                    break;
            }

            _focusedDrawingSurface = Derivation.Zero;
        }

        // DxFDy
        private void FocusFirstDerivationDrawingSurface()
        {
            FxBackground.Background = TransparentBrush;
            DFxBackground.Background = PurpleBrush;
            DDFxBackground.Background = TransparentBrush;

            switch (_focusedDrawingSurface)
            {
                case Derivation.Zero: // FDxDy
                    FunctionDrawingSurface.Draw -= FunctionDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw -= FirstDerivationDrawingSurface_OnDraw;

                    FunctionDrawingSurface.Draw += FirstDerivationDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw += FunctionDrawingSurface_OnDraw;
                    break;
                case Derivation.Second: //DyDxF
                    FunctionDrawingSurface.Draw -= SecondDerivationDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw -= FirstDerivationDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw -= FunctionDrawingSurface_OnDraw;

                    FunctionDrawingSurface.Draw += FirstDerivationDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw += FunctionDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw += SecondDerivationDrawingSurface_OnDraw;
                    break;
            }

            _focusedDrawingSurface = Derivation.First;
        }

        // DyDxF
        private void FocusSecondDerivation()
        {
            FxBackground.Background = TransparentBrush;
            DFxBackground.Background = TransparentBrush;
            DDFxBackground.Background = PurpleBrush;

            switch (_focusedDrawingSurface)
            {
                case Derivation.Zero: // FDxDy
                    FunctionDrawingSurface.Draw -= FunctionDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw -= SecondDerivationDrawingSurface_OnDraw;

                    FunctionDrawingSurface.Draw += SecondDerivationDrawingSurface_OnDraw; 
                    SecondDerivationDrawingSurface.Draw += FunctionDrawingSurface_OnDraw;
                    break;
                case Derivation.First: // DxFDy
                    FunctionDrawingSurface.Draw -= FirstDerivationDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw -= FunctionDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw -= SecondDerivationDrawingSurface_OnDraw;

                    FunctionDrawingSurface.Draw += SecondDerivationDrawingSurface_OnDraw;
                    FirstDerivationDrawingSurface.Draw += FirstDerivationDrawingSurface_OnDraw;
                    SecondDerivationDrawingSurface.Draw += FunctionDrawingSurface_OnDraw;
                    break;
            }

            _focusedDrawingSurface = Derivation.Second;
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