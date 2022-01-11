using System.Collections.Immutable;
using Obfuscation.Utils;

namespace Obfuscation.Core.Name
{
    public interface IIdentifierGenerator
    {
        public static ImmutableList<IIdentifierGenerator> AllIdentifierGenerators()
        {
            return AllInstances.OfType<IIdentifierGenerator>();
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