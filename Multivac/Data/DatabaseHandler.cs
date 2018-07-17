using Discord.WebSocket;
using LiteDB;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Multivac.Data
{
    public class DatabaseHandler
    {
        private readonly DiscordSocketClient _client;

        public DatabaseHandler(DiscordSocketClient client)
        {
            _client = client;

            _client.JoinedGuild += (g) => AddGuildToDb(g.Id);
            _client.LeftGuild += (g) => RemoveGuildFromDb(g.Id);
        }

        public void SyncDatabase()
        {
            Console.WriteLine("Syncing Database...");

            using (var db = new LiteDatabase(@"GuildData.db"))
            {
                var dbGuilds = db.GetCollection<GuildData>("guilds");

                // check that database contains all existing guilds
                foreach (var guild in _client.Guilds)
                {
                    if (!dbGuilds.Exists(x => x.GuildId == guild.Id))
                    {
                        Console.WriteLine("found missing guild");
                        AddGuildToDb(guild.Id);
                    }
                }

                // check if the database contains guilds that no longer exist
                foreach (var dbGuild in dbGuilds.FindAll())
                {
                    if (!_client.Guilds.Any(g => g.Id == dbGuild.GuildId))
                    {
                        Console.WriteLine("found non-existent guild");
                        RemoveGuildFromDb(dbGuild.GuildId);
                    }
                }
            }
        } // end SyncDatabase

        public Task AddGuildToDb(ulong guildId)
        {
            using (var db = new LiteDatabase(@"GuildData.db"))
            {
                var dbGuilds = db.GetCollection<GuildData>("guilds");

                var guild = _client.GetGuild(guildId);
                Console.WriteLine(guild.Name);

                var newGuild = new GuildData
                {
                    GuildId = guild.Id,
                    Name = guild.Name,
                    CommandPrefix = Variables.DefaultCommandPrefix,
                };

                dbGuilds.Insert(newGuild);
            }
            return Task.CompletedTask;
        }

        public Task RemoveGuildFromDb(ulong guildId)
        {
            using (var db = new LiteDatabase(@"GuildData.db"))
            {
                var dbGuilds = db.GetCollection<GuildData>("guilds");

                dbGuilds.Delete(x => x.GuildId == guildId);
            }
            return Task.CompletedTask;
        }

        public string GetGuildPrefix(ulong guildId)
        {
            using (var db = new LiteDatabase(@"GuildData.db"))
            {
                bool trueisgood = db.CollectionExists("guilds");

                var guilds = db.GetCollection<GuildData>("guilds");

                var guild = guilds.FindOne(x => x.GuildId == guildId);

                var thisGuildPrefix = guild.CommandPrefix;

                return String.IsNullOrEmpty(thisGuildPrefix) ? Variables.DefaultCommandPrefix : thisGuildPrefix;
            }
        } // end GetGuildPrefix

    } // end class Databasehandler
}
