using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Multivac.RollPlayingCompanion.RobsRuleset;
using Multivac.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multivac.RollPlayingCompanion
{
    public class RPCInteractiveCommandModule : InteractiveBase
    {
        private readonly DiscordSocketClient _client;

        private SocketUser _GM;
        private ISocketMessageChannel _RPChannel;
        private List<RobsRulesetCharacter> listOfCharacters;

        public RPCInteractiveCommandModule(DiscordSocketClient client)
        {
            _client = client;
        }


        [Command("startgame", RunMode = RunMode.Async)]
        public async Task RunRPC()
        {
            _RPChannel = Context.Channel;
            _GM = Context.User;
            await _RPChannel.SendMessageAsync($"{_GM.Mention} is now the GM");

            await RegisterCharactersAsync();
        }



        public async Task RegisterCharactersAsync()
        {
            int numOfPlayers;

            while (true)
            {
                await ReplyAsync($"{_GM.Mention} Enter the number of players");
                var response = await NextMessageAsync(timeout: new TimeSpan(1, 0, 0));

                if (response == null) return;

                if (int.TryParse(response.Content, out numOfPlayers) && (0 < numOfPlayers && numOfPlayers <= 50))
                {
                    await ReplyAsync($"Number of players: {numOfPlayers}");
                    break;
                }
                else await ReplyAsync("please enter a valid number between 1-50");
            }

            listOfCharacters = new List<RobsRulesetCharacter>(numOfPlayers);
            for (int i = 0; i < numOfPlayers; i++)
            {
                await ReplyAsync($"Player {i + 1} please type your name:");

                SocketMessage response;
                while (true)
                {
                    response = await NextMessageAsync(false, timeout: new TimeSpan(1, 0, 0));
                    if ((response.Author == _GM) || (listOfCharacters.Any(c => c.characterId == response.Author.Id)))
                    {
                        await ReplyAsync($"you're already a player, {response.Author.Mention}");
                        continue;
                    }
                    else if (response.Author.IsBot) continue;
                    else break;
                }
                

                listOfCharacters.Add(new RobsRulesetCharacter(response.Content, response.Author.Id));
                var newPlayer = listOfCharacters.Find(c => c.characterId == response.Author.Id);

                newPlayer.GenerateCharacter();
                await ReplyAsync($"Stats for {newPlayer.name}\n" + newPlayer.PrintStats());
            }

            var embed = new EmbedBuilder();
            embed.WithTitle("All Characters");
            foreach (var character in listOfCharacters)
            {
                embed.AddField(character.name, character.PrintStats());
            }

            await ReplyAsync(embed: embed.Build());

        } // end RegisterCharacters

        [Command("rollfek", RunMode = RunMode.Async)]
        public async Task Rollfek(string input)
        {
            await Context.Channel.TriggerTypingAsync();
            await ReplyAsync("", embed: Dice.Roll(input));
        }

        [Command("next", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            await ReplyAsync("What is 2+2?");
            var response = await NextMessageAsync();
            if (response != null)
                await ReplyAsync($"You replied: {response.Content}");
            else
                await ReplyAsync("You did not reply before the timeout");
        }
    }
}
