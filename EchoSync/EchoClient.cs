using System;
using EchoSync.Transport;

namespace EchoSync
{
    public class EchoClient : IDisposable, ITickable
    {
        private readonly IClient _client;
        
        public EchoClient(IClient client)
        {
            _client = client;
            _client.Connect();
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void Tick(float deltaTimeSeconds)
        {
            _client.Tick(deltaTimeSeconds);
        }
    }
}