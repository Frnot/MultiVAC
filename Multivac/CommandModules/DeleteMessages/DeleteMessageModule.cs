using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Multivac.CommandModules
{
    public class DeleteMessageModule : ModuleBase<SocketCommandContext>
    {
        [Command("delete")]
        [RequireUserPermission(ChannelPermission.ManageMessages, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Delete(int numToDelete)
        {
            DeleteMessageService deleteservice = new DeleteMessageService();

            IEnumerable<IMessage> messages;

            messages = await deleteservice.DownloadMessagesAsync(Context, numToDelete);

            await deleteservice.DeleteMessages(Context, messages);
        }


        [Command("delete")]
        [RequireUserPermission(ChannelPermission.ManageMessages, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Delete(int numToDelete, [Remainder] string stringIn)
        {
            DeleteMessageService deleteservice = new DeleteMessageService();

            IEnumerable<IMessage> messages;

            await deleteservice.ParseDeleteCommandAsync(Context, stringIn);
            if (await deleteservice.CheckParamConflict())
            {
                await ReplyAsync("error: parameter conflict");
                return;
            }

            else messages = await deleteservice.DownloadMessagesAsync(Context, numToDelete);

            await deleteservice.DeleteMessages(Context, messages);

        } // end delete


        [Command("nuke")]
        [RequireUserPermission(ChannelPermission.ManageChannels, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Nuke()
        {
            var oldChannel = Context.Channel as ITextChannel;

            var newChannel = await Context.Guild.CreateTextChannelAsync(oldChannel.Name);

            await newChannel.ModifyAsync(nc =>
            {
                nc.Topic = oldChannel.Topic;
                nc.Position = oldChannel.Position;
                nc.CategoryId = oldChannel.CategoryId;
                nc.IsNsfw = oldChannel.IsNsfw;
            });

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

    }
}
