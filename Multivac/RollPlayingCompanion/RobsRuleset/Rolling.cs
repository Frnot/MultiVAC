using System;
using System.Collections.Generic;
using System.Linq;

namespace Multivac.RollPlayingCompanion.RobsRuleset
{
    class Rolling
    {
        public static int R5d6DL()
        {
            List<int> rollResult = RNG.RandomNumber(1, 6, 5).ToList();

            Console.WriteLine($"initial roll: {string.Join(", ", rollResult)}");

            if (rollResult.Contains(6))
            {
                var numofsixes = rollResult.Where(n => n == 6).Count();
                while (numofsixes > 0)
                {
                    Console.WriteLine("Rolling again for each six");

                    var newroll = RNG.RandomNumber(1, 6, numofsixes);

                    Console.WriteLine($"new roll: {string.Join(", ", newroll)}");

                    rollResult.AddRange(newroll);

                    Console.WriteLine($"updated roll: {string.Join(", ", rollResult)}");

                    numofsixes = newroll.Where(n => n == 6).Count();
                }
            }


            Console.WriteLine("removing lowest number");
            rollResult.RemoveAt(rollResult.IndexOf(rollResult.Min()));
            Console.WriteLine($"updated roll: {string.Join(", ", rollResult)}");


            var total = rollResult.Sum();
            Console.WriteLine($"total: {total}");

            if (total < 20)
            {
                Console.WriteLine("rounding total up to 20");
                total = 20;
                Console.WriteLine($"final total: {total}");
            }

            return total;
        } // end R5d6DL

        public static int R3d6()
        {
            List<int> rollResult = RNG.RandomNumber(1, 6, 3).ToList();

            Console.WriteLine($"initial roll: {string.Join(", ", rollResult)}");

            if (rollResult.Contains(6))
            {
                var numofsixes = rollResult.Where(n => n == 6).Count();
                while (numofsixes > 0)
                {
                    Console.WriteLine("Rolling again for each six");

                    var newroll = RNG.RandomNumber(1, 6, numofsixes);

                    Console.WriteLine($"new roll: {string.Join(", ", newroll)}");

                    rollResult.AddRange(newroll);

                    Console.WriteLine($"updated roll: {string.Join(", ", rollResult)}");

                    numofsixes = newroll.Where(n => n == 6).Count();
                }
            }

            var total = rollResult.Sum();
            Console.WriteLine($"total: {total}");

            return total;
        } // end R3d6

        public static int R1d6()
        {
            int total = RNG.RandomNumber(1, 6);
            Console.WriteLine($"initial roll: {total}");

            int roll = total;
            while (roll == 6)
            {
                Console.WriteLine("Rolling again for each six");

                roll = RNG.RandomNumber(1, 6);
                Console.WriteLine($"new roll: {roll}");

                total += roll;
                Console.WriteLine($"new total: {total}");
            }

            return total;
        } // end R1d6
    }
}
