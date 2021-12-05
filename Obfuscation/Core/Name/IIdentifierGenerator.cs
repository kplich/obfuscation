using System;
using System.Collections.Generic;
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

        public string GenerateName(string originalName);

        public string GenerateClassName(string originalName)
        {
            return GenerateName(originalName);
        }

        public string GenerateVariableName(string originalName)
        {
            return GenerateName(originalName);
        }

        public string GeneratePropertyName(string originalName)
        {
            return GenerateName(originalName);
        }

        public string GenerateMethodName(string originalName)
        {
            return GenerateName(originalName);
        }
    }
}