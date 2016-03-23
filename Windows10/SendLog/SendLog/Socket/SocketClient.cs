using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SendLog.Socket
{
    internal class SocketClient
    {
        private readonly string _ip;
        private readonly int _port;

        private StreamSocket _socket;

        public IOutputStream Output
        {
            get
            {
                if (_socket == null)
                    return null;
                return _socket.OutputStream;
            }
        }

        public SocketClient(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public async Task DisconnectAsync()
        {
            if (_socket == null)
                return;
            await _socket.CancelIOAsync();
        }

        public async Task ConnectAsync()
        {
            await DisconnectAsync();
            try
            {
                var host = new HostName(_ip);
                if (_socket == null)
                    _socket = new StreamSocket();
                await _socket.ConnectAsync(host, _port.ToString());
                return;
            }
            catch (Exception e)
            {
                return;
            }
        }
    }
}
