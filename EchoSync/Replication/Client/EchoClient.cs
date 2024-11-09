using System;
using System.Text;
using EchoSync.Transport;

namespace EchoSync.Replication.Client
{
    public class EchoClient : IDisposable, ITickable, IReplicationEngine
    {
        private readonly IClient _client;
        
        public EchoClient(IClient client, object connectionKey)
        {
            _client = client;
            _client.Connect(connectionKey);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public void Tick(float deltaTimeSeconds)
        {
            if(_client.Receiver.HasData(0))
            {
                if(_client.Receiver.PeekLatest(0, out var data))
                {
                    var message = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"Received snapshot");
                    _client.Receiver.PopLatest(0);
                }
            }
            
            _client.Tick(deltaTimeSeconds);
        }

        public void RegisterNetObject(NetObject netObject)
        {
            throw new NotImplementedException();
        }

        public void UnregisterNetObject(NetObject netObject)
        {
            throw new NotImplementedException();
        }

        public bool HasAuthority()
        {
            return false;
        }
    }
}