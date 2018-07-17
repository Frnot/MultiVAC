using System.Threading.Tasks;

namespace Multivac.Main
{
    class Program
    {
        private static TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public static void Main(string[] args) => Task.WaitAll(RunBot.Instance.MainAsync(), tcs.Task);

        public static void Shutdown() => tcs.SetResult(true);
    }
}