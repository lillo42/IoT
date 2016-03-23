using SendLog.ConsumeInMemory;
using SendLog.Socket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SendLog.Timer
{
    internal class TimerSample
    {
        //private readonly SendFile _file;
        private readonly SocketClient _client;
        private CancellationToken _cancel;
        private readonly TimeSpan _interval;
        private System.Threading.Timer _timer;

        public TimerSample(SocketClient client, CancellationToken cancel)
        {
            _client = client;
            _cancel = cancel;
            _interval = TimeSpan.FromSeconds(3);
        }


        private async void SendAsync(object state)
        {
            var logMemory = new LogConsumerMemory(_client.Output, _cancel);
            await logMemory.StartAsync();

            //await _file.SendAsync();
            //await Task.Delay(100);

            //var logFile = new LogConsumerFile(_client.Output, _cancel.Token);
            //logFile.StarAsync();

            //var sendFile = new SendFile(_client.Output, LogAsync.Instance.Filename);
            //sendFile.Send();
            _timer.Change(TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
        }

        public void Begin()
        {
            _timer = new System.Threading.Timer(SendAsync, null, TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);
        }
   
            
    }
}
