using System;

namespace EchoSync.Serialization
{
    public ref struct BitStream
    {
        public Span<byte> Buffer { get; private set; }
        public int BytePosition  { get; private set; }
        public int BitPosition  { get; private set; }

        public BitStream(Span<byte> buffer)
        {
            Buffer = buffer;
            BytePosition = 0;
            BitPosition = 0;
        }

        public void WriteBits(int value, int bitCount)
        {
            for (int i = 0; i < bitCount; i++)
            {
                int bit = (value >> i) & 1;
                Buffer[BytePosition] |= (byte)(bit << BitPosition);
                BitPosition++;

                if (BitPosition == 8)
                {
                    BitPosition = 0;
                    BytePosition++;
                    if (BytePosition >= Buffer.Length)
                        throw new IndexOutOfRangeException("Buffer overflow while writing.");
                }
            }
        }

        public int ReadBits(int bitCount)
        {
            int value = 0;
            for (int i = 0; i < bitCount; i++)
            {
                if (BitPosition == 8)
                {
                    BitPosition = 0;
                    BytePosition++;
                    if (BytePosition >= Buffer.Length)
                        throw new IndexOutOfRangeException("Buffer overflow while reading.");
                }

                int bit = (Buffer[BytePosition] >> BitPosition) & 1;
                value |= (bit << i);
                BitPosition++;
            }

            return value;
        }

        public ReadOnlySpan<byte> ReadBytes(int byteCount)
        {
            ReadOnlySpan<byte> Data = Buffer.Slice(BytePosition, byteCount);
            BytePosition += byteCount;
            BitPosition = 0;
            return Data;
        }
    }
}