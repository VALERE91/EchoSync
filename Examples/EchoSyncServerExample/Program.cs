using EchoSync;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Server Example");

using EchoServer server = new EchoServer(new LiteNetLibServer(9050));
while (!Console.KeyAvailable)
{
    server.Tick(0.015f);
    Thread.Sleep(15);
}