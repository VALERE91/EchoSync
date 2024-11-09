using System;
using System.Collections.Generic;
using System.Text;
using EchoSync.Serialization;
using EchoSync.Transport;

namespace EchoSync.Replication.Server
{
    public class EchoServer : IDisposable, ITickable, IReplicationEngine
    {
        private readonly IServer _server;

        private readonly Dictionary<Guid, IPeer> _peers;
        private readonly Dictionary<Guid, int> _peerPlayer;
        private int _nextPlayerId = 0;
        private readonly Dictionary<int, Player> _players;
        public IServerRules ServerRules { get; }

        private List<NetObject> _netObjects = new List<NetObject>();

        public EchoServer(IServer server, IServerRules rules)
        {
            _server = server;
            _peers = new Dictionary<Guid, IPeer>();
            _peerPlayer = new Dictionary<Guid, int>();
            _players = new Dictionary<int, Player>();
            
            ServerRules = rules;
            
            _server.OnConnectionRequest += ConnectionRequestHandler;
            _server.OnClientConnected += ClientConnectedHandler;
            _server.OnClientDisconnected += ClientDisconnectedHandler;
            
            _server.Listen();
        }
        
        private void ClientDisconnectedHandler(IPeer peer)
        {
            Console.WriteLine($"Peer {peer.Identifier} disconnected");
            if (!_peers.ContainsKey(peer.Identifier)) return;
            if(!_peerPlayer.TryGetValue(peer.Identifier, out var playerId)) return;
            if(!_players.ContainsKey(playerId)) return;
            
            _peers.Remove(peer.Identifier);
            _peerPlayer.Remove(peer.Identifier);
            var player = _players[playerId];
            _players.Remove(playerId);
            ServerRules.DespawnPlayer(player);
        }

        private void ClientConnectedHandler(IPeer peer)
        {
            Console.WriteLine($"Peer {peer.Identifier} connected");
            var player = new Player(_nextPlayerId++, peer);
            _peers.Add(peer.Identifier, peer);
            _players.Add(player.PlayerId, player);
            _peerPlayer.Add(peer.Identifier, player.PlayerId);
            ServerRules.PostLogin(player);
            ServerRules.SpawnPlayer(player);
        }

        private bool ConnectionRequestHandler(string ip, int port, Span<byte> buffer)
        {
            Console.WriteLine($"Peer check from {ip}:{port}");
            return ServerRules.Login(buffer);
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public void Tick(float deltaTimeSeconds)
        {
            // Tick the server
            _server.Tick(deltaTimeSeconds);

            // Create the SnapShot
            Span<byte> snapshotBuffer = stackalloc byte[1024];
            var snapshotStream = new BitStream(snapshotBuffer);
            var snapshotWriter = new EchoBitStream();
            foreach (var netObject in _netObjects)
            {
                netObject.NetWriteTo(snapshotWriter, ref snapshotStream);
            }
            ReadOnlySpan<byte> dataToSend = snapshotBuffer.Slice(0, snapshotStream.BytePosition + 1);
            
            //Send it to every peer
            foreach (var (_, peer) in _peers)
            {
                peer.Sender.SendPacket(0, Reliability.Unreliable, dataToSend);
            }
        }

        public void RegisterNetObject(NetObject netObject)
        {
            _netObjects.Add(netObject);
        }

        public void UnregisterNetObject(NetObject netObject)
        {
            _netObjects.Remove(netObject);
        }

        public bool HasAuthority()
        {
            return true;
        }
    }
}