using Discord;
using System;

namespace Multivac.Utilities
{
    class Dice
    {

        public static Embed Roll(string input)
        {   // ./roll 10d100+5 where: rolls = 10, sides = 100, mod = +, modValue = 5

            // value before 'd' (defaults to 1)
            int rolls;
            // value after 'd' (defaults to 6)
            int sides;
            // the modifier (either + or - )
            char mod = '\0';
            // the value to modify the roll with
            int modValue = 0;
            bool hasMod;

            input = input.ToLower();
            int length = input.Length;

            if (!input.Contains("d"))
            {
                return Error("invalid input");
            }
            int dIndx = input.IndexOf("d");


            // if the user did not specify the ammount of rolls ('d' is the first character), default to one roll
            if (dIndx == 0)
            {
                rolls = 1;
            }
            // if the characters before 'd' cannot be interpreted as numbers, throw an error.
            else
            {
                try
                {
                    rolls = Int32.Parse(input.Substring(0, dIndx));
                }
                catch (Exception)
                {
                    return Error("invalid input");
                }
            } 


            // if input contains any modifiers
            if (input.Contains("+") || input.Contains("-") || input.Contains("x") || input.Contains("*") || input.Contains("/"))
            {
                hasMod = true;
                int modIndx = 0;

                // define the modifier and find its position
                if (input.Contains("+"))
                {
                    mod = '+';
                    modIndx = input.IndexOf("+");
                }
                else if (input.Contains("-"))
                {
                    mod = '-';
                    modIndx = input.IndexOf("-");
                }
                else if (input.Contains("x"))
                {
                    mod = '*';
                    modIndx = input.IndexOf("x");
                }
                else if (input.Contains("*"))
                {
                    mod = '*';
                    modIndx = input.IndexOf("*");
                }
                else if (input.Contains("/"))
                {
                    mod = '/';
                    modIndx = input.IndexOf("/");
                }

                // if the user did not specify the ammount of sides (there are no 
                // characters between 'd' and the modifier), default to a six sided die.
                if ((modIndx - dIndx) == 1)
                {
                    sides = 6;
                }
                // if the characters between 'd' and the modifier cannot be interpreted as numbers, throw an error.
                else
                {
                    try
                    {
                        sides = Int32.Parse(input.Substring((dIndx + 1), (modIndx - (dIndx + 1))));
                    }
                    catch (Exception)
                    {
                        return Error("invalid input");
                    }   
                }

                // if the characters after the modifier cannot be interpreted as numbers, throw an error.
                try
                {
                    modValue = Int32.Parse(input.Substring(modIndx + 1));
                }
                catch (Exception)
                {
                    return Error("invalid input");
                }
            }
            // else if input does not contain any modifiers
            else
            {
                hasMod = false;

                // if the user did not specify the ammount of sides ('d' is the last character), 
                // default to a six sided die.
                if (dIndx == (length - 1))
                {
                    sides = 6;
                }
                // if the characters after 'd' cannot be interpreted as numbers, throw an error.
                else
                {
                    try
                    {
                        sides = Int32.Parse(input.Substring(dIndx + 1));
                    }
                    catch (Exception)
                    {
                        return Error("invalid input");
                    }
                }
            }

           

            // generates (rolls) ammount of random numbers between (1 - sides)
            int[] randomNums = RNG.RandomNumber(1, sides, rolls);

            // concat all the random numbers into a string (seperated by ", ")
            string rollResults = string.Join(", ", randomNums);

            // sum all the random numbers into a total
            int total = 0;
            foreach (int num in randomNums)
            {
                total += num;
            }

            // if the input contained modifiers, modify the total accordingly
            if (hasMod)
            {
                int preModTotal = total;
                switch (mod)
                {
                    case '+':
                        total += modValue;
                        break;
                    case '-':
                        total -= modValue;
                        break;
                    case '*':
                        total *= modValue;
                        break;
                    case '/':
                        total /= modValue;
                        break;
                }

                // then build an embeded  and return
                var embed = new EmbedBuilder
                {
                    Title = $"Rolling: {input}",
                    Description = rollResults + '\n' +
                        $"Total: {preModTotal} {mod} {modValue} = **{total:n0}**",
                    Color = new Color(RandomColor.NoGrays())
                };
                return embed.Build();
            }
            // else build an embeded with the unmodified total and return
            else
            {
                var embed = new EmbedBuilder
                {
                    Title = $"Rolling: {input}",
                    Description = rollResults + '\n' +
                        $"Total: **{total:n0}**",
                    Color = new Color(RandomColor.NoGrays())
                };
                return embed.Build();
            }
        } // end Dice

        public static Embed Error(string errorMessage)
        {
            var embed = new EmbedBuilder
            {
                Title = "Error",
                Description = errorMessage,
                Color = new Color(2895667),
            };
            return embed.Build();
        } // end Error

    }
}
