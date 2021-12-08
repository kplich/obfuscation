using System;

namespace Obfuscation.Core.Name
{
    public class GuidIdentifierGenerator : IIdentifierGenerator
    {
        public string DisplayName => "GUID";

        public void ClearCache()
        { }

        public string TransformName(string originalName)
        {
            var guid = Guid.NewGuid().ToString().Replace("-", "");
            var firstChar = guid[0];
            while (firstChar.IsDigit())
            {
                guid = guid[1..] + firstChar;
                firstChar = guid[0];
            }

            return guid;
        }
    }

    internal static class Utils
    {
        internal static bool IsDigit(this char c)
        {
            return char.IsDigit(c);
        }
    }
}