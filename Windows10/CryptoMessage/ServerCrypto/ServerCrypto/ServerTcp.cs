using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ServerCrypto
{
    internal class ServerTcp
    {
        private readonly int _port;
        public int Port { get { return _port; } }

        private StreamSocketListener _listener = new StreamSocketListener();
        private StreamSocket _socket;
        private CancellationTokenSource _cancel;

        //Events
        public event TypedEventHandler<ServerTcp, string> OnError;
        public event TypedEventHandler<ServerTcp, string> OnDataRecive;

        public ServerTcp(int port)
        {
            _port = port;
        }

        public async Task CloseAsync()
        {
            if (_listener == null)
                return;
            //To Stop Read 
            _cancel.Cancel();
            await _socket.CancelIOAsync();
            await _listener.CancelIOAsync();
        }

        public async void StartAsync()
        {
            try
            {
                //Create a new Cancel Token
                _cancel = new CancellationTokenSource();

                _listener = new StreamSocketListener();

                //Assinged event when have a new connection
                _listener.ConnectionReceived += Listener_ConnectionReceived;
                _listener.Control.KeepAlive = true;
                //Bind port
                await _listener.BindServiceNameAsync(_port.ToString());
            }
            catch (Exception e)
            {
                InvokeOnError(e.Message);
            }
        }

        public async Task SendAsync(string text)
        {
            try
            {
                // DataWriter, who is send a message
                var writer = new DataWriter(_socket.OutputStream);
                //Encrypt message
                byte[] data = Cryptographic.Encrypt(text, "123");

                //Write Lenght message in buffer
                writer.WriteInt32(data.Length);
                //Write message in buffer
                writer.WriteBytes(data);

                //Send buffer
                await writer.StoreAsync();
                //Clear buffer
                await writer.FlushAsync();

            }
            catch (Exception e)
            {
                InvokeOnError(e.Message);
            }
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _socket = args.Socket;
            var reader = new DataReader(args.Socket.InputStream);
            try
            {
                //If Close
                while (!_cancel.IsCancellationRequested)
                {
                    //Wait recive some data
                    byte[] data = await ReciveData(reader);
                    IBuffer buffer = data.AsBuffer();
                    
                    //Decrypt message
                    string text = Cryptographic.Decrypt(buffer, "123");

                    //Invoke event when message Recive
                    InvokeOnDataRecive(text);
                }
            }
            catch (Exception e)
            {
                InvokeOnError(e.Message);
            }
        }

        private async Task<byte[]> ReciveData(DataReader reader)
        {
            //Read Lenght Message
            uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
            //if disconnect
            if (sizeFieldCount != sizeof(uint))
                throw new Exception("Disconnect");
            //Get Lenght Message from buffer
            uint bufferSize = reader.ReadUInt32();

            //Read Message
            uint dataRecive = await reader.LoadAsync(bufferSize);
            //if disconnect
            if (dataRecive != bufferSize)
                throw new Exception("Disconnect");
            var data = new byte[bufferSize];
            //Get message from buffer
            reader.ReadBytes(data);
            return data;
        }

        private void InvokeOnError(string error)
        {
            if (OnError != null)
                OnError(this, error);
        }

        private void InvokeOnDataRecive(string mensagem)
        {
            if (OnDataRecive != null)
                OnDataRecive(this, mensagem);
        }
    }
}
