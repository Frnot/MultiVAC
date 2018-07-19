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
    public partial class AudioHandler
    {
        public AudioHandler(DiscordSocketClient client, LavalinkManager lavalinkManager)
        {
            _client = client;

            _lavalinkManager = lavalinkManager;

            GuildPlaylist = new ConcurrentDictionary<ulong, (IMessageChannel boundChannel, List<(LavalinkTrack, string, ulong)>, bool)>();

            _lavalinkManager.TrackEnd += OnTrackEndAsync;
            //_client.UserVoiceStateUpdated += VoiceStateUpdatedAsync;
        }

        private readonly DiscordSocketClient _client;
        private readonly LavalinkManager _lavalinkManager;

        private readonly ConcurrentDictionary<ulong, (IMessageChannel boundChannel, List<(LavalinkTrack track, string platform, ulong requesterId)> Tracklist, bool Repeat)> GuildPlaylist;


        public async Task<bool> JoinChannelAsync(SocketCommandContext Context)
        {
            var boundChannel = Context.Channel;
            var voiceChannel = (Context.User as IVoiceState).VoiceChannel;

            if (voiceChannel == null)
            {
                await boundChannel.SendMessageAsync("you must be in a voice channel");
                return false;
            }

            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            if (player == null) player = await _lavalinkManager.JoinAsync(voiceChannel);

            GuildPlaylist.AddOrUpdate(Context.Guild.Id, (boundChannel, new List<(LavalinkTrack, string, ulong)>(), false), (key, value) => value);

            return true;
        }

        public async Task PlayMusicAsync(SocketCommandContext Context, string input)
        {
            if (!GuildPlaylist.ContainsKey(Context.Guild.Id) || _lavalinkManager.GetPlayer(Context.Guild.Id) == null)
            {
                if (!await JoinChannelAsync(Context)) return;
            }

            GuildPlaylist.TryGetValue(Context.Guild.Id, out var value);
            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            string searchplatform = "ytsearch";

            await value.boundChannel.SendMessageAsync($"Searching for: \"{input}\"");
            var newTrack = await _lavalinkManager.GetTrackAsync($"{searchplatform}:{input}");

            value.Tracklist.Add((newTrack, "youtube", Context.User.Id));

            if (player.CurrentTrack != null)
            {
                await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                    .AddField("Added song to queue",
                        $"**Title:** [{newTrack.Title}]({newTrack.Url})\n" +
                        $"**Length:** {newTrack.Length}")
                    .WithThumbnailUrl(ImageURL.YouTubeLogo)
                    .WithColor(255, 255, 255)
                    .Build());
            }
            else
            {
                await player.PlayAsync(value.Tracklist.First().track);
                await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                    .AddField("Playing Music",
                        $"**Title:** [{newTrack.Title}]({newTrack.Url})\n" +
                        $"**Length:** {newTrack.Length}")
                    .WithThumbnailUrl(ImageURL.YouTubeLogo)
                    .WithColor(255, 255, 255)
                    .Build());
            }
        }

        private async Task OnTrackEndAsync(LavalinkPlayer player, LavalinkTrack track, string _)
        {

            var guildId = player.VoiceChannel.GuildId;

            GuildPlaylist.TryGetValue(guildId, out var value);

            if (!value.Repeat) value.Tracklist.Remove(value.Tracklist.First());

            if (!value.Tracklist.Any()) Console.WriteLine("queue empty"); //wait for more songs

            else await player.PlayAsync(value.Tracklist.First().track);
        }

        public async Task StopMusicAsync(ulong guildId)
        {
            GuildPlaylist.TryGetValue(guildId, out var value);
            var player = _lavalinkManager.GetPlayer(guildId);

            await player.StopAsync();
        }

        public async Task RestartSongAsync(SocketCommandContext Context)
        {
            if (!IsMusicPlaying(Context.Guild.Id))
            {
                await Context.Channel.SendMessageAsync("Error: nothing is playing");
                return;
            }

            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);
            GuildPlaylist.TryGetValue(Context.Guild.Id, out var value);

            await player.PauseAsync();
            await player.PlayAsync(value.Tracklist.First().track);
        }

        public async Task SkipSongAsync(SocketCommandContext Context)
        {
            if (!IsMusicPlaying(Context.Guild.Id))
            {
                await Context.Channel.SendMessageAsync("Error: nothing is playing");
                return;
            }

            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);
            GuildPlaylist.TryGetValue(Context.Guild.Id, out var value);

            await player.PauseAsync();
            value.Tracklist.Remove(value.Tracklist.First());
            await player.PlayAsync(value.Tracklist.First().track);
        }

        public async Task RepeatSongAsync(ulong guildId)
        {
            GuildPlaylist.TryGetValue(guildId, out var value);
            value.Repeat = true;

            var repeatTrack = value.Tracklist.First().track;

            await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder().AddField("Repeating Song", $"Up next: [{repeatTrack.Title}]({repeatTrack.Url})").Build());
        }

        private bool IsMusicPlaying(ulong guildId)
        {
            var player = _lavalinkManager.GetPlayer(guildId);
            if (player == null || !GuildPlaylist.ContainsKey(guildId)) return false;
            return player.Playing;
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
