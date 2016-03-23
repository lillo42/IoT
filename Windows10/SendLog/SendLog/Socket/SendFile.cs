using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SendLog.Socket
{
    public sealed class SendFile
    {
        private readonly DataWriter _writer;
        private readonly byte[] _file; 
        public Exception LatsException { get; private set; }

        public SendFile(IOutputStream outputStream,string filename)
        {
            _writer = new DataWriter(outputStream);

            IStorageFolder local = ApplicationData.Current.LocalFolder;
            Task<Stream> taskStream = local.OpenStreamForReadAsync(filename);
            taskStream.Wait();

            Stream s = taskStream.Result;
            _file = new byte[s.Length];
            s.Read(_file, 0, _file.Length);
        }

        public IAsyncOperation<bool> SendAsync()
        {
            return SendAsyncHelper().AsAsyncOperation();
        }

        private async Task<bool> SendAsyncHelper()
        {
            try
            {
                //Write message in buffer
                _writer.WriteInt32(_file.Length);
                _writer.WriteBytes(_file);

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
