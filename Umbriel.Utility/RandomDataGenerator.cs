using System;
using System.Collections.Generic;
using System.Text;

namespace Umbriel.Utility
{
    public class RandomDataGenerator
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Creates a random string
        /// </summary>
        /// <param name="size">The size of the random string.</param>
        /// <returns></returns>
        public string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[random.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        /// <summary>
        /// Randoms the string.
        /// </summary>
        /// <param name="minsize">The minsize.</param>
        /// <param name="maxsize">The maxsize.</param>
        /// <returns></returns>
        public string RandomString(int minsize ,int maxsize)
        {
            int size = random.Next(minsize, maxsize);

            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[random.Next(_chars.Length)];
            }
            return new string(buffer);
        }

        /// <summary>
        /// Randoms the int.
        /// </summary>
        /// <param name="minsize">The minsize.</param>
        /// <param name="maxsize">The maxsize.</param>
        /// <returns></returns>
        public int RandomInt(int minsize, int maxsize)
        {
            return random.Next(minsize, maxsize);
        }

    }
}
