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
        public async Task NowPlayingAsync(SocketCommandContext Context)
        {
            ulong guildId = Context.Guild.Id;

            if (!IsMusicPlaying(guildId))
            {
                await Context.Channel.SendMessageAsync("nothing is playing");
                return;
            }

            GuildPlaylist.TryGetValue(guildId, out var value);
            var track = value.Tracklist.First().track;
            var requester = _client.GetUser(value.Tracklist.First().requesterId);

            await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                .AddField("Now Playing",
                    $"**Title:** [{track.Title}]({track.Url})\n" +
                    $"**Artist:** {track.Author}\n" +
                    $"**Length:** {track.Length}\n" +
                    $"**Requested by:** {requester.Mention}")
                .Build());
        }

        public async Task UpNextAsync(SocketCommandContext Context)
        {
            ulong guildId = Context.Guild.Id;

            if (!IsMusicPlaying(guildId))
            {
                await Context.Channel.SendMessageAsync("nothing is playing");
                return;
            }

            GuildPlaylist.TryGetValue(guildId, out var value);
            var nextTrack = value.Tracklist.ElementAt(1).track;
            var requester = _client.GetUser(value.Tracklist.First().requesterId);

            await value.boundChannel.SendMessageAsync(embed: new EmbedBuilder()
                .AddField("Now Playing",
                    $"**Title:** [{nextTrack.Title}]({nextTrack.Url})\n" +
                    $"**Artist:** {nextTrack.Author}\n" +
                    $"**Length:** {nextTrack.Length}\n" +
                    $"**Requested by:** {requester.Mention}")
                .Build());
        }


    }
}
