using EchoSync.Transport;

namespace EchoSync
{
    public class EchoServer
    {
        private readonly IServer _server;
        
        public EchoServer(IServer server)
        {
            _server = server;
        }
    }
}