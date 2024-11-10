using System.Diagnostics.CodeAnalysis;
using EchoSync.Transport;

namespace EchoSyncTest.Mocks.Transport;

public class PacketReceiverMock : IPacketReceiver
{
    public bool HasData(int channel)
    {
        return false;
    }

    public bool PeekLatest(int channel, [UnscopedRef] out ReadOnlySpan<byte> data)
    {
        data = default;
        return false;
    }

    public void PopLatest(int channel)
    {
        
    }
}