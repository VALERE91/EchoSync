using EchoSync;
using EchoSync.Replication;
using EchoSync.Utils;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Client Example");

ServiceLocator.Provide<ILinkingContext>(new DefaultLinkingContext());
ServiceLocator.Provide<IObjectIdGenerator>(new DefaultObjectIdGenerator());
ServiceLocator.Provide<IReplicationEngine>(new ReplicationEngine(false));

using EchoClient client = new EchoClient(new LiteNetLibClient("127.0.0.1", 9050));
while (!Console.KeyAvailable)
{
    client.Tick(0.015f);
    Thread.Sleep(15);
}