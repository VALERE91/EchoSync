using System;

namespace EchoSync.Transport
{
    public interface IServer : IDisposable, ITickable
    {
        public delegate void OnConnectionRequestDelegate(string ip, int port, Span<byte> buffer);
        public delegate void OnClientConnectedDelegate(IClient client);
        public delegate void OnClientDisconnectedDelegate(IClient client);
        
        public event OnConnectionRequestDelegate OnConnectionRequest;
        public event OnClientConnectedDelegate OnClientConnected;
        public event OnClientDisconnectedDelegate OnClientDisconnected;

        public void Listen();
    }
}