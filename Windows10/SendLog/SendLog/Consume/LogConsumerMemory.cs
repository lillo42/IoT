using SendLog.Log;
using SendLog.Socket;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SendLog.ConsumeInMemory
{
    internal class LogConsumerMemory
    {
        private readonly SendMessage _sender;
        private readonly CancellationToken _cancel;

        public LogConsumerMemory(IOutputStream outPut, CancellationToken cancel)
        {
            _sender = new SendMessage(outPut);
            _cancel = cancel;
        }

        public async Task StartAsync()
        {
            foreach (string log in LogAsync.Instance.LogInMemory)
            {
                await _sender.Send(log);
                if (_cancel != null && _cancel.IsCancellationRequested)
                    break;
                await Task.Delay(0);
            }
        }
    }
}
