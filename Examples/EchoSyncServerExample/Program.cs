using EchoSync;
using EchoSync.Replication;
using EchoSync.Utils;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Server Example");

ServiceLocator.Provide<ILinkingContext>(new DefaultLinkingContext());
ServiceLocator.Provide<IObjectIdGenerator>(new DefaultObjectIdGenerator());
ServiceLocator.Provide<IReplicationEngine>(new ReplicationEngine(true));

using EchoServer server = new EchoServer(new LiteNetLibServer(9050));
while (!Console.KeyAvailable)
{
    server.Tick(0.015f);
    Thread.Sleep(15);
}