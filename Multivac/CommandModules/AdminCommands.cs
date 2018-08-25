using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using Microsoft.CodeAnalysis.Scripting;
using Multivac.Utilities;
using System.Text.RegularExpressions;
using System.Linq;

namespace Multivac.Main
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        public CommandHandler _commandHandler { get; set; }

        [Command("eval")]
        [RequireOwner]
        public async Task Eval([Remainder] string input)
        {
            var pattern = new Regex("```(?i)(cs)?(?s)(.*)```");
            string evalString = pattern.Match(input).Groups[2].Value;

            if (evalString == null) return;

            await EvalService.EvaluateAsync(evalString, Context);
        } // end Eval

        

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task ChangePrefix([Remainder] string newPrefix)
        {
            await _commandHandler.ChangeGuildPrefixAsync(Context, newPrefix);
        } // end ChangePrefix(string)

        [Command("prefix")]
        public async Task ChangePrefix()
        {
            string prefix = _commandHandler.GetGuildPrefix(Context.Guild.Id);

            await ReplyAsync(embed: new EmbedBuilder()
                .WithDescription($"{Context.User.Mention} the command prefix for {Context.Guild.Name} is `{prefix}`")
                //.WithColor(RandomColor.NoGrays())
                .Build());
        }


        [Command("spam")]
        [RequireOwner]
        public async Task Spam(int input)
        { //used for testing, not for evil
            for (int i = 0; i < input; i++)
            {
                await ReplyAsync((i + 1).ToString());
            }
        }

        [Command("test")]
        [RequireOwner]
        public async Task Test(SocketGuildUser user)
        {
            await ReplyAsync($"username: {user.Username}");
            await ReplyAsync($"nickname: {user.Nickname}");

            //var fileName = "image.png";

            //var embed = new EmbedBuilder()
            //{
            //    Title = "picture",
            //    Description = "yeet boi",
            //    ThumbnailUrl = $"attachment://{fileName}"
            //}.Build();
            //await Context.Channel.SendFileAsync($"pictures/{fileName}", embed: embed);
            
            
        }

    } // end class AdminCommands
}
