using SendLog.Log;
using SendLog.Socket;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SendLog.ConsumerInTextFile
{
    internal class LogConsumerFile
    {
        private readonly SendMessage _sender;
        private readonly CancellationToken _cancel;

        public LogConsumerFile(IOutputStream output, CancellationToken cancel)
        {
            _sender = new SendMessage(output);
            _cancel = cancel;
        }

        public async void StarAsync()
        {
            foreach(string log in LogAsync.Instance.LogInFile)
            {
                await _sender.Send(log);
                if (_cancel != null && _cancel.IsCancellationRequested)
                    break;
                await Task.Delay(0);
            }
        }
    }
}
