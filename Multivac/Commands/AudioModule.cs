using Discord;
using Discord.Commands;
using Multivac.Main;
using System.Threading.Tasks;

namespace Multivac.Commands
{
    public class AudioCommands : ModuleBase<SocketCommandContext>
    {
        private readonly AudioHandler AudioHandler;

        public AudioCommands(AudioHandler AudioHandler)
        {
            this.AudioHandler = AudioHandler;
        }

        //private LavalinkPlayer _player;
        //private LavalinkTrack track;

        [Command("play")]
        public async Task PlayMusic([Remainder] string input)
        {
            await Join();
            await QueueSong(input);
            await PlayMusic();
        }

        [Command("play")]
        public async Task PlayMusic()
        {
            await ReplyAsync(embed: await AudioHandler.PlayMusicAsync(Context.Guild.Id));
        }

        [Command("queue"), Alias("add")]
        public async Task QueueSong([Remainder] string input)
        {
            await AudioHandler.QueueAsync(input, Context.Guild.Id);
        }

        [Command("skip")]
        public async Task SkipSong()
        {
            await AudioHandler.SkipSong(Context.Guild.Id);
        }


        [Command("join")]
        public async Task Join()
        {
            await AudioHandler.JoinChannelAsync(Context);

        }

        [Command("leave")]
        public async Task Leave()
        {
            await AudioHandler.DisconnectAsync(Context.Guild.Id);
        }

        [Command("volume")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Volume(int volume)
        {
            await AudioHandler.SetVolumeAsync(Context.Guild.Id, volume);
        }

    }
}
