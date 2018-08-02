using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Multivac.Utilities
{
    public static class MentionParser
    {
        //deprecated

/*
        public static SocketGuildUser GetUserFromString(SocketGuild guild, string input)
        {
            var mentionUser = UserFromMention(guild, input).First();
            if (mentionUser != null) return mentionUser;

            var idUser = UserFromID(guild, input).First();
            if (idUser != null) return idUser;

            var nameUser = UserFromName(guild, input).First();
            if (nameUser != null) return nameUser;

            return null;
        } // end GetUserFromString

        public static List<SocketGuildUser> GetUsersFromString(SocketGuild guild, string input)
        {
            var mentionUser = UserFromMention(guild, input);
            if (mentionUser != null) return mentionUser;

            var idUser = UserFromID(guild, input);
            if (idUser != null) return idUser;

            var nameUser = UserFromName(guild, input);
            if (nameUser != null) return nameUser;

            return null;
        } // end GetUsersFromString


        public static List<SocketGuildUser> UserFromMention(SocketGuild guild, string input)
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            Regex mentionPattern = new Regex("<@([!]?[0-9]*)>");

            if (mentionPattern.IsMatch(input))
            {
                var matches = mentionPattern.Matches(input).ToList();
                foreach (var match in matches)
                {
                    users.Add(guild.GetUser(Convert.ToUInt64(Regex.Replace(match.ToString(), "[<@!>]", ""))));
                }
                return users;
            }
            else return null;
        }

        public static List<SocketGuildUser> UserFromID(SocketGuild guild, string input)
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            Regex idPattern = new Regex("[0-9]{15,20}");

            if (idPattern.IsMatch(input))
            {
                var matches = idPattern.Matches(input).ToList();
                foreach (var match in matches)
                {
                    users.Add(guild.GetUser(Convert.ToUInt64(match.ToString())));
                }
                return users;
            }
            else return null;
        }

        public static List<SocketGuildUser> UserFromName(SocketGuild guild, string input)
        {
            List<SocketGuildUser> users = new List<SocketGuildUser>();

            foreach (var user in guild.Users)
            {
                string name = String.IsNullOrEmpty(user.Nickname) ? user.Username : user.Nickname;
                if (input.ToLower().Contains(name.ToLower())) users.Add(user);
            }

            if (users.Count == 1) return null;
            else return users;
        }
        */
    }
}
