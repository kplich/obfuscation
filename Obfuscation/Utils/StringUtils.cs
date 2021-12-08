using System.Text;

namespace Obfuscation.Utils
{
    public static class StringUtils
    {
        public static string Repeat(this string s, int numberOfTimes)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < numberOfTimes; i++)
            {
                builder.Append(s);
            }

            return builder.ToString();
        }
    }
}