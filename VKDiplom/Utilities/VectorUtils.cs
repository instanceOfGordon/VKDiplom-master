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
using Microsoft.Xna.Framework;

namespace VKDiplom.Utilities
{
    public static class VectorUtils
    {
        public static float Angle(Vector2 a, Vector2 b)
        {
            var dot = Vector2.Dot(a,b);
            var mulLength = a.Length()*b.Length();           
            return (float)Math.Acos(dot/mulLength);
        } 
    }
}
