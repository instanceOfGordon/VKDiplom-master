using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using HermiteInterpolation.Shapes.SplineInterpolation;
using HermiteInterpolation.SplineKnots;

namespace VKDiplom
{
    public partial class MainPage
    {
        private bool _splineSubtractionClicked = false;
        //private SurfaceDimension _uDimensionOnSubtract;
        //private SurfaceDimension _vDimensionOnSubtract;
        private SplineFactory _minuendShapeFactory;
        private KnotsGeneratorFactory _minuendKnotsFactory;
        private string _nameOnSubtract;

        private void SplineSubtractionButton_OnClick(object sender, RoutedEventArgs e)
        {
            _splineSubtractionClicked = true;
            ShapesTextBoxesEnabled = false;
            _minuendShapeFactory = (SplineFactory)InterpolationTypeComboBox.SelectedValue;
            _minuendKnotsFactory = (KnotsGeneratorFactory)KnotsGeneratorComboBox.SelectedValue;
            _nameOnSubtract = MathExpressionTextBox.Text;
        }

        private bool ShapesTextBoxesEnabled
        {
            get { return HermiteUMaxTextBox.IsEnabled; }
            set
            {
                HermiteUMinTextBox.IsEnabled = value;
                HermiteVMinTextBox.IsEnabled = value;
                HermiteUMaxTextBox.IsEnabled = value;
                HermiteVMaxTextBox.IsEnabled = value;
                HermiteUCountTextBox.IsEnabled = value;
                HermiteVCountTextBox.IsEnabled = value;
            }    
        }
        

    }
}
