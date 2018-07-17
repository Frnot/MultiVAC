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

            GuildPlaylist = new ConcurrentDictionary<ulong, (List<LavalinkTrack> Tracklist, bool Repeat)>();

            _lavalinkManager.TrackEnd += OnTrackEndAsync;
            _client.UserVoiceStateUpdated += VoiceStateUpdatedAsync;
        }

        private readonly DiscordSocketClient _client;
        private readonly LavalinkManager _lavalinkManager;

        private readonly ConcurrentDictionary<ulong, (List<LavalinkTrack> Tracklist, bool Repeat)> GuildPlaylist;

        public async Task JoinChannelAsync(SocketCommandContext Context)
        {
            var voiceChannel = (Context.User as IVoiceState).VoiceChannel;

            if (voiceChannel == null)
            {
                await Context.Channel.SendMessageAsync("you must be in a voice channel");
                return;
            }

            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            if (player == null) player = await _lavalinkManager.JoinAsync(voiceChannel);

            GuildPlaylist.AddOrUpdate(Context.Guild.Id, (new List<LavalinkTrack>(), false), (key, value) => value);

        }

        public async Task<Embed> PlayMusicAsync(ulong guildId)
        {
            GuildPlaylist.TryGetValue(guildId, out var value);

            var player = _lavalinkManager.GetPlayer(guildId);

            
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

            if (!value.Tracklist.Any()) await DisconnectAsync(guildId);
            else await PlayMusicAsync(guildId);
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
            await PlayMusicAsync(guildId);
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
