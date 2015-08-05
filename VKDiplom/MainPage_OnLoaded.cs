#region

using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using HermiteInterpolation.Functions;
using HermiteInterpolation.Shapes.HermiteSpline;
using VKDiplom.Engine;
using VKDiplom.Engine.Utils;
using VKDiplom.Utils;

#endregion

namespace VKDiplom
{
    public partial class MainPage
    {
        private void FunctionDrawingSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadScene(sender as DrawingSurface, out _functionScene);

            MexicanHatDemo(_functionScene, Derivation.Zero);
        }
        
        private void MexicanHatDemo(Scene scene, Derivation derivation)
        {
            if(scene==null)return;
//            Func<double, double, double> f = (x, y) => Math.Sin(Math.Sqrt(x*x + y*y));
//            Func<double, double, double> xd = (x, y) => (x*Math.Cos(Math.Sqrt(x*x + y*y)))
//                                                        /(Math.Sqrt(x*x + y*y));
//            Func<double, double, double> yd = (x, y) => (y*Math.Cos(Math.Sqrt(x*x + y*y)))
//                                                        /Math.Sqrt(x*x + y*y);
//            Func<double, double, double> xyd = (x, y) => -(y*x*Math.Sin(Math.Sqrt(x*x + y*y)))
//                                                         /(x*x + y*y)
//                                                         - (y*x*Math.Cos(Math.Sqrt(x*x + y*y)))
//                                                         /(Math.Pow(x*x + y*y, 1.5));


            //var aproximationFunction = new InterpolatedFunction(f, xd, yd, xyd);
            //var aproximationFunction = new InterpolatedFunction(f);
            var aproximationFunction = InterpolatedFunction.FromString("sin(sqrt(x^2+y^2))", "x", "y");
            //var aproximationFunction = InterpolatedFunction.FromString("x^4+y^4", "x", "y");
//            var shape = new HermiteSurface(new double[] {-3, -2, -1, 0, 1, 2, 3},
//                new double[] {-3, -2, -1, 0, 1, 2, 3},
                //  aproximationFunction, derivation);
                //var shape = new ClassicHermiteSurface(new double[] { -2, -1, 0, 1 }, new double[] { -2, -1, 0, 1 },
                // var shape = new HermiteShape(new double[] { -2, -1 }, new double[] { -2, -1 },
            var shape = HermiteSurfaceFactory.CreateBiquartic(-3,3,7,-3,3,7,
            //var shape = HermiteSurfaceFactory.CreateBiquartic(-3, 1, 7, -3, 1, 7,
                aproximationFunction, derivation);
            //shape.ColoredHeight();
            //shape.ColoredSimple(Color.FromNonPremultiplied(96,72,128,255));
            shape.ColoredBySegment();
            //shape.DrawStyle = DrawStyle.Wireframe;
            scene.Add(shape);
        }

        private void FirstDerivationDrawingSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadScene(sender as DrawingSurface, out _firstDerScene);
            MexicanHatDemo(_firstDerScene, Derivation.First);
        }

        private void SecondDerivationDrawingSurface_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadScene(sender as DrawingSurface, out _secondDerScene);
            MexicanHatDemo(_secondDerScene, Derivation.Second);
        }


        private void LoadScene(DrawingSurface drawingSurface, out Scene scene)
        {
            if (VkDiplomGraphicsInitializationUtils.Is3DBlocked())
            {
                VkDiplomGraphicsInitializationUtils.ShowReport();
                scene = null;
                return;
            }
            scene = new Scene(_camera);


            //_functionScene.Camera.AspectRatio =1.0f;
            //GraphicsDeviceManager.Current.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise; 

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
    }
}