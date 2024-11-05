using System;

namespace EchoSync.Transport
{
    public interface IServer : IDisposable, ITickable
    {
        public delegate bool OnConnectionRequestDelegate(string ip, int port, Span<byte> buffer);
        public delegate void OnClientConnectedDelegate(IPeer peer);
        public delegate void OnClientDisconnectedDelegate(IPeer peer);
        
        public event OnConnectionRequestDelegate OnConnectionRequest;
        public event OnClientConnectedDelegate OnClientConnected;
        public event OnClientDisconnectedDelegate OnClientDisconnected;

        public void Listen();
    }
}