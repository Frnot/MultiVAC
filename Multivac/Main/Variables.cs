using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Multivac
{
    public class Variables
    {
        public readonly static string ProgramVersion = "5.1.5";

        public static string DiscordToken { get; private set; }
        public static string DefaultCommandPrefix { get; private set; }
        public static string GoogleAPIKey { get; private set; }
        public static string EmojiPattern { get; private set; }

        public static Task LoadConfig()
        {
            dynamic JsonConfig = JObject.Load(new JsonTextReader(new StreamReader("config.json")));

            DiscordToken = JsonConfig.Token.ToString();
            DefaultCommandPrefix = JsonConfig.CommandPrefix.ToString();
            GoogleAPIKey = JsonConfig.GoogleApiKey.ToString();

            EmojiPattern = File.ReadAllText("emojipattern.txt");

            return Task.CompletedTask;
        } // end LoadConfig
    }
}
