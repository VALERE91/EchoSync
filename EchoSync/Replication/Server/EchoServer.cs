using System;
using System.Collections.Generic;
using System.Text;
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
            foreach (var (identifier, peer) in _peers)
            {
                if (!peer.Receiver.HasData(0))
                {
                    continue;
                }
                if (!peer.Receiver.PeekLatest(0, out var data))
                {
                    continue;
                }
                
                Console.WriteLine($"Received data from {identifier}: {Encoding.UTF8.GetString(data)}");
                peer.Sender.SendPacket(0, Reliability.Unreliable, data);
                peer.Receiver.PopLatest(0);
            }
            _server.Tick(deltaTimeSeconds);
        }

        public bool HasAuthority()
        {
            return true;
        }
    }
}