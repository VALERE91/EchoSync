using System;

namespace EchoSync.Transport
{
    public interface IClient : IDisposable, ITickable, IPeer
    {
        public void Connect(object connectionKey);
    }
}