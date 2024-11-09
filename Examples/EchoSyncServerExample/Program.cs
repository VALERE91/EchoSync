// This is a simple example of how to create an EchoSync server using the LiteNetLib adapter.

using CommonGameplayCode;
using EchoSync.Replication;
using EchoSync.Replication.Server;
using EchoSync.Utils;
using EchoSyncServerExample;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Server Example");

ServiceLocator.InitializeDefaultServices();

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