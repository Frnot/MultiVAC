using Discord;

namespace Multivac.Utilities
{
    public class Coin
    {
        public static Embed Flip()
        {
            string result = "";

            int num = RNG.RandomNumber(0, 1);

            if (num == 1)
                result = "HEADS";
            if (num == 0)
                result = "TAILS";

            var embed = new EmbedBuilder
            {
                Title = "Flipping a Coin",
                Description = $"You flipped: **{result}**",
                Color = new Color(RandomColor.NoGrays())
            };
            return embed.Build();
        }
    }
}
