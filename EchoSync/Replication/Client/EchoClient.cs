using System;
using System.Collections.Generic;
using System.Text;
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
                    var receivedData = data.ToArray();
                    var snapshotBitStream = new BitStream(receivedData);
                    var snapshotReader = new EchoBitStream();

                    var frameNumber = snapshotReader.Read<uint>(ref snapshotBitStream);
                    var numberOfObjectDestroyed = snapshotReader.Read<int>(ref snapshotBitStream);
                    for (var i = 0; i < numberOfObjectDestroyed; i++)
                    {
                        var objectId = snapshotReader.Read<uint>(ref snapshotBitStream);
                        Console.WriteLine($"Received destroy object {objectId}");
                    }
                    var numberOfObject = snapshotReader.Read<int>(ref snapshotBitStream);
                    for (var i = 0; i < numberOfObject; i++)
                    {
                        var classId = snapshotReader.Read<int>(ref snapshotBitStream);
                        var objectId = snapshotReader.Read<uint>(ref snapshotBitStream);
                        
                        if (!_trackedNetObjects.ContainsKey(objectId))
                        {
                            var netObject = _linkingContext.CreateNetObject(classId, objectId);
                            _trackedNetObjects.Add(objectId, netObject);
                        }
                        
                        _trackedNetObjects[objectId].NetReadFrom(snapshotReader, ref snapshotBitStream);
                    }
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
    }
}