using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Multivac.Main;
using System.Threading.Tasks;

namespace Multivac.Commands
{
    public class OwnerCommands : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient _client { get; set; }

        [Command("stop")]
        [RequireOwner]
        public async Task Stop()
        {
            await _client.SetStatusAsync(UserStatus.Invisible);
            await _client.StopAsync();
            Program.Shutdown();
        }

        [Command("restart")]
        [RequireOwner]
        public async Task Restart()
        {
            await _client.StopAsync();
            await RunBot.Instance.Startup();
            await _client.StartAsync();
        }
    }
}
