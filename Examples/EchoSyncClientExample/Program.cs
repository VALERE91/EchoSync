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

ILinkingContext linkingContext = ServiceLocator.Get<ILinkingContext>();
linkingContext.RegisterNetClass<GamePlayerController>(GamePlayerController.Factory);
linkingContext.RegisterNetClass<Character>(Character.Factory());

var engine = new Engine(60);
var world = new World();
engine.AddTickable(world);
ServiceLocator.Provide<IWorld>(world);

using EchoClient client = new EchoClient(new LiteNetLibClient("127.0.0.1", 9050), "key");
ServiceLocator.Provide<IReplicationEngine>(client);
engine.AddTickable(client);

Thread engineThread = new Thread(engine.Run);
engineThread.Start();
Console.ReadLine();
engine.ShouldStop = true;
engineThread.Join();