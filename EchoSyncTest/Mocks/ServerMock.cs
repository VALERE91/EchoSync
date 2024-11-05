using EchoSync.Transport;

namespace EchoSyncTest.Mocks;

public class ServerMock : IServer
{
    //Temporary implementation, will be expanded upon
#pragma warning disable CS0067
    public event IServer.OnClientCheckDelegate? OnClientCheck;
    public event IServer.OnClientConnectedDelegate? OnClientConnected;
    public event IServer.OnClientDisconnectedDelegate? OnClientDisconnected;
#pragma warning restore CS0067
}