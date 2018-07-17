using System;
using System.Security.Cryptography;

namespace Multivac
{
    public static class RNG
    {
        private static RNGCryptoServiceProvider cryptoRand = new RNGCryptoServiceProvider();

        public static int RandomNumber(int min, int max)
        {
            // increase range by one to account for rounding
            int range = (max - min) + 1;

            byte[] fourBytes = new byte[4];
            // get four completely random bytes
            cryptoRand.GetBytes(fourBytes);

            // convert random bytes to an int (rand)
            int rand = Math.Abs(BitConverter.ToInt32(fourBytes, 0));
            // divide rand by its highest possible value (+1) to ensure result is always decimal
            double salt = rand / ((double)int.MaxValue + 1);
            // use salt to generate a whole number within range
            int result = (int)((range * salt) + min);

            return (result);
        }

        public static int[] RandomNumber(int min, int max, int count)
        {
            int[] randArray = new int[count];

            for (int i = 0; i < count; i++)
            {
                randArray[i] = RandomNumber(min, max);
            }

            return (randArray);
        }


    }
}
