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
            Span<char> charSpan = "Hello, World!".ToCharArray();
            Span<byte> byteSpan = stackalloc byte[Encoding.UTF8.GetByteCount(charSpan)];
            Encoding.UTF8.GetBytes(charSpan, byteSpan);
            _client.Sender.SendPacket(0, Reliability.Reliable, byteSpan);
            
            if(_client.Receiver.HasData(0))
            {
                if(_client.Receiver.PeekLatest(0, out var data))
                {
                    var message = Encoding.UTF8.GetString(data);
                    Console.WriteLine($"Received message: {message}");
                    _client.Receiver.PopLatest(0);
                }
            }
            
            _client.Tick(deltaTimeSeconds);
        }

        public bool HasAuthority()
        {
            return false;
        }
    }
}