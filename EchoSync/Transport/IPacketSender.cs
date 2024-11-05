using System;

namespace EchoSync.Transport
{
    public enum Reliability
    {
        Unreliable,
        Sequenced,
        Reliable
    }
    
    public interface IPacketSender
    {
        public void SendPacket(int channel, Reliability reliability, ReadOnlySpan<byte> data);
    }
}