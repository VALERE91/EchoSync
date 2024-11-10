using System;

namespace EchoSync.Transport
{
    public interface IPacketReceiver
    {
        public bool HasData(int channel);

        public bool PeekLatest(int channel, out ReadOnlySpan<byte> data);

        public void PopLatest(int channel);
    }
}