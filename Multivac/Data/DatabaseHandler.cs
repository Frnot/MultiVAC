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
        private readonly LiteDatabase db;

        public DatabaseHandler(DiscordSocketClient client, LiteDatabase dbService)
        {
            _client = client;
            db = dbService;

            _client.JoinedGuild += (g) => AddGuildToDb(g.Id);
            _client.LeftGuild += (g) => RemoveGuildFromDb(g.Id);
        }

        public void SyncDatabase()
        {
            Console.WriteLine("Syncing Database...");

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
        } // end SyncDatabase

        public Task AddGuildToDb(ulong guildId)
        {
            var dbGuilds = db.GetCollection<GuildData>("guilds");

            var guild = _client.GetGuild(guildId);
            Console.WriteLine(guild.Name);

            var newGuild = new GuildData
            {
                GuildId = guild.Id,
                Name = guild.Name,
                CommandPrefix = "default",

                Volume = 100,
            };

            dbGuilds.Insert(newGuild);
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

        

    } // end class Databasehandler
}
