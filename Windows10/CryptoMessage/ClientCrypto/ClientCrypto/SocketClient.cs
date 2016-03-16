using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ClientCrypto
{
    internal class SocketClient
    {
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;

        private CancellationTokenSource _cancel;


        public string Ip { get; set; }
        public int Port { get; set; }


        public event TypedEventHandler<SocketClient, string> OnError;
        public event TypedEventHandler<SocketClient, string> OnDataReceive;

        public async Task ConnectAsync(string ip, int port)
        {
            Ip = ip;
            Port = port;
            try
            {
                //Resolved HostName to connect
                var hostName = new HostName(Ip);
                _socket = new StreamSocket();
                //Indicates whether keep-alive packets are sent to the remote destination
                _socket.Control.KeepAlive = true;
                //Connect
                await _socket.ConnectAsync(hostName, Port.ToString());
                _cancel = new CancellationTokenSource();
                _writer = new DataWriter(_socket.OutputStream);
                //Read Data
                ReadAsync();
            }
            catch (Exception ex)
            {
                InvokeOnError(ex.Message);
            }
        }

        public async Task DisconnectAsync()
        {
            try
            {
                _cancel.Cancel();

                _reader.DetachStream();
                _reader.DetachBuffer();

                _writer.DetachStream();
                _writer.DetachBuffer();

                await _socket.CancelIOAsync();

                _cancel = new CancellationTokenSource();
            }
            catch (Exception ex)
            {
                InvokeOnError(ex.Message);
            }
        }

        public async Task SendAsync(string message)
        {
            byte[] data = Cryptographic.Encrypt(message, "123");
            try
            {
                //Write Lenght message in buffer
                _writer.WriteInt32(data.Length);
                //Write message in buffer
                _writer.WriteBytes(data);

                //Send buffer
                await _writer.StoreAsync();
                //Clear buffer
                await _writer.FlushAsync();
            }
            catch (Exception ex)
            {
                InvokeOnError(ex.Message);
            }
        }

        private async Task ReadAsync()
        {
            _reader = new DataReader(_socket.InputStream);
            try
            {
                //If Close
                while (!_cancel.IsCancellationRequested)
                {
                    //Wait recive some data
                    byte[] data = await ReceiveData(_reader);
                    IBuffer buffer = data.AsBuffer();
                    //Decrypt message
                    string text = Cryptographic.Decrypt(buffer, "123");
                    //Invoke event when message Recive
                    InvokeOnDataRecEive(text);
                }
            }
            catch (Exception e)
            {
                InvokeOnError(e.Message);
            }
        }

        private static async Task<byte[]> ReceiveData(DataReader reader)
        {
            //Read Lenght Message
            uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
            //if disconnect
            if (sizeFieldCount != sizeof(uint))
                throw new Exception("Disconnect");
            //Get Lenght Message from buffer
            uint bufferSize = reader.ReadUInt32();
            uint dataRecive = await reader.LoadAsync(bufferSize);
            //if disconnect
            if (dataRecive != bufferSize)
                throw new Exception("Desconexão");
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

        private void InvokeOnDataRecEive(string mensagem)
        {
            if (OnDataReceive != null)
                OnDataReceive(this, mensagem);
        }
    }
}
