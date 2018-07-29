using System;
using System.Threading;
using System.Threading.Tasks;

namespace Multivac.Main
{
    class Program
    {
        private static TaskCompletionSource<bool> tcs;
        private static bool Reboot;

        public static readonly RunBot Bot = new RunBot();

        public static void Main(string[] args)
        {
            do
            {
                Reboot = false;
                tcs = new TaskCompletionSource<bool>();

                Task.WaitAll(Bot.MainAsync(), tcs.Task);
            } while (Reboot);
        }

        public static void Shutdown() => tcs.SetResult(true);

        public static void Restart()
        {
            Reboot = true;
            tcs.SetResult(true);
        }
    }
}