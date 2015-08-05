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
    }
}