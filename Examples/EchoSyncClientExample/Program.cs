using EchoSync;
using LiteNetLibAdapters;

Console.WriteLine("Echo Sync Client Example");

using EchoClient client = new EchoClient(new LiteNetLibClient("127.0.0.1", 9050));
while (!Console.KeyAvailable)
{
    client.Tick(0.015f);
    Thread.Sleep(15);
}