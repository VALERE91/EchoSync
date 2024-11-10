using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters
{
    public class LiteNetLibServer : IServer
    {
        public event IServer.OnConnectionRequestDelegate? OnConnectionRequest;
        public event IServer.OnClientConnectedDelegate? OnClientConnected;
        public event IServer.OnClientDisconnectedDelegate? OnClientDisconnected;

        private readonly int _port;
        private readonly EventBasedNetListener _listener;
        private readonly NetManager _server;

        private readonly Dictionary<int, LiteNetLibPeer> _peers = new();
    
        public LiteNetLibServer(int port)
        {
            _port = port;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
        
            _listener.ConnectionRequestEvent += ConnectionRequestEventHandler;
            _listener.PeerConnectedEvent += PeerConnectedEventHandler;
            _listener.PeerDisconnectedEvent += PeerDisconnectedEventHandler;
        
            _listener.NetworkReceiveEvent += NetworkReceiveEventHandler;
        }

        private void NetworkReceiveEventHandler(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            if (!_peers.TryGetValue(peer.Id, out LiteNetLibPeer? value))
            {
                return;
            }

            value.QueueData(reader, channel, deliveryMethod);
        }

        private void PeerDisconnectedEventHandler(NetPeer peer, DisconnectInfo info)
        {
            if(_peers.ContainsKey(peer.Id))
            {
                OnClientDisconnected?.Invoke(_peers[peer.Id]);
                _peers.Remove(peer.Id);
            }
        }

        private void PeerConnectedEventHandler(NetPeer peer)
        {
            var p = new LiteNetLibPeer(peer);
            _peers.Add(peer.Id, p);
            OnClientConnected?.Invoke(p);
        }

        private void ConnectionRequestEventHandler(ConnectionRequest request)
        {
            Console.WriteLine(request.Data.RawData.Length);
            bool? accept = OnConnectionRequest?.Invoke(request.RemoteEndPoint.Address.ToString(), request.RemoteEndPoint.Port, request.Data.RawData);
            if (accept != null && accept.Value)
            {
                request.Accept();
                return;
            }

            request.Reject();
        }

        public void Dispose()
        {
            _server.Stop();
            GC.SuppressFinalize(this);
        }

        public void Tick(float deltaTimeSeconds)
        {
            _server.PollEvents();
        }

        public void Listen()
        {
            _server.Start(_port);
        }
    }
}