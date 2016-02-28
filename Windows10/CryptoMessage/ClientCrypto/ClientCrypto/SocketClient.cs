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
        public event TypedEventHandler<SocketClient, string> OnDataRecive;

        public async Task ConnectAsync(string ip, int port)
        {
            Ip = ip;
            Port = port;
            try
            {
                var hostName = new HostName(Ip);
                _socket = new StreamSocket();
                _socket.Control.KeepAlive = true;
                await _socket.ConnectAsync(hostName, Port.ToString());
                _cancel = new CancellationTokenSource();
                _writer = new DataWriter(_socket.OutputStream);
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
                _writer.WriteInt32(data.Length);
                _writer.WriteBytes(data);

                await _writer.StoreAsync();
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
                while (!_cancel.IsCancellationRequested)
                {
                    byte[] data = await ReciveData(_reader);
                    IBuffer buffer = data.AsBuffer();
                    string text = Cryptographic.Decrypt(buffer, "123");

                    InvokeOnDataRecive(text);
                }
            }
            catch (Exception e)
            {
                InvokeOnError(e.Message);
            }
        }

        private static async Task<byte[]> ReciveData(DataReader reader)
        {
            uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
            //if disconnect
            if (sizeFieldCount != sizeof(uint))
                throw new Exception("Desconexão");
            uint bufferSize = reader.ReadUInt32();
            uint dataRecive = await reader.LoadAsync(bufferSize);
            if (dataRecive != bufferSize)
                throw new Exception("Desconexão");
            var data = new byte[bufferSize];
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
