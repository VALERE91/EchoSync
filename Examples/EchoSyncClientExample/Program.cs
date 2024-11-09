using CommonGameplayCode;
using EchoSync;
using EchoSync.Replication;
using EchoSync.Replication.Client;
using EchoSync.Serialization;
using EchoSync.Utils;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Client Example");

ServiceLocator.InitializeDefaultServices();

EchoBitStream.RegisterType<Vector3>(
    (IBitWriter writer, ref BitStream bs, Vector3 vec) =>
    {
        writer.Write<float>(ref bs, vec.X);
        writer.Write<float>(ref bs, vec.Y);
        writer.Write<float>(ref bs, vec.Z);
    },
    (IBitReader reader, ref BitStream bs) => new Vector3
    {
        X = reader.Read<float>(ref bs),
        Y = reader.Read<float>(ref bs),
        Z = reader.Read<float>(ref bs)
    }
);

using EchoClient client = new EchoClient(new LiteNetLibClient("127.0.0.1", 9050), "key");
ServiceLocator.Provide<IReplicationEngine>(client);
while (!Console.KeyAvailable)
{
    client.Tick(0.015f);
    Thread.Sleep(15);
}