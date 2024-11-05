using System;

namespace EchoSync.Transport
{
    public interface IServer
    {
        public delegate void OnClientCheckDelegate(string ip, int port, Span<byte> buffer);
        public delegate void OnClientConnectedDelegate(IClient client);
        public delegate void OnClientDisconnectedDelegate(IClient client);
        
        public event OnClientCheckDelegate OnClientCheck;
        public event OnClientConnectedDelegate OnClientConnected;
        public event OnClientDisconnectedDelegate OnClientDisconnected;
    }
}