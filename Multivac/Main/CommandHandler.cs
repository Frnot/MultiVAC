using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Multivac.Data;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Multivac.Main
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly LiteDatabase db;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, LiteDatabase dbService)
        {
            _client = client;
            _commands = commands;
            _services = services;
            db = dbService;
        } // end Constructor

        public async Task InitializeCommandsAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _commands.Log += RunBot.Log;

            _client.MessageReceived += HandleCommandAsync;
        } // end InitCommands

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;

            var context = new SocketCommandContext(_client, message);

            string botPrefix = GetGuildPrefix(context.Guild.Id);
            
            int argPos = 0;
            if (!(message.HasStringPrefix(botPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            //if (result.Error == CommandError.UnmetPrecondition)
                await context.Channel.SendMessageAsync(result.ErrorReason);

        } // end HandleCommandAsync()



        public string GetGuildPrefix(ulong guildId)
        {
            var guilds = db.GetCollection<GuildData>("guilds");

            var guild = guilds.FindOne(x => x.GuildId == guildId);

            var thisGuildPrefix = guild.CommandPrefix;

            return (thisGuildPrefix == "default") ? Variables.DefaultCommandPrefix : thisGuildPrefix;
        } // end GetGuildPrefix

        public async Task ChangeGuildPrefixAsync(SocketCommandContext Context, string newPrefix)
        {
            if (string.IsNullOrEmpty(newPrefix)) return;

            var guildList = db.GetCollection<GuildData>("guilds");
            var thisGuild = guildList.FindOne(x => x.GuildId == Context.Guild.Id);

            if (newPrefix.ToLower().Equals("reset"))
            {
                thisGuild.CommandPrefix = "";
                await Context.Channel.SendMessageAsync($"the command prefix has been reset.");
            }
            else
            {
                thisGuild.CommandPrefix = newPrefix;
                await Context.Channel.SendMessageAsync($"the command prefix has been changed.");
            }
            guildList.Update(thisGuild);

            await Context.Channel.SendMessageAsync($"the command prefix for {Context.Guild.Name} is now `{GetGuildPrefix(Context.Guild.Id)}`");
        } // end ChangeGuildPrefixAsync

    }
}
