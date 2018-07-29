using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Multivac.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            IEnumerable<IMessage> messages;

            if (numToDelete == 0) messages = await FindAllMessagesAsync(Context);
            else messages = await FindMessagesAsync(Context, numToDelete);

            await DeleteMessages(Context, messages);
        }

        [Command("delete")]
        [RequireUserPermission(ChannelPermission.ManageMessages, Group = "perms")]
        [RequireOwner(Group = "perms")]
        public async Task Delete(int numToDelete, [Remainder] string stringIn)
        {
            IEnumerable<IMessage> messages;

            await ParseDeleteCommandAsync(Context, stringIn);
            if (await CheckParamConflict())
            {
                await ReplyAsync("error: parameter conflict");
                return;
            }

            if (numToDelete == 0) messages = await FindAllMessagesAsync(Context);
            else messages = await FindMessagesAsync(Context, numToDelete);

            await DeleteMessages(Context, messages);

        } // end delete


        private  string InfoMessage;
        private bool findEveryType = true;
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

        private async Task ParseDeleteCommandAsync(SocketCommandContext Context, string input)
        {
            Console.WriteLine("parsedeletecommand running...");

            string message = input.ToLower();

            userAuthors = MentionParser.GetUsersFromString(Context.Guild, input);
            if (userAuthors.Count > 0)
            {
                findEveryType = false;
                findUser = true;
            }

            if (new Regex("<@&([0-9]*)>").IsMatch(message))
            {
                findEveryType = false;
                findRole = true;
                var matches = new Regex("<@&([0-9]*)>").Matches(message).ToList();
                foreach (var match in matches)
                {
                    var ID = Regex.Replace(match.ToString(), "[<@&>]", "");
                    roleAuthors.Add(Context.Guild.GetRole(Convert.ToUInt64(ID)));
                }
            }

            if (new Regex("\".*?\"").IsMatch(message))
            {
                findEveryType = false;
                findString = true;
                var matches = new Regex("\".*?\"").Matches(message).ToList();
                foreach (var match in matches)
                {
                    phrases.Add(match.ToString().Replace("\"", ""));
                }
            }

            List<string> allParams = new List<string>()
            {
                "bots", "humans", "text", "mentions", "embeds", "pins", "emojis", "images", "links"
            };
            if (allParams.Any(param => message.Contains(param))) findEveryType = false;

            if (message.Contains("bots")) findBots = true;
            if (message.Contains("humans")) findHumans = true;
            if (message.Contains("text")) findText = true;
            if (message.Contains("mentions")) findMentions = true;
            if (message.Contains("embeds")) findEmbeds = true;
            if (message.Contains("pins")) findPins = true;
            if (message.Contains("emojis")) findEmojis = true;
            if (message.Contains("images")) findImages = true;
            if (message.Contains("links")) findLinks = true;

            foreach (var phrase in phrases)
            {
                Console.WriteLine(phrase);//test
            }
        } // end ParseDeleteCommandAsync

        private async Task<bool> CheckParamConflict()
        {
            if (findBots && findHumans) return true;
            return false;
        } // end CheckParamConflict


        private async Task<IEnumerable<IMessage>> FindAllMessagesAsync(SocketCommandContext Context)
        {
            Console.WriteLine("running FindAllMessagesAsync...");//test

            List<IMessage> matchedMessages = new List<IMessage>();

            IMessage deleteCommand = (await Context.Channel.GetMessagesAsync(1).FlattenAsync()).First();
            var lastMessage = deleteCommand;
            await Context.Channel.DeleteMessageAsync(deleteCommand);
            while (true)
            {
                var messages = await Context.Channel.GetMessagesAsync(lastMessage, Direction.Before).FlattenAsync();
                if (messages.Count() > 0) lastMessage = messages.Last();
                else
                {
                    InfoMessage = "all messages deleted";
                    break;
                }

                if (findEveryType) matchedMessages.AddRange(messages.ToList());

                else matchedMessages.AddRange(ParseEachMessage(messages));
            }
            return TrimOldMessages(matchedMessages);
        } // end FindAllMessagesAsync


        private async Task<IEnumerable<IMessage>> FindMessagesAsync(SocketCommandContext Context, int numToDelete)
        {
            Console.WriteLine("running FindMessagesAsync...");//test
            Console.WriteLine($"find every type? : {findEveryType}");//test

            List<IMessage> matchedMessages = new List<IMessage>();

            IMessage deleteCommand = (await Context.Channel.GetMessagesAsync(1).FlattenAsync()).First();
            var lastMessage = deleteCommand;
            await Context.Channel.DeleteMessageAsync(deleteCommand);
            while (matchedMessages.Count < numToDelete)
            {
                var messages = await Context.Channel.GetMessagesAsync(lastMessage, Direction.Before).FlattenAsync();
                Console.WriteLine($"message count: {messages.Count()}");//test

                if (messages.Count() > 0) lastMessage = messages.Last();
                else
                {
                    InfoMessage = "there are no more messages to delete";
                    break;
                }

                Console.WriteLine($"how many to take {numToDelete - matchedMessages.Count()}");//test


                if (findEveryType) matchedMessages.AddRange(messages.ToList().Take(numToDelete - matchedMessages.Count()));

                else matchedMessages.AddRange(ParseEachMessage(messages, (numToDelete - matchedMessages.Count())));
            }
            return TrimOldMessages(matchedMessages);
        } // end FindMessagesAsync



        private IEnumerable<IMessage> ParseEachMessage(IEnumerable<IMessage> messages, int numToParse = 100)
        {
            Console.WriteLine("running ParseEachMessage...");//test
            Console.WriteLine($"how many messages: {messages.Count()}");//test

            List<IMessage> matchedMessages = new List<IMessage>();
            foreach (var message in messages)
            {
                if (matchedMessages.Count() == numToParse) break;

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



        private IEnumerable<IMessage> TrimOldMessages(IEnumerable<IMessage> messages)
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);

            return messages.Where(m => m.Timestamp >= twoWeeksAgo);
        }




        private async Task DeleteMessages(SocketCommandContext Context, IEnumerable<IMessage> messages)
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
