using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multivac.Data
{
    public static class Database
    {
        private static DiscordSocketClient _client;
        private static readonly LiteCollection<Guild> GuildDatabase = new LiteDatabase(@"GuildData.db").GetCollection<Guild>("guilds");

        public static Guild Load(ulong id)
        {
            return GuildDatabase.FindOne(g => g.Id == id);
        }

        public static void Save(Guild guild)
        {
            GuildDatabase.Update(guild);
        }



        public static void SyncDatabase(DiscordSocketClient client)
        {
            _client = client;
            _client.JoinedGuild += (guild) => AddGuildToDb(guild.Id);
            _client.LeftGuild += (guild) => RemoveGuildFromDb(guild.Id);

            Console.WriteLine("Syncing Database...");

            // check that database contains all existing guilds
            foreach (var guild in _client.Guilds)
            {
                if (!GuildDatabase.Exists(x => x.Id == guild.Id))
                {
                    Console.WriteLine("found missing guild");
                    AddGuildToDb(guild.Id);
                }
            }

            // check if the database contains guilds that no longer exist
            foreach (var guild in GuildDatabase.FindAll())
            {
                if (!_client.Guilds.Any(g => g.Id == guild.Id))
                {
                    Console.WriteLine("found non-existent guild");
                    RemoveGuildFromDb(guild.Id);
                }
            }
        } // end SyncDatabase


        public static Task AddGuildToDb(ulong guildId)
        {
            var guild = _client.GetGuild(guildId);
            Console.WriteLine(guild.Name);

            var newGuild = new Guild
            {
                Id = guild.Id,
                Name = guild.Name,
            };

            GuildDatabase.Insert(newGuild);
            return Task.CompletedTask;
        }

        public static Task RemoveGuildFromDb(ulong guildId)
        {
            GuildDatabase.Delete(x => x.Id == guildId);
            return Task.CompletedTask;
        }

    }
}
