using System;
using System.Collections.Generic;

namespace Obfuscation.Utils
{
    public static class ListUtils
    {
        public static T GetRandomElement<T>(this IList<T> list)
        {
            return list[new Random().Next(list.Count)];
        }
    }
}