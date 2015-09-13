using System;
using System.Linq.Expressions;
using System.Text;

namespace HermiteInterpolation.Utils
{
    internal static class MyArrays
    {
        internal static T[][] JaggedArray<T>(int n, int m)
        {
            var array = new T[n][];
            for (int i = 0; i < n; i++)
            {
                array[i] = new T[m];
            }
            return array;
        }

        internal static T[] InitalizedArray<T>(int n, T value)
        {
            var array = new T[n];
            for (int i = 0; i < n; i++)
            {
                array[i] = value;
            }
            return array;
        }

        internal static string[] AsStrings<T>(T[][] array)
        {
            var strings = new string[array.Length];//JaggedArray<string>(array.Length, array[0].Length);
            
            for (int i = 0; i < array.Length; i++)
            {
                var sb = new StringBuilder("[\t");
                for (int j = 0; j < array[0].Length; j++)
                {
                    sb.Append($"{i} {j}| ").Append(array[i][j].ToString()).Append("\t");
                }
                strings[i] = sb.ToString();
            }
            return strings;
        }

        internal static void WriteArray<T>(T[][] array)
        {
            var strings = AsStrings(array);
            for (int i = 0; i < strings.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine(strings[i]);
                Console.WriteLine(strings[i]);
            }
        }
    }
}