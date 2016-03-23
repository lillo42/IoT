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
        private DateTime _lastSend = DateTime.Now;
        private int _interval;

        public TimerSample(SocketClient client, CancellationToken cancel)
        {
            _client = client;
            _cancel = cancel;
            _interval = 30;
        }


        private async void SendAsync()
        {
            var logMemory = new LogConsumerMemory(_client.Output, _cancel);
            await logMemory.StartAsync();

            //await _file.SendAsync();
            //await Task.Delay(100);

            //var logFile = new LogConsumerFile(_client.Output, _cancel.Token);
            //logFile.StarAsync();

            //var sendFile = new SendFile(_client.Output, LogAsync.Instance.Filename);
            //sendFile.Send();
        }

        public void Begin()
        {
            Task.Run(() => Run());
        }

        private void Run()
        {
            while (!_cancel.IsCancellationRequested)
            {
                TimeSpan last = DateTime.Now.Subtract(_lastSend);
                if (last.Seconds > _interval)
                {
                    SendAsync();
                    _lastSend = DateTime.Now;
                }
            }
        }
    }
}
