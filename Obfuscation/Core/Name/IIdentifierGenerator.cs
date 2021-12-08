using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Obfuscation.Core.Name
{
    public interface IIdentifierGenerator
    {
        public static ImmutableList<IIdentifierGenerator> AllGenerators()
        {
            var interfaceType = typeof(IIdentifierGenerator);
            var assembly = Assembly.GetAssembly(interfaceType) ?? throw new Exception("Unable to get the assembly!");
            var allTypes = assembly.GetTypes();
            var whereIsSubclass = allTypes.Where(theType =>
                theType.IsClass && !theType.IsAbstract && theType.GetInterfaces().Contains(interfaceType));
            var selected = whereIsSubclass.Select(theType => Activator.CreateInstance(theType) as IIdentifierGenerator);
            return selected.ToImmutableList();
        }

        public string DisplayName { get; }

        public void ClearCache();

        public string TransformName(string originalName);

        public string TransformClassName(string originalName)
        {
            return TransformName(originalName);
        }

        public string TransformVariableName(string originalName)
        {
            return TransformName(originalName);
        }

        public string TransformPropertyName(string originalName)
        {
            return TransformName(originalName);
        }

        public string TransformMethodName(string originalName)
        {
            return TransformName(originalName);
        }
    }
}