using System;
using System.Collections.Generic;
using System.Linq;
using Obfuscation.Utils;

namespace Obfuscation.Core.Name
{
    public class MeaninglessIdentifierGenerator : IIdentifierGenerator
    {
        public string DisplayName => "Meaningless";

        private static readonly IDictionary<string, string> ExistingKeys = new Dictionary<string, string>();
        private static readonly IList<string> MethodNames = new List<string> {
            string.Empty,
            "get",
            "set",
            "find",
            "run",
            "open",
            "calculate",
            "do",
            "is",
            "insert",
            "delete",
            "create",
            "update",
            "add",
            "remove",
            "count",
            "tag",
            "send",
            "save",
            "copy"
        };

        private static readonly IList<string> VariableNames = new List<string>
        {
            "manager",
            "object",
            "service",
            "controller",
            "helper",
            "handler",
            "factory",
            "builder"
        };

        private static readonly IList<string> Adjectives = new List<string>
        {
            "happy",
            "sad",
            "specific",
            "small",
            "default",
            "dummy",
            "mock",
        };

        private static readonly IDictionary<string, string> UsedMethods = new Dictionary<string, string>();

        public void ClearCache()
        { 
            ExistingKeys.Clear();
            UsedMethods.Clear();
        }

        public string TransformName(string originalName)
        {
            if (originalName == string.Empty)
            {
                var newName = Adjectives.GetRandomElement() + VariableNames.GetRandomElement().Capitalize() +
                              VariableNames.GetRandomElement().Capitalize();
                ExistingKeys[string.Empty] = newName;
                return newName;
            }

            var strippedName = string.Concat(originalName.Where(char.IsLetterOrDigit));
            var capitalizedName = strippedName.Capitalize();
            var name = string.Concat(capitalizedName.Where(x => char.IsUpper(x) || char.IsDigit(x)));
            var counter = 2;

            var alteredName = name;
            while (ExistingKeys.ContainsKey(alteredName) && ExistingKeys[alteredName] != originalName)
            {
                alteredName = $"{name}{counter}";
                counter++;
            }

            ExistingKeys[alteredName] = originalName;
            return alteredName;
        }

        public string TransformClassName(string originalName)
        {
            return $"Class{TransformName(originalName)}";
        }

        public string TransformVariableName(string originalName)
        {
            return $"var{TransformName(originalName)}";
        }

        public string TransformPropertyName(string originalName)
        {
            return $"prop{TransformName(originalName)}";
        }

        public string TransformMethodName(string originalName)
        {
            if (!UsedMethods.ContainsKey(originalName)) {
                UsedMethods[originalName] = MethodNames.OrderBy(_ => Guid.NewGuid()).FirstOrDefault();
            }
            return $"{UsedMethods[originalName]}{TransformName(originalName)}";
        }
    }

    internal static class StringUtils
    {
        internal static string Capitalize(this string s)
        {
            if (s.Length <= 1) return s.ToUpper();
            
            var firstLetter = s[0];
            var restOfTheString = s[1..];
            return char.ToUpper(firstLetter) + restOfTheString;

        }
    }
}