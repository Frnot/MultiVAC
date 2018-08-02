using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Multivac.CommandModules
{
    public static class ParseDiscordUser
    {
        private static readonly Regex MentionPattern = new Regex("<@([!]?[0-9]*)>");
        private static readonly Regex IdPattern = new Regex("[0-9]{15,20}");


        public static List<SocketGuildUser> FromAny(SocketGuild guild, string input)
        {
            List<SocketGuildUser> userList = new List<SocketGuildUser>();

            userList.AddRange(FromMention(guild, input));
            userList.AddRange(FromID(guild, input));

            if (userList.Any()) return userList;

            return null;
        }

        public static List<SocketGuildUser> FromMention(SocketGuild guild, string input)
        {
            if (!MentionPattern.IsMatch(input)) return null;

            var matches = MentionPattern.Matches(input);

            var users = matches.Select(match => guild.GetUser(Convert.ToUInt64(Regex.Replace(match.ToString(), "[<@!>]", ""))));

            return users.ToList();
        }

        public static List<SocketGuildUser> FromID(SocketGuild guild, string input)
        {
            if (!IdPattern.IsMatch(input)) return null;

            var matches = IdPattern.Matches(input);

            var users = matches.Select(match => guild.GetUser(Convert.ToUInt64(match.ToString())));

            return users.ToList();
        }

        public static List<SocketGuildUser> FromUsername(SocketGuild guild, string input)
        {
            input = input.ToLower();

            StringComparison comp = StringComparison.OrdinalIgnoreCase;

            var users = guild.Users.Where(u => input.Contains(u.Username.ToLower(), comp) || input.Contains(u.Nickname.ToLower(), comp));

            return users.ToList();
        }

    }
}
