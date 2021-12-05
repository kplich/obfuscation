using System;
using System.Collections.Generic;
using System.Linq;

namespace Obfuscation.Core.Name
{
    public class MeaninglessIdentifierGenerator : IIdentifierGenerator
    {
        public string DisplayName => "Meaningless";

        private static IDictionary<string, string> existingKeys = new Dictionary<string, string>();
        private static readonly IList<string> methodNames = new List<string> {
            { string.Empty },
            { "get" },
            { "set" },
            { "find" },
            { "run" },
            { "open" },
            { "calculate" },
            { "do" },
            { "is" },
            { "insert" },
            { "delete" },
            { "create" },
            { "update" },
            { "add" },
            { "remove" },
            { "count" },
            { "tag" },
            { "send" },
            { "save" },
            { "copy" }
        };

        private static IDictionary<string, string> usedMethods = new Dictionary<string, string>();

        public void ClearCache()
        { 
            existingKeys.Clear();
            usedMethods.Clear();
        }

        public string GenerateName(string originalName)
        {
            var strippedName = String.Concat(originalName.Where(c => (char.IsLetterOrDigit(c))));
            var capitalizedName = char.ToUpper(strippedName[0]) + strippedName.Substring(1);
            var name = String.Concat(capitalizedName.Where(x => Char.IsUpper(x) || Char.IsDigit(x)));
            var counter = 2;
            var alteredName = name;
            while (existingKeys.ContainsKey(alteredName) && existingKeys[alteredName] != originalName) {
               alteredName = $"{name}{counter}";
               counter++;
            }
            existingKeys[alteredName] = originalName;
            return alteredName;
        }

        public string GenerateClassName(string originalName)
        {
            return $"Class{GenerateName(originalName)}";
        }

        public string GenerateVariableName(string originalName)
        {
            return $"var{GenerateName(originalName)}";
        }

        public string GeneratePropertyName(string originalName)
        {
            return $"prop{GenerateName(originalName)}";
        }

        public string GenerateMethodName(string originalName)
        {
            if (!usedMethods.ContainsKey(originalName)) {
                usedMethods[originalName] = methodNames.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            }
            return $"{usedMethods[originalName]}{GenerateName(originalName)}";
        }
    }
}