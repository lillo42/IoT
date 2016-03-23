using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;
using System.Threading;
using System.Threading.Tasks;
using SendLog.Timer;
using SendLog.Socket;
using SendLog.RealTime;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SendLog
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private List<CancellationTokenSource> _listToken = new List<CancellationTokenSource>();

        private static TimerSample _dispatcher;
        private static RealTimeSample _realTime;
        private static SocketClient _client;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            _client = new SocketClient("192.168.0.31", 9000);
            await _client.ConnectAsync();

            var p = new Productor.Productor();
            Task t = p.BeginCreateLog(true, false);

            //ExampleTimer();
            ExampleRealTime();
        }

        private void ExampleRealTime()
        {
            var cancel = new CancellationTokenSource();
            _listToken.Add(cancel);

            _realTime = new RealTimeSample(_client, cancel.Token);
            _realTime.Begin();
        }

        private void ExampleTimer()
        {
            ExampleDispathcerTimer();
        }

        private void ExampleDispathcerTimer()
        {
            var cancel = new CancellationTokenSource();
            _listToken.Add(cancel);

            _dispatcher = new TimerSample(_client,cancel.Token);
            _dispatcher.Begin();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _listToken.ForEach(x => x.Cancel());
            Task.Delay(1000).Wait();
            _backgroundTaskDeferral?.Complete();
        }
    }
}
