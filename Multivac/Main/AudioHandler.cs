using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Multivac.Utilities;
using SharpLink;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Multivac.Main
{
    public class AudioHandler
    {
        public AudioHandler(DiscordSocketClient client, LavalinkManager lavalinkManager)
        {
            _client = client;

            _lavalinkManager = lavalinkManager;

            GuildPlaylist = new ConcurrentDictionary<ulong, (IMessageChannel commandChannel, List< LavalinkTrack>, bool, bool)>();

            _lavalinkManager.TrackEnd += OnTrackEndAsync;
            _client.UserVoiceStateUpdated += VoiceStateUpdatedAsync;
        }

        private readonly DiscordSocketClient _client;
        private readonly LavalinkManager _lavalinkManager;

        private readonly ConcurrentDictionary<ulong, (IMessageChannel commandChannel, List<LavalinkTrack> Tracklist, bool PlayingMusicStatus, bool Repeat)> GuildPlaylist;


        public async Task JoinChannelAsync(SocketCommandContext Context)
        {
            var textChannel = Context.Channel;
            var voiceChannel = (Context.User as IVoiceState).VoiceChannel;

            if (voiceChannel == null)
            {
                await textChannel.SendMessageAsync("you must be in a voice channel");
                return;
            }

            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            if (player == null) player = await _lavalinkManager.JoinAsync(voiceChannel);

            GuildPlaylist.AddOrUpdate(Context.Guild.Id, (textChannel, new List<LavalinkTrack>(), false, false), (key, value) => value);

        }

        public async Task<Embed> PlayMusicAsync(SocketCommandContext Context)
        {
            if (!GuildPlaylist.TryGetValue(Context.Guild.Id, out var value)) await JoinChannelAsync(Context);
            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            value.PlayingMusicStatus = true;
            var track = value.Tracklist.First();

            var embed = new EmbedBuilder()
                .AddField("Playing Music",
                    $"**Title:** [{track.Title}]({track.Url})\n" +
                    $"**Length:** {track.Length}")
                .WithThumbnailUrl(ImageURL.YouTubeLogo)
                .WithColor(255, 255, 255)
                .Build();

            await player.PlayAsync(track);

            return embed;
        }

        private async Task OnTrackEndAsync(LavalinkPlayer player, LavalinkTrack track, string arg3)
        {
            var guildId = player.VoiceChannel.GuildId;

            GuildPlaylist.TryGetValue(guildId, out var value);

            if (!value.Repeat) value.Tracklist.Remove(track);
            else value.Repeat = false;

            if (!value.Tracklist.Any()) Console.WriteLine("queue empty"); //wait for more songs

            else await player.PlayAsync(value.Tracklist.First());
        }

        public async Task StopMusic(ulong guildId)
        {
            GuildPlaylist.TryGetValue(guildId, out var value);
            var player = _lavalinkManager.GetPlayer(guildId);

            await player.StopAsync();
            value.PlayingMusicStatus = false;
        }

        public async Task QueueAsync(string input, ulong guildId)
        {
            Console.WriteLine($"searching for {input}");

            var newtrack = await _lavalinkManager.GetTrackAsync($"ytsearch:{input}");
            Console.WriteLine($"done searching, found: {newtrack.Title}");

            GuildPlaylist.TryGetValue(guildId, out var value);
            value.Tracklist.Add(newtrack);
        }

        public async Task SkipSong(ulong guildId)
        {
            var player = _lavalinkManager.GetPlayer(guildId);
            GuildPlaylist.TryGetValue(guildId, out var value);

            await player.PauseAsync();
            value.Tracklist.Remove(value.Tracklist.First());
            await player.PlayAsync(value.Tracklist.First());
        }

        public async Task<Embed> RepeatSong(ulong guildId)
        {
            GuildPlaylist.TryGetValue(guildId, out var value);
            value.Repeat = true;

            var repeatTrack = value.Tracklist.First();

            return new EmbedBuilder().AddField("Repeating Song", $"Up next: [{repeatTrack.Title}]({repeatTrack.Url})").Build();
        }

        public async Task DisconnectAsync(ulong guildId)
        {
            var player = _lavalinkManager.GetPlayer(guildId);
            await player.DisconnectAsync();
        }

        public async Task SetVolumeAsync(ulong guildId, int volume)
        {
            var player = _lavalinkManager.GetPlayer(guildId);
            if (player == null) return;
            if (volume < 0 || volume > 150) return;
            await player.SetVolumeAsync((uint) volume);
            Console.WriteLine($"volume has been set to {volume}");
        }

        private async Task VoiceStateUpdatedAsync(SocketUser socketUser, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (socketUser.IsBot) return;
            var voiceChannel = oldState.VoiceChannel;

            if (voiceChannel.Users.Count(u => !u.IsBot) > 0) return;
            await DisconnectAsync(voiceChannel.Guild.Id);
        }
    } // end class AudioHandler
}
