using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Multivac.Data;
using Multivac.Main;
using Multivac.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Multivac.Commands
{
    public class InfoCommands : ModuleBase<SocketCommandContext>
    {
        public CommandHandler _commandHandler { get; set; }

        [Command("serverinfo")]
        [Alias("guildinfo")]
        public async Task GuildInfo()
        {
            var guild = Context.Guild;
            var embed = new EmbedBuilder();
            DateTimeConversion.VoiceRegionToTimeZone(Context.Guild.VoiceRegionId, out string timezone, out int offset);

            embed.WithTitle($"Server Info for **{guild.Name}**");
            embed.ThumbnailUrl = guild.IconUrl;
            embed.AddField(x =>
            {
                x.Name = "Server Age";
                x.Value = $"{DateTimeConversion.YearsSince(guild.CreatedAt)}, {DateTimeConversion.DaysSince(guild.CreatedAt)} old\n" +
                          $"This server was created on {guild.CreatedAt.DateTime.AddHours(offset).ToShortDateString()} at {guild.CreatedAt.DateTime.AddHours(offset).ToShortTimeString()} ({timezone})";
            });
            embed.AddField(x =>
            {
                x.Name = "Population";
                x.Value = $"Online: {guild.Users.Where(u => u.Status == UserStatus.Online || u.Status == UserStatus.Idle || u.Status == UserStatus.AFK || u.Status == UserStatus.DoNotDisturb).Count()} " +
                          $"({TextManipulation.Pluralize("bot", "bots", guild.Users.Where(u => (u.Status == UserStatus.Online || u.Status == UserStatus.Idle || u.Status == UserStatus.AFK || u.Status == UserStatus.DoNotDisturb) && u.IsBot).Count())})" +
                          $"\nTotal:   {guild.MemberCount} ({TextManipulation.Pluralize("bot", "bots", guild.Users.Where(u => u.IsBot).Count())})";
            });
            embed.AddField(x =>
            {
                x.Name = "Bot Specific Server Info";
                x.Value = $"Bot added: {Context.Guild.CurrentUser.JoinedAt.Value.DateTime.ToShortDateString()}\n" + //how long bot has been in this guild
                          $"Command Prefix: `{_commandHandler.GetGuildPrefix(guild.Id)}`\n";
            });

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        } // end GuildInfo



        [Command("userinfo")]
        [Alias("user")]
        public async Task UserInfo(IUser userIn = null)
        {

            DateTimeConversion.VoiceRegionToTimeZone(Context.Guild.VoiceRegionId, out string timezone, out int offset);


            SocketGuildUser user = (SocketGuildUser)userIn ?? (SocketGuildUser)Context.User;

            var embed = new EmbedBuilder();

            embed.WithTitle($"{user.Username}#{user.Discriminator}'s Info:");
            embed.ThumbnailUrl = user.GetAvatarUrl();

            if (user.Nickname != null)
            {
                embed.AddField(x =>
                {
                    x.Name = "Nickname";
                    x.Value = $"{user.Nickname}";
                });
            }

            embed.AddField(x =>
            {
                x.Name = "Status";
                x.Value = user.Status;
            });
            embed.AddField(x =>
            {
                x.Name = "Account Created";
                DateTime created = user.CreatedAt.UtcDateTime.AddHours(offset);
                x.Value = $"{created.ToShortDateString()} at {created.ToShortTimeString()} ({timezone})";
            });
            embed.AddField(x =>
            {
                x.Name = "Joined";
                DateTime joindate = ((DateTimeOffset)user.JoinedAt).UtcDateTime.AddHours(offset);
                x.Value = $"{joindate.ToShortDateString()} at {joindate.ToShortTimeString()} ({timezone})";
            });
            embed.AddField(x =>
            {
                x.Name = "Roles";
                x.Value = string.Join(", ", user.Roles);
            });
            embed.WithFooter($"ID: {user.Id}");

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        } // end UserInfo

        [Command("whojoined")]
        public async Task WhoJoined()
        {
            var userlist = Context.Guild.Users.OrderByDescending(u => u.JoinedAt);
            await ReplyAsync(embed: new EmbedBuilder
            {
                Title = "Last five users to join this server:",
                Description = $"{userlist.ElementAt(0)}\n" +
                    $"{userlist.ElementAt(1)}\n" +
                    $"{userlist.ElementAt(2)}\n" +
                    $"{userlist.ElementAt(3)}\n" +
                    $"{userlist.ElementAt(4)}"
            }.Build());
        }

        [Command("sysinfo")]
        public async Task SystemInfo()
        {
            var embed = new EmbedBuilder();

            embed.WithAuthor(x =>
            {
                x.Name = $"MultiVAC {Variables.ProgramVersion} System Information";
                x.Url = "https://github.com/Frnot/MultivacBot";
                x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl();
            });
            embed.ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl();
            embed.AddField(x =>
            {
                x.Name = "Library";
                x.Value = $"Discord.Net {DiscordConfig.Version}";
            });
            embed.AddField(x =>
            {
                x.Name = "Runtime";
                x.Value = $"{RuntimeInformation.FrameworkDescription} {RuntimeInformation.ProcessArchitecture}";
            });
            embed.AddField(x =>
            {
                x.Name = "Uptime";
                x.Value = (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"d\:hh\:mm\:ss");
            });
            embed.AddField(x =>
            {
                x.Name = "Host OS";
                x.Value = $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}";
            });
            embed.AddField(x =>
            {
                x.Name = "System Uptime";
                x.Value = (TimeSpan.FromMilliseconds(Environment.TickCount)).ToString(@"d\:hh\:mm\:ss");
            });

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        } // end SystemInfo

        [Command("botinfo")]
        public async Task BotInfo()
        {
            var embed = new EmbedBuilder();

            embed.WithAuthor(x =>
            {
                x.Name = $"MultiVAC version {Variables.ProgramVersion}";
                x.Url = "https://discordapp.com/oauth2/authorize?client_id=453744121845383175&scope=bot&permissions=536341840";
                x.IconUrl = Context.Client.CurrentUser.GetAvatarUrl();
            });

            embed.WithDescription($"[Invite Link](https://discordapp.com/oauth2/authorize?client_id=453744121845383175&scope=bot&permissions=536341840)\n" +
                $"[Origin of MultiVAC](https://en.wikipedia.org/wiki/Multivac)\n" +
                $"Created by Frnot");

            embed.WithFooter("ONLINE: 26.05.2018•01:34");

            await Context.Channel.SendMessageAsync("", embed: embed.Build());
        }

        [Command("latency", RunMode = RunMode.Async)]
        [Alias("ping", "pong", "rtt")]
        [Summary("Returns the current estimated round-trip latency over WebSocket")]
        //I didnt write this. I have no clue how it works. Im not even sure if it works correctly.
        public async Task Latency()
        {
            ulong target = 0;
            CancellationTokenSource cts = new CancellationTokenSource();

            Task WaitTarget(SocketMessage message)
            {
                if (message.Id != target) return Task.CompletedTask;
                cts.Cancel();
                return Task.CompletedTask;
            }

            var latency = Context.Client.Latency;
            var s = Stopwatch.StartNew();
            var m = await ReplyAsync($"heartbeat: {latency}ms, init: ---, rtt: ---");
            var init = s.ElapsedMilliseconds;
            target = m.Id;
            s.Restart();
            Context.Client.MessageReceived += WaitTarget;

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), cts.Token);
            }
            catch (TaskCanceledException)
            {
                var rtt = s.ElapsedMilliseconds;
                s.Stop();
                await m.ModifyAsync(x => x.Content = $"heartbeat: {latency}ms, init: {init}ms, rtt: {rtt}ms");
                return;
            }
            finally
            {
                Context.Client.MessageReceived -= WaitTarget;
            }
            s.Stop();
            await m.ModifyAsync(x => x.Content = $"heartbeat: {latency}ms, init: {init}ms, rtt: timeout");
        } // end Ping

    }
}
