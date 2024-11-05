using System;
using EchoSync.Transport;

namespace EchoSync
{
    public class EchoServer : IDisposable, ITickable
    {
        private readonly IServer _server;
        
        public EchoServer(IServer server)
        {
            _server = server;
            
            _server.OnConnectionRequest += ConnectionRequestHandler;
            _server.OnClientConnected += ClientConnectedHandler;
            _server.OnClientDisconnected += ClientDisconnectedHandler;
            
            _server.Listen();
        }

        private void ClientDisconnectedHandler(IClient client)
        {
            Console.WriteLine($"Client {client.Identifier} disconnected");
        }

        private void ClientConnectedHandler(IClient client)
        {
            Console.WriteLine($"Client {client.Identifier} connected");
        }

        private bool ConnectionRequestHandler(string ip, int port, Span<byte> buffer)
        {
            Console.WriteLine($"Client check from {ip}:{port}");
            return true;
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public void Tick(float deltaTimeSeconds)
        {
            _server.Tick(deltaTimeSeconds);
        }
    }
}