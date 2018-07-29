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
        public AudioService _audioService { get; set; }

        [Command("die")]
        [RequireOwner]
        public async Task Stop()
        {
            await _audioService.DisconAllPlayersAsync();
            await _client.SetStatusAsync(UserStatus.Invisible);
            await _client.StopAsync();
            Program.Shutdown();
        }

        [Command("undie")]
        [RequireOwner]
        public async Task Restart()
        {
            await _audioService.DisconAllPlayersAsync();
            await _client.SetStatusAsync(UserStatus.Invisible);
            await _client.StopAsync();
            Program.Restart();
        }
    }
}
