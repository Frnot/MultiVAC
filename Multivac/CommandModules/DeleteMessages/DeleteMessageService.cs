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
    class DeleteMessageService
    {
        private string InfoMessage;
        private bool filterMessages = false;

        private bool findUser = false;
        private List<SocketGuildUser> userAuthors = new List<SocketGuildUser>();

        private bool findRole = false;
        private List<SocketRole> roleAuthors = new List<SocketRole>();

        private bool findString { get; set; }
        private List<string> phrases = new List<string>();

        private bool findBots = false;
        private bool findHumans = false;
        private bool findText = false;
        private bool findMentions = false;
        private bool findEmbeds = false;
        private bool findPins = false;
        private bool findEmojis = false;
        private bool findImages = false;
        private bool findLinks = false;

        private Direction _direction = Direction.Before;
        private IMessage commandMessage;



        public async Task ParseDeleteCommandAsync(SocketCommandContext Context, string input)
        {
            commandMessage = (await Context.Channel.GetMessagesAsync(1).FlattenAsync()).First();

            var options = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "bots",
                "humans",
                "text",
                "mentions",
                "embeds",
                "pins",
                "emojis",
                "images",
                "links",
                "before",
                "after"
            };

            string message = input.ToLower();

            userAuthors = ParseDiscordUser.FromAny(Context.Guild, input);
            if (userAuthors != null)
            {
                filterMessages = true;
                findUser = true;
            }

            if (new Regex("<@&([0-9]*)>").IsMatch(message))
            {
                filterMessages = true;
                findRole = true;
                var matches = new Regex("<@&([0-9]*)>").Matches(message);
                foreach (var match in matches)
                {
                    var ID = Regex.Replace(match.ToString(), "[<@&>]", "");
                    roleAuthors.Add(Context.Guild.GetRole(Convert.ToUInt64(ID)));
                }
            }

            if (new Regex("\".*?\"").IsMatch(message))
            {
                filterMessages = true;
                findString = true;
                var matches = new Regex("\".*?\"").Matches(message);
                foreach (var match in matches)
                {
                    phrases.Add(match.ToString().Replace("\"", ""));
                }
            }

            if (options.Any(o => message.Contains(o))) filterMessages = true;

            if (message.Contains("bots")) findBots = true;
            if (message.Contains("humans")) findHumans = true;
            if (message.Contains("text")) findText = true;
            if (message.Contains("mentions")) findMentions = true;
            if (message.Contains("embeds")) findEmbeds = true;
            if (message.Contains("pins")) findPins = true;
            if (message.Contains("emojis")) findEmojis = true;
            if (message.Contains("images")) findImages = true;
            if (message.Contains("links")) findLinks = true;

            if (message.Contains("before"))
            {
                _direction = Direction.Before;
                //parse for message id
                //startMessage =
            }
            if (message.Contains("after"))
            {
                _direction = Direction.After;
                //parse for message id
                //startMessage =
            }

            foreach (var phrase in phrases)
            {
                Console.WriteLine(phrase);//test
            }
        } // end ParseDeleteCommandAsync

        public async Task<bool> CheckParamConflict()
        {
            if (findBots && findHumans) return true;
            return false;
        } // end CheckParamConflict

        public async Task<IEnumerable<IMessage>> DownloadMessagesAsync(SocketCommandContext Context, int numToDelete)
        {
            if (numToDelete == 0) numToDelete = int.MaxValue;

            List<IMessage> matchedMessages = new List<IMessage>();

            var lastMessage = commandMessage;
            await Context.Channel.DeleteMessageAsync(commandMessage);
            while (matchedMessages.Count < numToDelete)
            {
                var messages = await Context.Channel.GetMessagesAsync(lastMessage, _direction).FlattenAsync();
                if (messages.Count() > 0) lastMessage = messages.Last();
                else
                {
                    InfoMessage = "there are no more messages to delete";
                    break;
                }

                Console.WriteLine($"how many to take {numToDelete - matchedMessages.Count()}");//test

                messages = TrimOldMessages(messages, out bool foundOldMessage);
                matchedMessages.AddRange(FilterMessages(messages.ToList().Take(numToDelete - matchedMessages.Count())));
                if (foundOldMessage) break;
            }
            return matchedMessages;
        } // end FindMessagesAsync



        private IEnumerable<IMessage> FilterMessages(IEnumerable<IMessage> messages)
        {
            List<IMessage> matchedMessages = new List<IMessage>();

            if (!filterMessages) return messages.ToList();

            foreach (var message in messages)
            {
                if (findUser && userAuthors.Contains(message.Author))
                    matchedMessages.Add(message);

                else if (findRole && roleAuthors.Any(role => role.Members.Contains(message.Author)))
                    matchedMessages.Add(message); //SocketGuildUser vs IUser

                else if (findString && phrases.Any(phrase => message.Content.Contains(phrase)))
                    matchedMessages.Add(message);

                else if (findBots && message.Author.IsBot)
                    matchedMessages.Add(message);

                else if (findHumans && !message.Author.IsBot)
                    matchedMessages.Add(message);

                else if (findText) ;
                //message.Content is only string;

                else if (findMentions && message.Tags.Any(t => t.Type == TagType.UserMention
                || t.Type == TagType.EveryoneMention || t.Type == TagType.HereMention
                || t.Type == TagType.RoleMention || t.Type == TagType.ChannelMention))
                    matchedMessages.Add(message);

                else if (findEmbeds && message.Embeds.Any())
                    matchedMessages.Add(message);

                else if (findPins && message.IsPinned)
                    matchedMessages.Add(message);

                else if (findEmojis && (message.Tags.Any(tag => tag.Type == TagType.Emoji) || Regex.IsMatch(message.Content, Variables.EmojiPattern)))
                    matchedMessages.Add(message);

                else if (findImages && message.Attachments != null)
                    matchedMessages.Add(message);

                else if (findLinks && new Regex("(?i)https?://").IsMatch(message.Content))
                    matchedMessages.Add(message);
            }; // end foreach

            return matchedMessages;
        } // end ParseEachMessage



        private IEnumerable<IMessage> TrimOldMessages(IEnumerable<IMessage> messages, out bool foundOldMessage)
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);

            var trimmedMessages = messages.Where(m => m.Timestamp >= twoWeeksAgo);

            if (trimmedMessages.Count() < messages.Count()) foundOldMessage = true;
            else foundOldMessage = false;

            return trimmedMessages;
        }




        public async Task DeleteMessages(SocketCommandContext Context, IEnumerable<IMessage> messages)
        {
            var channel = Context.Channel as ITextChannel;

            Console.WriteLine("messages ready for deletion:");//test
            foreach (var message in messages)
            {
                Console.WriteLine(message.ToString());//test
            }


            await channel.DeleteMessagesAsync(messages);

            if (!String.IsNullOrEmpty(InfoMessage))
            {
                var m1 = await Context.Channel.SendMessageAsync(InfoMessage);
                await Task.Delay(5000);
                await m1.DeleteAsync();
            }

            var m = await Context.Channel.SendMessageAsync($"{messages.Count()} messages deleted.");
            await Task.Delay(5000);
            await m.DeleteAsync();
        }
    }
}
