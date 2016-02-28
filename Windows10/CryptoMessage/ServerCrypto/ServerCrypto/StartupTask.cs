using Windows.ApplicationModel.Background;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace ServerCrypto
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //

            BackgroundTaskDeferral background = taskInstance.GetDeferral();

            var server = new ServerTcp(9000);
            server.OnError += Server_OnError;
            server.OnDataRecive += Server_OnDataRecive;

            ThreadPool.RunAsync(x =>
            {
                server.StartAsync();
            });
        }

        private async void Server_OnDataRecive(ServerTcp sender, string args)
        {
            //Answear text Recive
            await sender.SendAsync(string.Concat("Text Recive:", args));
        }

        private void Server_OnError(ServerTcp sender, string args)
        {
        }
    }
}
