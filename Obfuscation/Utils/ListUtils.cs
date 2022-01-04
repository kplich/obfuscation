using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscation.Utils
{
    public static class ListUtils
    {
        public static T GetRandomElement<T>(this IList<T> list)
        {
            return list[new Random().Next(list.Count)];
        }
        
        public static List<T> SwapElements<T>(this List<T> list, int first, int second)
        {
            (list[first], list[second]) = (list[second], list[first]);
            return list;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            var rand = new Random();
            return list.OrderBy(_ => rand.Next()).ToList();
        }
    }
}