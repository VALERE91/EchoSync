using EchoSync.Transport;
using LiteNetLib;
using LiteNetLibAdapters.Receivers;
using LiteNetLibAdapters.Senders;

namespace LiteNetLibAdapters;

public class LiteNetLibClient : IClient
{
    private readonly string _host;
    private readonly int _port;
    
    public IPacketReceiver Receiver { get; set; }
    
    public IPacketSender Sender { get; set; }
    
    public Guid Identifier { get; set; } = Guid.NewGuid();

    private readonly NetManager _client;
    
    public LiteNetLibClient(string host, int port)
    {
        _host = host;
        _port = port;
        
        var listener = new EventBasedNetListener();
        _client = new NetManager(listener);
        _client.EnableStatistics = true;
        _client.Start();
        
        Sender = new ClientSender(_client);
        Receiver = new ClientReceiver(listener);
    }
    
    public void Connect(object connectionKey)
    {
        if (connectionKey is string key)
        {
            _client.Connect(_host, _port, key);
        }
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