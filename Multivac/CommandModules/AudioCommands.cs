using Discord;
using Discord.Commands;
using Multivac.Main;
using System.Threading.Tasks;

namespace Multivac.Commands
{
    public class AudioCommands : ModuleBase<SocketCommandContext>
    {
        private readonly AudioService _audioService;

        public AudioCommands(AudioService AudioService)
        {
            _audioService = AudioService;
        }
        // end Constructor


        [Command("join")]
        public async Task Join()
            => await _audioService.JoinChannelAsync(Context);

        [Command("play")]
        public async Task PlayMusic([Remainder] string input)
        {
            await _audioService.PlayMusicAsync(Context, input);
        }

        [Command("stop")]
        [RequireOwner]
        public async Task StopMusic()
            => await _audioService.StopMusicAsync(Context.Guild.Id);

        [Command("restart")]
        public async Task RestartSong()
            => await _audioService.RestartSongAsync(Context);
        

        [Command("skip"), Alias("next")]
        public async Task SkipSong()
            => await _audioService.SkipSongAsync(Context);

        [Command("loop"), Alias("repeat")]
        public async Task RepeatSong()
            => await _audioService.RepeatSongAsync(Context.Guild.Id);

        [Command("leave")]
        public async Task Leave()
            => await _audioService.DisconnectAsync(Context.Guild.Id);



        [Command("nowplaying", RunMode = RunMode.Async), Alias("np", "currentsong", "currenttrack")]
        public async Task NowPlaying()
            => await _audioService.NowPlayingAsync(Context);

        [Command("upnext"), Alias("nextsong", "nexttrack")]
        public async Task UpNext()
            => await _audioService.UpNextAsync(Context);

        [Command("showqueue"), Alias("songlist", "songs")]
        public async Task ShowTrackList()
            => await _audioService.ShowQueueAsync(Context);

        [Command("volume")]
        public async Task Volume()
            => await _audioService.ShowVolumeAsync(Context);

        [Command("volume")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Volume(int volume)
            => await _audioService.SetVolumeAsync(Context.Guild.Id, volume);
    }
}
