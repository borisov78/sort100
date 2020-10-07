using System;

namespace Sort100.Generator
{
    public static class RandomUtil
    {
        private static readonly Random Random = new Random();
        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }
    }
}