// This is a simple example of how to create an EchoSync server using the LiteNetLib adapter.
using EchoSync.Replication;
using EchoSync.Utils;
using EchoSyncServerExample;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Server Example");

ServiceLocator.InitializeDefaultServices();

using EchoServer server = new EchoServer(new LiteNetLibServer(9050), new ServerRules());
while (!Console.KeyAvailable)
{
    server.Tick(0.015f);
    Thread.Sleep(15);
}