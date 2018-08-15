namespace Multivac.Data
{
    public class Guild
    {
        //public int Id { get; set; }
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string CommandPrefix { get; set; } = "default";

        // Misc
        public bool AllowColorRoles { get; set; } = false;

        // Music
        public ulong BoundMusicChannelId { get; set; }
        public int Volume { get; set; } = 100;

        // RP Companion
        public ulong BoundRPChannelId { get; set; }
    }
}
