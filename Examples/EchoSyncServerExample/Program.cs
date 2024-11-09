using EchoSync;
using EchoSync.Utils;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Server Example");

ServiceLocator.InitializeDefaultServices(true);

using EchoServer server = new EchoServer(new LiteNetLibServer(9050));
while (!Console.KeyAvailable)
{
    server.Tick(0.015f);
    Thread.Sleep(15);
}