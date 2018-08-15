using Discord;
using Discord.Commands;
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

        //put nuke here//////////////////////////////////////////////////////////
        
    }
}
