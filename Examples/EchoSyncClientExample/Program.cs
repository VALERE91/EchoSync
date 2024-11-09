using EchoSync;
using EchoSync.Replication;
using EchoSync.Replication.Client;
using EchoSync.Utils;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Client Example");

ServiceLocator.InitializeDefaultServices();

using EchoClient client = new EchoClient(new LiteNetLibClient("127.0.0.1", 9050), "key");
ServiceLocator.Provide<IReplicationEngine>(client);
while (!Console.KeyAvailable)
{
    client.Tick(0.015f);
    Thread.Sleep(15);
}