using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters;

public class LiteNetLibServer : IServer
{
    public event IServer.OnConnectionRequestDelegate? OnConnectionRequest;
    public event IServer.OnClientConnectedDelegate? OnClientConnected;
    public event IServer.OnClientDisconnectedDelegate? OnClientDisconnected;

    private readonly int _port;
    private readonly EventBasedNetListener _listener;
    private readonly NetManager _server;

    public LiteNetLibServer(int port)
    {
        _port = port;
        _listener = new EventBasedNetListener();
        _server = new NetManager(_listener);
        
        _listener.ConnectionRequestEvent += request =>
        {
            Console.WriteLine(request.Data.RawData.Length);
            bool? accept = OnConnectionRequest?.Invoke(request.RemoteEndPoint.Address.ToString(), 
                 request.RemoteEndPoint.Port, 
                 request.Data.RawData);
            if (accept != null && accept.Value)
            {
                request.Accept();
                return;
            }
            
            request.Reject();
        };
        
        _listener.PeerConnectedEvent += peer =>
        {
            OnClientConnected?.Invoke(new LiteNetLibClient(peer));
        };
        
        _listener.PeerDisconnectedEvent += (peer, info) =>
        {
            OnClientDisconnected?.Invoke(new LiteNetLibClient(peer));
        };
    }
    
    public void Dispose()
    {
        _server.Stop();
        GC.SuppressFinalize(this);
    }

    public void Tick(float deltaTimeSeconds)
    {
        _server.PollEvents();
    }

    public void Listen()
    {
        _server.Start(_port);
    }
}