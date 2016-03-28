

using System.Collections;

namespace HermiteInterpolation.Utils
{
    public static class EnumerableExtensions
    {
        public static int IndexOf(this IEnumerable enumerable, object someObject)
        {
            int idx = 0;
            foreach (var item in enumerable)
            {
                if (item.Equals(someObject))
                    return idx;
                ++idx;
            }
            return -1;
        }
    }
}
