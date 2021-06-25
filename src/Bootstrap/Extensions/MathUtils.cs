using System;

namespace Bootstrap.Extensions
{
    public class MathUtils
    {
        public static readonly Random Random = new Random();

        [Obsolete]
        public static int GetRandomInteger()
        {
            return Random.Next();
        }

        [Obsolete]
        public static int GetRandomInteger(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static int GetRandom() => Random.Next();

        public static int GetRandom(int max, bool compareAbsoluteValue = false) =>
            compareAbsoluteValue ? Random.Next(-max, max) : Random.Next(max);

        public static int GetRandom(int min, int max) => Random.Next(min, max);
    }
}