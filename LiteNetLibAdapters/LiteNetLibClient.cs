using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters;

public class LiteNetLibClient : IClient
{
    private readonly string _host;
    private readonly int _port;
    public IPacketReceiver Receiver { get; set; }
    
    public IPacketSender Sender { get; set; }
    
    public Guid Identifier { get; set; }

    private EventBasedNetListener _listener;

    private NetManager _client;
    
    public LiteNetLibClient(NetPeer peer)
    {
        
    }
    
    public LiteNetLibClient(string host, int port)
    {
        _host = host;
        _port = port;
        _listener = new EventBasedNetListener();
        _client = new NetManager(_listener);
        _client.Start();
    }
    
    public void Connect()
    {
        _client.Connect(_host, _port, "key");
    }

    public void Dispose()
    {
        _client.Stop();
    }

    public void Tick(float deltaTimeSeconds)
    {
        _client.PollEvents();
    }
}