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
        // end Constructor


        [Command("join")]
        public async Task Join()
            => await AudioHandler.JoinChannelAsync(Context);

        [Command("play")]
        public async Task PlayMusic([Remainder] string input)
        {
            await AudioHandler.PlayMusicAsync(Context, input);
        }

        [Command("stop")]
        public async Task StopMusic()
            => await AudioHandler.StopMusic(Context.Guild.Id);
        

        

        [Command("skip")]
        public async Task SkipSong()
            => await AudioHandler.SkipSong(Context);

        [Command("loop"), Alias("repeat")]
        public async Task RepeatSong()
            => await AudioHandler.RepeatSong(Context.Guild.Id);


        

        [Command("leave")]
        public async Task Leave()
            => await AudioHandler.DisconnectAsync(Context.Guild.Id);






        [Command("showqueue"), Alias("songlist", "songs")]
        public async Task ShowTrackList([Remainder] string input)
        {
            //todo
        }

        [Command("volume")]
        //[RequireUserPermission(GuildPermission.Administrator, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Volume(int volume)
            => await AudioHandler.SetVolumeAsync(Context.Guild.Id, volume);

    }
}
