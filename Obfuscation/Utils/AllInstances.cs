using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Obfuscation.Utils
{
    public static class AllInstances
    {
        public static ImmutableList<T> OfType<T>() where T : class
        {
            var mainType = typeof(T);
            var assembly = Assembly.GetAssembly(mainType) ?? throw new Exception("Unable to get the assembly!");
            return assembly.GetTypes()
                    .Where(aType => !aType.IsAbstract && aType.InheritsFrom(mainType))
                    .Select(theType => Activator.CreateInstance(theType) as T)
                    .ToImmutableList();
        }

        private static bool InheritsFrom(this Type someType, Type otherType)
        {
            if (otherType.IsClass)
            {
                return someType.GetTypeInfo().IsSubclassOf(otherType);
            }
            else if (otherType.IsInterface)
            {
                var implementedInterfaces = someType.GetInterfaces();

                if (otherType.IsGenericType)
                {
                    return implementedInterfaces.Any(anInterface =>
                        anInterface.Name.Equals(otherType.Name) && anInterface.Namespace.Equals(otherType.Namespace));
                }
                else
                {
                    return implementedInterfaces.Contains(otherType);
                }
            }

            return false;
        }
    }
}