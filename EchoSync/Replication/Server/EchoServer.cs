using System;
using System.Collections.Generic;
using System.Text;
using EchoSync.Inputs;
using EchoSync.Messages;
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
        private readonly Dictionary<int, PlayerController> _playersControllers;

        private uint _frameNumber = 0;
        
        public IServerRules ServerRules { get; }

        private readonly List<NetObject> _netObjects = new List<NetObject>();
        private readonly List<uint> _destroyedNetObjects = new List<uint>();
        
        public EchoServer(IServer server, IServerRules rules)
        {
            _server = server;
            _peers = new Dictionary<Guid, IPeer>();
            _peerPlayer = new Dictionary<Guid, int>();
            _players = new Dictionary<int, Player>();
            _playersControllers = new Dictionary<int, PlayerController>();
            
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
            var playerController = ServerRules.PostLogin(player);
            _playersControllers.Add(player.PlayerId, playerController);
            ServerRules.SpawnPlayer(playerController);
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
            _frameNumber++;
            
            // Tick the server
            _server.Tick(deltaTimeSeconds);

            // Create the SnapShot
            SnapshotMessage snapshot = new SnapshotMessage
            {
                Frame = _frameNumber,
                ObjectDeletedCount = _destroyedNetObjects.Count,
                DeletedObjects = _destroyedNetObjects,
                ObjectCount = _netObjects.Count
            };
            
            Span<byte> snapshotBuffer = stackalloc byte[1024];
            ReadOnlySpan<byte> dataToSend = snapshot.Serialize(ref snapshotBuffer, _destroyedNetObjects, _netObjects);
            
            //Send it to every peer
            //TODO we need to be able to select which peer needs it
            foreach (var (_, peer) in _peers)
            {
                peer.Sender.SendPacket(0, Reliability.Unreliable, dataToSend);
            }
            
            //Read inputs and RPCs
            foreach (var (_, peer) in _peers)
            {
                if (!peer.Receiver.HasData(0)) continue;
                if (!peer.Receiver.PeekLatest(0, out var data)) continue;
                
                Span<byte> receivedData = data.ToArray();
                
                MessageTypes messageType = (MessageTypes)receivedData[0];
                if (messageType == MessageTypes.Input)
                {
                    if (_peerPlayer.TryGetValue(peer.Identifier, out var playerId) 
                        && _playersControllers.TryGetValue(playerId, out var playerController))
                    {
                        playerController.InputReceived(ref receivedData);
                    }
                }
                else if (messageType == MessageTypes.Rpc)
                {
                    RpcMessage rpcMessage = new RpcMessage();
                    var parameters = rpcMessage.Deserialize(ref receivedData);
                    //Get the associated net object
                    NetObject netObject = _netObjects.Find(o => o.ObjectId == rpcMessage.ObjectId);
                    if (netObject == null) continue;
                    netObject.InvokeRpc(rpcMessage.MethodId, parameters);
                }
                
                peer.Receiver.PopLatest(0);
            }
            
            //Tick all the players
            foreach (var (_, playerController) in _playersControllers)
            {
                playerController.Tick(deltaTimeSeconds);
            }
        }

        public void RegisterNetObject(NetObject netObject)
        {
            _netObjects.Add(netObject);
        }

        public void UnregisterNetObject(NetObject netObject)
        {
            _destroyedNetObjects.Add(netObject.ObjectId);
            _netObjects.Remove(netObject);
        }

        public bool HasAuthority()
        {
            return true;
        }

        public uint GetFrameNumber()
        {
            return _frameNumber;
        }

        public FrameType GetFrameType()
        {
            return FrameType.Server;
        }

        public IPeer GetLocalPeer()
        {
            throw new NotImplementedException("Server does not have a local peer");
        }
    }
}