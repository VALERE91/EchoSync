using System;
using System.Collections.Generic;
using EchoSync.Replication;
using EchoSync.Serialization;

namespace EchoSync.Messages
{
    public class SnapshotMessage
    {
        public MessageTypes Type { get; set; } = MessageTypes.Snapshot;

        public uint Frame { get; set; }

        public int ObjectDeletedCount { get; set; }

        public List<uint> DeletedObjects { get; set; } = new List<uint>();

        public int ObjectCount { get; set; }
        
        public Span<byte> Serialize(ref Span<byte> buffer, List<uint> destroyedNetObjects, List<NetObject> netObjects)
        {
            var snapshotStream = new BitStream(buffer);
            var snapshotWriter = new EchoBitStream();
            
            snapshotWriter.Write(ref snapshotStream, (byte)Type);
            //Write the frame number
            snapshotWriter.Write<uint>(ref snapshotStream, Frame);
            //Write the number of destroyed objects
            snapshotWriter.Write<int>(ref snapshotStream, ObjectDeletedCount);
            //Write the destroyed objects identifiers
            foreach (var destroyedNetObject in destroyedNetObjects)
            {
                snapshotWriter.Write<uint>(ref snapshotStream, destroyedNetObject);
            }
            destroyedNetObjects.Clear();
            
            //Write all the net objects
            snapshotWriter.Write<int>(ref snapshotStream, netObjects.Count);
            foreach (var netObject in netObjects)
            {
                netObject.NetWriteTo(snapshotWriter, ref snapshotStream);
            }
            return buffer.Slice(0, snapshotStream.BytePosition);
        }

        public void Deserialize(ref Span<byte> buffer, ILinkingContext linkingContext, 
            Func<uint, bool> shouldCreateNetObject,
            Action<uint, NetObject> netObjectCreated,
            Func<uint, NetObject> getNetObject)
        {
            var snapshotBitStream = new BitStream(buffer);
            var snapshotReader = new EchoBitStream();

            var messageType = snapshotReader.Read<byte>(ref snapshotBitStream);
            Frame = snapshotReader.Read<uint>(ref snapshotBitStream);
            ObjectDeletedCount = snapshotReader.Read<int>(ref snapshotBitStream);
            for (var i = 0; i < ObjectDeletedCount; i++)
            {
                var objectId = snapshotReader.Read<uint>(ref snapshotBitStream);
                Console.WriteLine($"Received destroy object {objectId}");
            }
            
            ObjectCount = snapshotReader.Read<int>(ref snapshotBitStream);
            for (var i = 0; i < ObjectCount; i++)
            {
                var classId = snapshotReader.Read<int>(ref snapshotBitStream);
                var objectId = snapshotReader.Read<uint>(ref snapshotBitStream);
                     
                if(shouldCreateNetObject(objectId))
                {
                    var netObject = linkingContext.CreateNetObject(classId, objectId);
                    netObjectCreated(objectId, netObject);
                }
                getNetObject(objectId).NetReadFrom(snapshotReader, ref snapshotBitStream);
            }
        }
    }
}