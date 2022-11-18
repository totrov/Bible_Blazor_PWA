using System.Collections.Generic;
using System.Linq;

namespace Bible_Blazer_PWA.Extensions
{
    public static class Collections
    {
        public static void Put<TKey, TValue>(this SortedDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static string ConcatWithDotDelemiter(this int[] array)
        {
            return string.Join('.', array.Select(x => x.ToString()));
        }
    }
}
