using System;

namespace Obfuscation.Core.Bloat.ReplaceLiteralWithProperty.Collatz
{
    public static class CollatzUtils
    {
        internal const int GreatestSmallNumber = 10;
        internal const int SmallestMediumNumber = 11;
        internal const int GreatestMediumNumber = 200;
        
        internal static int GenerateNumberWithSequenceLength(int length)
        {
            if (length.IsBig())
            {
                throw new ArgumentException("number must not be big (greater than 200)");
            }

            int resultLength;
            int temp;
            do
            {
                temp = new Random().Next((int) Math.Pow(2, 14));
                resultLength = CalculateSequenceLength(temp);
            } while (resultLength != length);

            return temp;
        }
        
        internal static int CalculateSequenceLength(int number)
        {
            var length = 0;

            var temp = number;
            while (temp > 1)
            {
                length++;

                if (temp % 2 == 0)
                {
                    temp /= 2;
                }
                else
                {
                    temp *= 3;
                    temp += 1;
                }
            }

            return length + 1;
        }

        private static bool IsBetweenIncluding(this int number, int fromIncluding, int toIncluding)
        {
            return number >= fromIncluding && number <= toIncluding;
        }

        public static bool IsSmall(this int number)
        {
            return number.IsBetweenIncluding(0, GreatestSmallNumber);
        }

        public static bool IsMedium(this int number)
        {
            return number.IsBetweenIncluding(SmallestMediumNumber, GreatestMediumNumber);
        }

        public static bool IsBig(this int number)
        {
            return number > GreatestMediumNumber;
        }
    }
}