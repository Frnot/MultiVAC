using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Multivac.AudioModule;
using Multivac.Utilities;
using SharpLink;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Multivac.Main
{
    public partial class AudioHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly LavalinkManager _lavalinkManager;
        private readonly ConcurrentDictionary<ulong, (IMessageChannel boundChannel, Queue<(LavalinkTrack track, ulong requesterId)> Tracklist, bool IsPlaying, bool Repeat)> GuildPlaylist;

        public AudioHandler(DiscordSocketClient client, LavalinkManager lavalinkManager)
        {
            _client = client;

            _lavalinkManager = lavalinkManager;

            GuildPlaylist = new ConcurrentDictionary<ulong, (IMessageChannel boundChannel, Queue<(LavalinkTrack, ulong)>, bool, bool)>();

            _lavalinkManager.TrackEnd += OnTrackEndAsync;
            _client.UserVoiceStateUpdated += VoiceStateUpdatedAsync;
            _client.UserVoiceStateUpdated += AutoUnmuteAsync;
        } // end constructor



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

            GuildPlaylist.AddOrUpdate(Context.Guild.Id, (boundChannel, new Queue<(LavalinkTrack, ulong)>(), false, false), (key, value) => value);

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


            VideoPlatformTools.PlatformSearchSelector(input, out var siteSpecifier, out var searchTerm);


            await value.boundChannel.SendMessageAsync($"Searching for: \"{input}\"");
            Console.WriteLine($"searching for exactly: {siteSpecifier}{searchTerm}");

            var newTrack = await _lavalinkManager.GetTrackAsync($"{siteSpecifier}{searchTerm}");

            value.Tracklist.Enqueue((newTrack, Context.User.Id));

            if (value.IsPlaying)
            {
                await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                    .AddField("Added song to queue",
                        $"**Title:** [{newTrack.Title}]({newTrack.Url})\n" +
                        $"**Length:** {newTrack.Length}")
                    //.WithThumbnailUrl(ImageURL.YouTubeLogo)
                    .WithColor(255, 255, 255)
                    .Build());
            }
            else
            {
                value.IsPlaying = true;
                GuildPlaylist.AddOrUpdate(Context.Guild.Id, value, (key, val) => value);

                await player.PlayAsync(value.Tracklist.Dequeue().track);

                await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                    .AddField("Playing Music",
                        $"**Title:** [{newTrack.Title}]({newTrack.Url})\n" +
                        $"**Length:** {newTrack.Length}")
                    //.WithThumbnailUrl(ImageURL.YouTubeLogo)
                    .WithColor(255, 255, 255)
                    .Build());
            }
        }

        private async Task OnTrackEndAsync(LavalinkPlayer player, LavalinkTrack track, string _)
        {
            Console.WriteLine("enter OntrackEndAsync");

            var guildId = player.VoiceChannel.GuildId;

            GuildPlaylist.TryGetValue(guildId, out var value);

            if (value.Repeat)
            {
                await value.boundChannel.SendMessageAsync($"repeating {track.Title}");
                await player.PlayAsync(track);
            }
            else if (!value.Tracklist.Any())
            {
                await value.boundChannel.SendMessageAsync("no more songs in queue");
                value.IsPlaying = false;
                GuildPlaylist.AddOrUpdate(guildId, value, (key, val) => value);
            }
            else await player.PlayAsync(value.Tracklist.Dequeue().track);
        }

        public async Task StopMusicAsync(ulong guildId)
        {
            GuildPlaylist.TryGetValue(guildId, out var value);
            var player = _lavalinkManager.GetPlayer(guildId);

            await player.StopAsync();
            value.Tracklist.Clear();
            value.IsPlaying = false;
            GuildPlaylist.AddOrUpdate(guildId, value, (key, val) => value);
        }

        public async Task RestartSongAsync(SocketCommandContext Context)
        {
            if (!IsMusicPlaying(Context.Guild.Id))
            {
                await Context.Channel.SendMessageAsync("Error: nothing is playing");
                return;
            }

            var player = _lavalinkManager.GetPlayer(Context.Guild.Id);

            var currentTrack = player.CurrentTrack;

            await player.PauseAsync();
            await player.PlayAsync(currentTrack);
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

            await player.StopAsync();
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
            GuildPlaylist.TryGetValue(guildId, out var value);

            if (player == null || !GuildPlaylist.ContainsKey(guildId)) return false;

            return value.IsPlaying;
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

        private async Task AutoUnmuteAsync(SocketUser socketUser, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if ((socketUser.Id == _client.CurrentUser.Id) && (socketUser as IGuildUser).IsMuted)
            {
                Console.WriteLine("fire");
                await (socketUser as IGuildUser).ModifyAsync(x => { x.Mute = false; x.Deaf = false; });
            }
        }

    } // end class AudioHandler
}
