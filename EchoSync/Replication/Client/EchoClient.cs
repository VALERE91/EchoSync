using System;
using System.Collections.Generic;
using System.Text;
using EchoSync.Messages;
using EchoSync.Serialization;
using EchoSync.Transport;
using EchoSync.Utils;

namespace EchoSync.Replication.Client
{
    public class EchoClient : IDisposable, ITickable, IReplicationEngine
    {
        private readonly IClient _client;
        
        private Dictionary<uint, NetObject> _trackedNetObjects = new Dictionary<uint, NetObject>();
        
        private ILinkingContext _linkingContext;
        
        private uint _frameNumber = 0;
        
        public EchoClient(IClient client, object connectionKey)
        {
            _client = client;
            _linkingContext = ServiceLocator.Get<ILinkingContext>();
            _client.Connect(connectionKey);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void Tick(float deltaTimeSeconds)
        {
            _frameNumber++;
            if(_client.Receiver.HasData(0))
            {
                if(_client.Receiver.PeekLatest(0, out var data))
                {
                    Span<byte> receivedData = data.ToArray();
                    
                    SnapshotMessage snapshotMessage = new SnapshotMessage();
                    snapshotMessage.Deserialize(ref receivedData, _linkingContext,
                        objectId => !_trackedNetObjects.ContainsKey(objectId),
                        (objectId, netObject) =>
                        {
                            _trackedNetObjects.Add(objectId, netObject);
                        },
                        objectId => _trackedNetObjects[objectId]);
                    
                    _client.Receiver.PopLatest(0);
                }
            }
            
            _client.Tick(deltaTimeSeconds);
        }

        public void RegisterNetObject(NetObject netObject)
        {
            throw new NotImplementedException("Predictive spawning is not supported for now");
        }

        public void UnregisterNetObject(NetObject netObject)
        {
            throw new NotImplementedException("Predictive unspawning is not supported for now");
        }

        public bool HasAuthority()
        {
            return false;
        }

        public uint GetFrameNumber()
        {
            //TODO : Implement frame synchronisation (Issues #17)
            return _frameNumber;
        }

        public FrameType GetFrameType()
        {
            return FrameType.Prediction;
        }

        public IPeer GetLocalPeer()
        {
            return _client;
        }
    }
}