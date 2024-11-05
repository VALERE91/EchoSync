using System.Diagnostics.CodeAnalysis;
using EchoSync.Transport;
using LiteNetLib;

namespace LiteNetLibAdapters.Receivers;

public class PeerReceiver : IPacketReceiver
{
    private readonly Dictionary<byte, Queue<NetPacketReader>> _packets = new();
    
    public void QueueData(NetPacketReader reader, byte channel)
    {
        if (!_packets.TryGetValue(channel, out var queue))
        {
            queue = new Queue<NetPacketReader>();
            _packets.Add(channel, queue);
        }

        queue.Enqueue(reader);
    }
    
    public bool HasData(int channel)
    {
        if (channel > byte.MaxValue)
        {
            return false;
        }
        
        if (!_packets.ContainsKey((byte)channel))
        {
            return false;
        }

        return _packets[(byte)channel].Count > 0;
    }

    public bool PeekLatest(int channel, out ReadOnlySpan<byte> data)
    {
        if (!_packets.TryGetValue((byte)channel, out var readers))
        {
            data = ReadOnlySpan<byte>.Empty;
            return false;
        }

        if (readers.Count <= 0)
        {
            data = ReadOnlySpan<byte>.Empty;
            return false;
        }
        
        NetPacketReader packet = readers.Peek();
        data = packet.GetRemainingBytes();
        return true;
    }

    public void PopLatest(int channel)
    {
        if (!_packets.TryGetValue((byte)channel, out var readers))
        {
            return;
        }
        
        if (readers.Count <= 0)
        {
            return;
        }

        NetPacketReader packet = readers.Dequeue();
        packet.Recycle();
    }
}