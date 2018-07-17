using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
        private readonly DatabaseHandler _databaseHandler;

        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services, DatabaseHandler databaseHandler)
        {
            _client = client;
            _commands = commands;
            _services = services;
            _databaseHandler = databaseHandler;
        } // end Constructor

        public async Task InitializeCommandsAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _commands.Log += RunBot.Log;

            _client.MessageReceived += HandleCommandAsync;
            _client.MessageUpdated += MessageUpdatedAsync;

        } // end InitCommands

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null || message.Author.IsBot) return;

            var context = new SocketCommandContext(_client, message);

            string botPrefix = _databaseHandler.GetGuildPrefix(context.Guild.Id);
            
            int argPos = 0;
            if (!(message.HasStringPrefix(botPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            //if (result.Error == CommandError.UnmetPrecondition)
                await context.Channel.SendMessageAsync(result.ErrorReason);

        } // end HandleCommandAsync()

        private async Task MessageUpdatedAsync(Cacheable<IMessage, ulong> beforeEdit, SocketMessage afterEdit, ISocketMessageChannel channel)
        {
            //let admins turn this on/off per guild
            await HandleCommandAsync(afterEdit);
        } // end MessageUpdatesAsync()

    }
}
