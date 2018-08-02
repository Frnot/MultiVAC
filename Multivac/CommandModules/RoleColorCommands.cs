using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Multivac.CommandModules
{
    public class RoleColorCommands : ModuleBase<SocketCommandContext>
    {
        [Command("rolecolor")]
        public async Task SetRoleColor(string input)
        {
            var user = Context.User as SocketGuildUser;
            var hexRegex = new Regex(@"(?i)#*[\da-f]{6}");

            if (hexRegex.IsMatch(input))
            {
                input = input.Replace("#", "").ToUpper();
                Console.WriteLine(input);

                if (Context.Guild.Roles.Any(r => r.Name == $"#{input}"))
                {
                    Console.WriteLine("role exists!");
                    var role = Context.Guild.Roles.First(r => r.Name == $"#{input}");
                    await user.AddRoleAsync(role);
                }
                else
                {
                    Console.WriteLine("role does not exists!");
                    var role = await Context.Guild.CreateRoleAsync($"#{input}", color: new Color(Convert.ToUInt32(input, 16)), isHoisted: false);

                    int newRolePos = user.Roles.Max(r => r.Position);
                    Console.WriteLine($"role pos {newRolePos}");

                    await Context.Guild.ReorderRolesAsync(new List<ReorderRoleProperties>() { new ReorderRoleProperties(role.Id, newRolePos + 1) });
                    await user.AddRoleAsync(role);
                }
            }
            else if (input.ToLower() == "list")
            {
                Console.WriteLine("listing");

                var roles = user.Roles.Where(r => hexRegex.IsMatch(r.Name));

                foreach (var role in roles)
                {
                    Console.WriteLine(role.Name);
                }
            }

            else if (input.ToLower() == "reset")
            {
                Console.WriteLine("resetting");

                var roles = user.Roles.Where(r => hexRegex.IsMatch(r.Name));

                await user.RemoveRolesAsync(roles);

                foreach (var role in roles.Where(r => r.Members.Count() == 0))
                {
                    await role.DeleteAsync();
                }
            }
        }
    }
}
