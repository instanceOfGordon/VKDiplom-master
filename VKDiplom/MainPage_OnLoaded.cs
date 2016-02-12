#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Graphics;
using HermiteInterpolation;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Numerics.MathFunctions;
using HermiteInterpolation.Shapes;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;
using VKDiplom.Utilities;

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
            //var aproximationFunction = new InterpolativeMathFunction(f, xd, yd, xyd);
            //var aproximationFunction = new InterpolativeMathFunction(f);
            var mathExpression = MathExpression.CreateDefault("sin(sqrt(x^2+y^2))", "x", "y");
            //var aproximationFunction = InterpolativeMathFunction.FromExpression("x^4+y^4", "x", "y");
//            var shape = new SegmentSurface(new double[] {-3, -2, -1, 0, 1, 2, 3},
//                new double[] {-3, -2, -1, 0, 1, 2, 3},
            //  aproximationFunction, derivation);
            //var shape = new ClassicHermiteSurface(new double[] { -2, -1, 0, 1 }, new double[] { -2, -1, 0, 1 },
            // var shape = new HermiteShape(new double[] { -2, -1 }, new double[] { -2, -1 },
            var shape = new BiquarticHermiteSpline(new SurfaceDimension(-3, 3, 7), new SurfaceDimension(-3, 3, 7),
                //var shape = HermiteSurfaceFactoryHolder.CreateBiquartic(-3, 1, 7, -3, 1, 7,
                mathExpression, derivation);
            //shape.ColoredHeight();
            //shape.ColoredSimple(Color.FromNonPremultiplied(96,72,128,255));
            shape.ColoredByShades(_colorWheel.Next);
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
            scene = new Scene(_camera);
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
            double umin = double.NaN,
                umax = double.NaN, 
                vmin = double.NaN, 
                vmax = double.NaN;
            int ucount = int.MinValue,
                vcount = int.MinValue;

            foreach (var drawable in _functionScene)
            {
                
            }
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
                        new MathFunctionSurface(uDimension, vDimension,
                            MathExpression.CreateDefault(MathExpressionTextBox.Text, "x", "y"))
                },
                {
                    "Bicubic",
                    (uDimension, vDimension, knotsGenerator, derivation) =>
                        new BicubicHermiteSpline(uDimension, vDimension, knotsGenerator, derivation)
                },
                {
                    "Biquartic",
                    (uDimension, vDimension, knotsGenerator, derivation) =>
                        new BiquarticHermiteSpline(uDimension, vDimension, knotsGenerator, derivation)
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

            ShapesComboBox.ItemsSource = _functionScene;
            ShapesComboBox.DisplayMemberPath = "Name";
            //var item = new ComboBoxItem();
            //item.Content = "Name";
            //item.Foreground = 
            //ShapesComboBox.ItemTemplate.DataType = new ComboBoxItem();

        }

        //private void Select

        private void SplineComboBox_OnSelectedItem(object sender, SelectionChangedEventArgs e)
        {

            var splineSelector = sender as ComboBox;
            if (splineSelector?.ItemsSource == null) return;
            if (splineSelector.SelectedIndex <= -1 || splineSelector.SelectedIndex >= _functionScene.Count) return;

            ScenesAction(scene => scene.HighlightedShapeIndex = splineSelector.SelectedIndex);

            var selectedSpline = _functionScene[splineSelector.SelectedIndex] as Spline;
            if (selectedSpline != null)
            {
                var culture = CultureInfo.CurrentCulture;
                HermiteUMinTextBox.Text = selectedSpline.UDimension.Min.ToString(culture);
                HermiteVMinTextBox.Text = selectedSpline.VDimension.Min.ToString(culture);
                HermiteUMaxTextBox.Text = selectedSpline.UDimension.Max.ToString(culture);
                HermiteVMaxTextBox.Text = selectedSpline.VDimension.Max.ToString(culture);
                HermiteUCountTextBox.Text = selectedSpline.UDimension.KnotCount.ToString(culture);
                HermiteVCountTextBox.Text = selectedSpline.VDimension.KnotCount.ToString(culture);
                MathExpressionTextBox.Text = selectedSpline.Name;

                if (selectedSpline is BiquarticHermiteSpline)

                    InterpolationTypeComboBox.SelectedIndex = 2;
                else if (selectedSpline is BicubicHermiteSpline)

                    InterpolationTypeComboBox.SelectedIndex = 1;

                if (selectedSpline.KnotsGenerator is DirectKnotsGenerator)
                    KnotsGeneratorComboBox.SelectedIndex = 0;
                else if (selectedSpline.KnotsGenerator is ReducedDeBoorKnotsGenerator)
                    KnotsGeneratorComboBox.SelectedIndex = 2;
                else if (selectedSpline.KnotsGenerator is DeBoorKnotsGenerator)
                    KnotsGeneratorComboBox.SelectedIndex = 1;
            }
            var selectedSurface = _functionScene[splineSelector.SelectedIndex] as MathFunctionSurface;
            if (selectedSurface != null)
            {
                var culture = CultureInfo.CurrentCulture;
                HermiteUMinTextBox.Text = selectedSurface.UDimension.Min.ToString(culture);
                HermiteVMinTextBox.Text = selectedSurface.VDimension.Min.ToString(culture);
                HermiteUMaxTextBox.Text = selectedSurface.UDimension.Max.ToString(culture);
                HermiteVMaxTextBox.Text = selectedSurface.VDimension.Max.ToString(culture);
                HermiteUCountTextBox.Text = selectedSurface.UDimension.KnotCount.ToString(culture);
                HermiteVCountTextBox.Text = selectedSurface.VDimension.KnotCount.ToString(culture);
                InterpolationTypeComboBox.SelectedIndex = 0;
                InterpolationTypeComboBox_OnSelectionChanged(InterpolationTypeComboBox, null);
            }
        }

        private int SetSelectedIndexBySplineType(Type splineType)
        {
            var itemSource = KnotsGeneratorComboBox.ItemsSource as IDictionary<string, SplineFactory>;

            if (itemSource == null) return -1;
            var idx = -1;
            foreach (var pair in itemSource)
            {
                var searchedSplineType = pair.Value.Method.ReturnType;
                if (searchedSplineType == splineType)
                {
                    KnotsGeneratorComboBox.SelectedIndex = idx;
                    return idx;
                }

            }
            return idx;
        }
    }
}