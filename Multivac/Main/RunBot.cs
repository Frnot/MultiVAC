using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Multivac.Data;
using SharpLink;
using System;
using System.Threading.Tasks;

namespace Multivac.Main
{
    public class RunBot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly LavalinkManager _lavalinkManager;
        private readonly LiteDatabase _dbService;
        private readonly IServiceProvider _services;

        public RunBot(DiscordSocketClient client = null, CommandService commands = null, 
                      LavalinkManager lavalinkManager = null, IServiceProvider services = null,
                      LiteDatabase dbService = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 50,
            });

            _commands = commands ?? new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            });

            _lavalinkManager = lavalinkManager ?? new LavalinkManager(_client, new LavalinkManagerConfig()
            {
                //RESTHost = "localhost",
                RESTHost = "127.0.0.1",
                RESTPort = 2333,
                //WebSocketHost = "localhost",
                WebSocketHost = "127.0.0.1",
                WebSocketPort = 80,
                Authorization = "yeet",
                //TotalShards = 1
            });

            _dbService = dbService ?? new LiteDatabase(@"GuildData.db");

            _services = services ?? new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_lavalinkManager)
                .AddSingleton(_dbService)
                .AddSingleton<InteractiveService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<DatabaseHandler>()
                .AddSingleton<AudioService>()
                .BuildServiceProvider();

        } // end Constructor

        public async Task MainAsync()
        {
            await Startup();

            await _services.GetRequiredService<CommandHandler>().InitializeCommandsAsync();

            _client.Log += Log;

            _client.Ready += Ready;

            await _client.LoginAsync(TokenType.Bot, Variables.DiscordToken);
            await _client.StartAsync();
        } // end RunBotAsync

        public async Task Startup()
        {
            await Variables.LoadConfig();
        }

        private async Task Ready()
        {
            await _client.SetGameAsync(Variables.ProgramVersion);
            await _lavalinkManager.StartAsync();
            _services.GetRequiredService<DatabaseHandler>().SyncDatabase(); /////////////////////////
        }

        public static Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            return Task.CompletedTask;
        } // end Log(LogMessage)
        
    } // end class StartUp
}
