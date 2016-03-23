using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace SendLog.Socket
{
    internal class SendMessage
    {
        private readonly DataWriter _writer;
        public Exception LatsException { get; private set; }

        public SendMessage(IOutputStream outputStream)
        {
            _writer = new DataWriter(outputStream);
        }

        public async Task<bool> Send(string message)
        {
            try
            {
                //Write message in buffer
                _writer.WriteUInt32(_writer.MeasureString(message));
                _writer.WriteString(message);

                //Send message
                await _writer.StoreAsync();
                await _writer.FlushAsync();

                return true;
            }
            catch (Exception e)
            {
                LatsException = e;
                return false;
            }
        }
    }
}
