using EchoSync.Transport;

namespace EchoSync
{
    public class EchoClient
    {
        private readonly IClient _client;
        
        public EchoClient(IClient client)
        {
            _client = client;
        }
    }
}