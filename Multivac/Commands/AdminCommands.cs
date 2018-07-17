using Discord.Commands;
using Discord;
using System;
using System.Threading.Tasks;
using LiteDB;
using Multivac.Data;

namespace Multivac.Modules
{
    public class AdminCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DatabaseHandler _databaseHandler;

        public AdminCommands(DatabaseHandler databaseHandler)
        {
            _databaseHandler = databaseHandler;
        }


        [Command("nuke")]
        [RequireUserPermission(ChannelPermission.ManageChannels, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Nuke()
        {
            var oldChannel = Context.Channel as ITextChannel;

            var hm = new RequestOptions();
            hm.AuditLogReason = "test";

            var newChannel = await Context.Guild.CreateTextChannelAsync(oldChannel.Name, nc =>
            {
                nc.Topic = oldChannel.Topic;
                nc.Position = oldChannel.Position;
                nc.CategoryId = oldChannel.CategoryId;
                nc.IsNsfw = oldChannel.IsNsfw;
            }, hm);

            foreach (var perm in oldChannel.PermissionOverwrites)
            {
                switch (perm.TargetType)
                {
                    case PermissionTarget.Role:
                        await newChannel.AddPermissionOverwriteAsync(Context.Guild.GetRole(perm.TargetId), perm.Permissions);
                        break;
                    case PermissionTarget.User:
                        await newChannel.AddPermissionOverwriteAsync(Context.Guild.GetUser(perm.TargetId), perm.Permissions);
                        break;
                }
            }

            await oldChannel.DeleteAsync();
        } // end Nuke

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.Administrator, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task ChangePrefix(string newPrefix)
        {
            using (var db = new LiteDatabase(@"GuildData.db"))
            {
                if (string.IsNullOrEmpty(newPrefix)) return;

                var guildList = db.GetCollection<GuildData>("guilds");
                var thisGuild = guildList.FindOne(x => x.GuildId == Context.Guild.Id);

                if (newPrefix.ToLower().Equals("reset"))
                {
                    thisGuild.CommandPrefix = "";
                    await ReplyAsync($"the command prefix has been reset.");
                }
                else
                {
                    thisGuild.CommandPrefix = newPrefix;
                    await ReplyAsync($"the command prefix has been changed.");
                }
                guildList.Update(thisGuild);

                await ReplyAsync($"the command prefix for {Context.Guild} is now `{_databaseHandler.GetGuildPrefix(Context.Guild.Id)}`");
            }
        } // end ChangePrefix(string)

        

        [Command("spam")]
        [RequireOwner]
        public async Task Spam(int input)
        { //used for testing, not for evil
            for (int i = 0; i < input; i++)
            {
                await ReplyAsync((i + 1).ToString());
            }
        }

        [Command("test")]
        [RequireOwner]
        public async Task Test([Remainder] string input)
        {
        }


        [Command("readtest")]
        [RequireOwner]
        public async Task ReadTest()
        {
            Console.WriteLine("running ReadTest");
            using (var db = new LiteDatabase(@"GuildData.db"))
            {
                var guilds = db.GetCollection<GuildData>("guilds");
                foreach (var guild in guilds.FindAll())
                {
                    Console.WriteLine($"ID: {guild.Id}" +
                        $"Guild ID: {guild.GuildId}" +
                        $"Guild Prefix: {guild.CommandPrefix}");
                }
            }
        }
    } // end class AdminCommands
}
