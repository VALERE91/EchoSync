// This is a simple example of how to create an EchoSync server using the LiteNetLib adapter.

using CommonGameplayCode;
using EchoSync.Replication;
using EchoSync.Replication.Server;
using EchoSync.Serialization;
using EchoSync.Utils;
using EchoSyncServerExample;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Server Example");

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

ServiceLocator.InitializeDefaultServices();

ILinkingContext linkingContext = ServiceLocator.Get<ILinkingContext>();
linkingContext.RegisterNetClass<Character>(Character.Factory());

var engine = new Engine(60);
var world = new World();
engine.AddTickable(world);

using EchoServer server = new EchoServer(new LiteNetLibServer(9050), new ServerRules(world));
engine.AddTickable(server);

ServiceLocator.Provide<IReplicationEngine>(server);

Thread engineThread = new Thread(engine.Run);
engineThread.Start();
Console.ReadLine();
engine.ShouldStop = true;
engineThread.Join();