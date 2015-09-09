using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.HermiteSpline;
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
        //GraphicsDevice _graphicsDevice;
        //private SceneTime _sceneTime;
        private readonly RotateCamera _camera = new RotateCamera();
        private readonly Color _color = DefaultColor;
        private readonly ColorWheel _colors = new ColorWheel();
        private readonly SolidColorBrush _disabledColorBrush = new SolidColorBrush(Colors.DarkGray);
        private readonly DrawStyle _drawStyle = DrawStyle.Surface;
        //private readonly InterpolatedFunction _aproximationFunction = new InterpolatedFunction();

        private readonly TextureStyle _textureStyle = TextureStyle.HeightColored;
        private bool _canDrag;
        // Scenes
        private Scene _firstDerScene;
        private Derivation _focusedDrawingSurface = Derivation.Zero;
        private Scene _functionScene; //= new Scene();
        private Dictionary<string, HermiteSurfaceFactory> _hermiteChoices;
        private Dictionary<string, KnotsGeneratorFactory> _knotsChoices;
        private Point _mouseDownPosition;
        private Scene _secondDerScene;

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
            FdxdyBackground.Background = PurpleBrush;
            DxfdyBackground.Background = TransparentBrush;
            DydxfBackground.Background = TransparentBrush;

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
            FdxdyBackground.Background = TransparentBrush;
            DxfdyBackground.Background = PurpleBrush;
            DydxfBackground.Background = TransparentBrush;

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
            FdxdyBackground.Background = TransparentBrush;
            DxfdyBackground.Background = TransparentBrush;
            DydxfBackground.Background = PurpleBrush;

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
    }
}