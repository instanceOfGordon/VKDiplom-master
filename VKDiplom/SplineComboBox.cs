using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace VKDiplom
{
    public class SplineComboBox : ComboBox
    {
        private readonly IList<Type> _splineTypes = new List<Type>();
        private readonly IList<Type> _splineGeneratorTypes = new List<Type>();

        public SplineComboBox RegisterSplineType(params Type[] types)
        {
            foreach(var type in types)
            {
                _splineTypes.Add(type);
            }
            return this;
        }

        public SplineComboBox RegisterSplineGeneratorType(params Type[] types)
        {
            foreach (var type in types)
            {
                _splineGeneratorTypes.Add(type);
            }
            return this;
        }

        public int SplineTypeId(Type type)
        {
            for (int i = 0; i < _splineTypes.Count; i++)
            {
                if (type.Equals(_splineTypes[i]))
                    return i;
            }
            return -1;
        }
        public int SplineGeneratorTypeId(Type type)
        {
            for (int i = 0; i < _splineTypes.Count; i++)
            {
                if (type.Equals(_splineTypes[i]))
                    return i;
            }
            return -1;
        }

    }
}
