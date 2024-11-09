using System;
using System.Collections.Generic;
using System.Text;
using EchoSync.Replication;
using EchoSync.Transport;
using EchoSync.Utils;

namespace EchoSync.Replication
{
    public class EchoServer : IDisposable, ITickable, IReplicationEngine
    {
        private readonly IServer _server;

        private readonly Dictionary<Guid, IPeer> _peers;

        public IServerRules ServerRules { get; }

        public EchoServer(IServer server, IServerRules rules)
        {
            _server = server;
            _peers = new Dictionary<Guid, IPeer>();
            
            ServerRules = rules;
            
            _server.OnConnectionRequest += ConnectionRequestHandler;
            _server.OnClientConnected += ClientConnectedHandler;
            _server.OnClientDisconnected += ClientDisconnectedHandler;
            
            _server.Listen();
        }

        private void ClientDisconnectedHandler(IPeer peer)
        {
            Console.WriteLine($"Peer {peer.Identifier} disconnected");
            if (_peers.ContainsKey(peer.Identifier))
            {
                _peers.Remove(peer.Identifier);
            }
        }

        private void ClientConnectedHandler(IPeer peer)
        {
            Console.WriteLine($"Peer {peer.Identifier} connected");
            _peers.Add(peer.Identifier, peer);
        }

        private bool ConnectionRequestHandler(string ip, int port, Span<byte> buffer)
        {
            Console.WriteLine($"Peer check from {ip}:{port}");
            return true;
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