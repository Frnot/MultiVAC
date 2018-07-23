namespace Multivac.Data
{
    public class GuildData
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string CommandPrefix { get; set; }

        // Music
        public ulong BoundMusicChannelId { get; set; }
        public int Volume { get; set; }

        // RP Comanion
        public ulong BoundRPChannelId { get; set; }
    }
}
