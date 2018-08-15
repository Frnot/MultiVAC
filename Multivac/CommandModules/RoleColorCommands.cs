using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Multivac.CommandModules
{
    public class RoleColorCommands : ModuleBase<SocketCommandContext>
    {
        public static Regex HexRegex = new Regex(@"(?i)#*[\da-f]{6}");

        [Command("rolecolor")]
        public async Task SetRoleColor(string input)
        {
            var user = Context.User as SocketGuildUser;

            //check if guild is allowed

            if (user.Roles.Max(r => r.Position) > Context.Guild.CurrentUser.Roles.Max(r => r.Position))
            {
                await ReplyAsync(embed: new EmbedBuilder().AddField("Error: lack required permissions", "I cannot manipulate roles above mine").Build());
                return;
            }

            if (HexRegex.IsMatch(input))
            {
                await RemoveColor(user);
                input = input.Replace("#", "").ToUpper();
                Console.WriteLine(input);

                var addedRole = await AddRole(user, input);

                await PromoteColorRole(user, addedRole);
            }
            
            else if (input.ToLower() == "clear")
            {
                await RemoveColor(user);
            }

            else
            {
                await ReplyAsync("error: please enter a color in hex format or `clear` to clear role color");
            }
        } // end rolecolor


        public async Task<IRole> AddRole(SocketGuildUser user, string input)
        {
            if (Context.Guild.Roles.Any(r => r.Name == $"#{input}"))
            {
                Console.WriteLine("role exists!");
                var role = Context.Guild.Roles.First(r => r.Name == $"#{input}");
                await user.AddRoleAsync(role);

                return role;
            }
            else
            {
                var role = await Context.Guild.CreateRoleAsync($"#{input}", new GuildPermissions(), color: new Color(Convert.ToUInt32(input, 16)), isHoisted: false);
                await user.AddRoleAsync(role);

                return role;
            }
        } // end AddRole()

        private async Task PromoteColorRole(SocketGuildUser user, IRole colorRole)
        {
            var maxRolePos = user.Roles.Max(r => r.Position);
            var colorRolePos = colorRole.Position;

            if (colorRolePos < maxRolePos)
            {
                await Context.Guild.ReorderRolesAsync(new List<ReorderRoleProperties>() { new ReorderRoleProperties(colorRole.Id, maxRolePos + 1) });
            }
        } // end CompareHeirarchy()

        public async Task RemoveColor(SocketGuildUser user)
        {
            var role = user.Roles.Where(r => HexRegex.IsMatch(r.Name)).FirstOrDefault();

            if (role == null) return;

            if (role.Members.Count() <= 1)
            {
                await role.DeleteAsync();
            }
            else
            {
                await user.RemoveRoleAsync(role);
            }
        } // end RemoveColor()

    }
}
