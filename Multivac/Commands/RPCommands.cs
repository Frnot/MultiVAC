using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Multivac.Modules
{
    public class RPCCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        public RPCCommands(DiscordSocketClient client)
        {
            _client = client;
        }

        //[Command("startgame")]
        public async Task StartGame()
        {
            //var game = new RPCMain(_client);
            //game.StartGame(Context);

        }

    }
}
