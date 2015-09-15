#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Graphics;
using HermiteInterpolation;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.Shapes.SplineInterpolation.Bicubic;
using HermiteInterpolation.Shapes.SplineInterpolation.Biquartic;
using HermiteInterpolation.SplineKnots;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;

#endregion

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isSoftwareRendered)
            {
                var drawer = sender as DrawingSurface;
                drawer.Draw -= FirstDerivationDrawingSurface_OnDraw;
                drawer.Draw += (s, darg) => { };
            }
            LoadScene(sender as DrawingSurface, out _functionScene);

             //MexicanHatDemo(_functionScene, Derivation.Zero);
        }

        private void MexicanHatDemo(Scene scene, Derivation derivation)
        {
            if (scene == null) return;
//            Func<double, double, double> f = (x, y) => Math.Sin(Math.Sqrt(x*x + y*y));
//            Func<double, double, double> xd = (x, y) => (x*Math.Cos(Math.Sqrt(x*x + y*y)))
//                                                        /(Math.Sqrt(x*x + y*y));
//            Func<double, double, double> yd = (x, y) => (y*Math.Cos(Math.Sqrt(x*x + y*y)))
//                                                        /Math.Sqrt(x*x + y*y);
//            Func<double, double, double> xyd = (x, y) => -(y*x*Math.Sin(Math.Sqrt(x*x + y*y)))
//                                                         /(x*x + y*y)
//                                                         - (y*x*Math.Cos(Math.Sqrt(x*x + y*y)))
//                                                         /(Math.Pow(x*x + y*y, 1.5));


            //var aproximationFunction = new InterpolativeMathFunction(f, xd, yd, xyd);
            //var aproximationFunction = new InterpolativeMathFunction(f);
            var mathExpression = new MathExpression("sin(sqrt(x^2+y^2))", "x", "y");
            //var aproximationFunction = InterpolativeMathFunction.CompileFromString("x^4+y^4", "x", "y");
//            var shape = new SegmentSurface(new double[] {-3, -2, -1, 0, 1, 2, 3},
//                new double[] {-3, -2, -1, 0, 1, 2, 3},
            //  aproximationFunction, derivation);
            //var shape = new ClassicHermiteSurface(new double[] { -2, -1, 0, 1 }, new double[] { -2, -1, 0, 1 },
            // var shape = new HermiteShape(new double[] { -2, -1 }, new double[] { -2, -1 },
            var shape = new BiquarticHermiteSurface(new SurfaceDimension(-3, 3, 7), new SurfaceDimension(-3, 3, 7),
                //var shape = HermiteSurfaceFactoryHolder.CreateBiquartic(-3, 1, 7, -3, 1, 7,
                mathExpression, derivation);
            //shape.ColoredHeight();
            //shape.ColoredSimple(Color.FromNonPremultiplied(96,72,128,255));
            shape.ColoredByShades(_colorWheel.Next());
            //shape.DrawStyle = DrawStyle.Wireframe;
            scene.Add(shape);
        }

        private void FirstDerivationDrawingSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isSoftwareRendered)
            {
                var drawer = sender as DrawingSurface;
                drawer.Draw -= FirstDerivationDrawingSurface_OnDraw;
                drawer.Draw += (s, darg) => { };
            }
            LoadScene(sender as DrawingSurface, out _firstDerScene);
            //MexicanHatDemo(_firstDerScene, Derivation.First);
        }

        private void SecondDerivationDrawingSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isSoftwareRendered)
            {
                var drawer = sender as DrawingSurface;
                drawer.Draw -= FirstDerivationDrawingSurface_OnDraw;
                drawer.Draw += (s, darg) => { };
            }
            LoadScene(sender as DrawingSurface, out _secondDerScene);
            //MexicanHatDemo(_secondDerScene, Derivation.Second);
        }

       

        private void LoadScene(DrawingSurface drawingSurface, out Scene scene)
        {
            //if (VkDiplomGraphicsInitializationUtils.IsHardwareAccelerated()
            //{
             
            //    scene = null;
            //    return;
            //}
            scene = new Scene(_camera);


            //_functionScene.Camera.AspectRatio =1.0f;
            //GraphicsDeviceManager.Current.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise; 

            if (!_isSoftwareRendered)
                SetMultiSampleAntialiasing(drawingSurface, 8);

            if (!Application.Current.IsRunningOutOfBrowser)
            {
                HtmlPage.Plugin.Focus();
            }
        }

        private void LoadFromFile_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void SaveToFile_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void FileButton_OnClick(object sender, RoutedEventArgs e)
        {
            //ContextMenuService.GetContextMenu(FileMenuBarButton).IsOpen = true;
        }

        private void SettingsButton_OnCLick(object sender, RoutedEventArgs e)
        {
            //ContextMenuService.GetContextMenu(SettingsButton).IsOpen = true;
            var child = new SettingsWindow(this);
            //child.SetWindow(Window.GetWindow(this));
            child.Show();
        }

        private void HelpButton_OnCLick(object sender, RoutedEventArgs e)
        {
        }

        private void InittializeComboBoxes()
        {
            _hermiteChoices = new Dictionary<string, SplineFactory>
            {
                {
                    "Direct function",
                    (uDimension, vDimension, knotsGenerator, derivation) =>
                        new MathFunctionSurface(uDimension, vDimension,new MathExpression(MathExpressionTextBox.Text,XVariableTextBox.Text,YVariableTextBox.Text))
                },
                {
                    "Bicubic",
                    (uDimension, vDimension, knotsGenerator, derivation) =>
                        new BicubicHermiteSurface(uDimension, vDimension,knotsGenerator, derivation)                    
                },
                {
                    "Biquartic",
                   (uDimension, vDimension, knotsGenerator, derivation) =>
                        new BiquarticHermiteSurface(uDimension, vDimension,knotsGenerator, derivation)       
                }             
            };
            _knotsChoices = new Dictionary<string, KnotsGeneratorFactory>
            {
                {
                    "Direct",
                    function => new DirectKnotsGenerator(function)
                },
                {
                    "De Boor",
                    function => new DeBoorKnotsGenerator(function)
                },
                {
                    "Reduced de Boor",
                    function => new ReducedDeBoorKnotsGenerator(function)
                }
            };

            InterpolationTypeComboBox.ItemsSource = _hermiteChoices;
            InterpolationTypeComboBox.DisplayMemberPath = "Key";
            InterpolationTypeComboBox.SelectedValuePath = "Value";
            InterpolationTypeComboBox.SelectedIndex = 0;

            KnotsGeneratorComboBox.ItemsSource = _knotsChoices;
            KnotsGeneratorComboBox.DisplayMemberPath = "Key";
            KnotsGeneratorComboBox.SelectedValuePath = "Value";
            KnotsGeneratorComboBox.SelectedIndex = 0;

            SplinesComboBox.ItemsSource = _functionScene;
            SplinesComboBox.DisplayMemberPath = "Name";
            //var item = new ComboBoxItem();
            //item.Content = "Name";
            //item.Foreground = 
            //SplinesComboBox.ItemTemplate.DataType = new ComboBoxItem();

        }

        private void SplineComboBox_OnSelectedItem(object sender, SelectionChangedEventArgs e)
        {
            
            var splineSelector = sender as ComboBox;
            if (splineSelector == null) return;
            if (splineSelector.ItemsSource==null) return;
            ScenesAction(scene=> scene.HighlightedShapeIndex=splineSelector.SelectedIndex);
        }
    }
}