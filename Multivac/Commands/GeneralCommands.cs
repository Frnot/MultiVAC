using Discord.Commands;
using Multivac.Utilities;
using System.Threading.Tasks;

namespace Multivac.Modules
{
    public class GeneralCommands : ModuleBase<SocketCommandContext>
    {
        

        [Command("roll", RunMode = RunMode.Async)]
        public async Task Roll(string input)
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync("", embed: Dice.Roll(input));
        }

        [Command("flip", RunMode = RunMode.Async)]
        public async Task Flip()
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync("", embed: Coin.Flip());
        }

        
    }
}
