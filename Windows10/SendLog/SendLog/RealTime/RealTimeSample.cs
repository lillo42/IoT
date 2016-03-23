using SendLog.Log;
using SendLog.Socket;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SendLog.RealTime
{
    internal class RealTimeSample
    {
        //private readonly SendFile _file;
        private readonly SendMessage _client;
        private readonly CancellationToken _cancel;
        public RealTimeSample(SocketClient client, CancellationToken cancel)
        {
            //_file = new SendFile(client.Output, LogAsync.Instance.Filename);
            _client = new SendMessage(client.Output);
            _cancel = cancel;
        }

        public void Begin()
        {
            Task t = ReadForever();
        }

        private async Task ReadForever()
        {
            while (_cancel != null && !_cancel.IsCancellationRequested)
            {
                IEnumerable<string> listLog = LogAsync.Instance.LogInMemory;
                //IEnumerable<string> listLog = LogAsync.Instance.LogInFile;

                //await _file.SendAsync();
                //await Task.Delay(100);

                foreach (string log in listLog)
                {
                    await _client.Send(log);
                    if (_cancel.IsCancellationRequested)
                        break;
                    await Task.Delay(0);
                }

                await Task.Delay(100);
            }
        }
    }
}
