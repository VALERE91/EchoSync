using EchoSync.Serialization;

namespace EchoSyncTest;

class Vector3
{
    public float X;
    public float Y;
    public float Z;
}

class Quat
{
    public float X;
    public float Y;
    public float Z;
    public float W;
}

public class SerializationTest
{
    [SetUp]
    public void Setup()
    {
        EchoBitStream.RegisterType<Vector3>(
            (IBitWriter writer, ref BitStream bs, Vector3 vec) =>
            {
                writer.Write<float>(ref bs, vec.X);
                writer.Write<float>(ref bs, vec.Y);
                writer.Write<float>(ref bs, vec.Z);
            },
            (IBitReader reader, ref BitStream bs) => new Vector3
            {
                X = reader.Read<float>(ref bs),
                Y = reader.Read<float>(ref bs),
                Z = reader.Read<float>(ref bs)
            }
        );
        
        EchoBitStream.RegisterType<Quat>(
            (IBitWriter writer, ref BitStream bs, Quat quat) =>
            {
                //TODO : optimize serialization of Quat using Three-least compression
                writer.Write<float>(ref bs, quat.X);
                writer.Write<float>(ref bs, quat.Y);
                writer.Write<float>(ref bs, quat.Z);
                writer.Write<float>(ref bs, quat.W);
            },
            (IBitReader reader, ref BitStream bs) => new Quat
            {
                X = reader.Read<float>(ref bs),
                Y = reader.Read<float>(ref bs),
                Z = reader.Read<float>(ref bs),
                W = reader.Read<float>(ref bs)
            }
        );
    }
    
    [Test]
    public void CanSerializeVector3()
    {
        Random rand = new Random();
        var vector = new Vector3 { X = 1.0f, Y = 2.0f, Z = 3.0f };
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, vector);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedVector = reader.Read<Vector3>(ref readerBitStream);
        
        Assert.That(deserializedVector.X, Is.EqualTo(vector.X));
        Assert.That(deserializedVector.Y, Is.EqualTo(vector.Y));
        Assert.That(deserializedVector.Z, Is.EqualTo(vector.Z));
    }
    
    [Test]
    public void CanSerializeQuat()
    {
        var quat = new Quat { X = 1.0f, Y = 2.0f, Z = 3.0f, W = 4.0f };
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, quat);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedQuat = reader.Read<Quat>(ref readerBitStream);
        
        Assert.That(deserializedQuat.X, Is.EqualTo(quat.X));
        Assert.That(deserializedQuat.Y, Is.EqualTo(quat.Y));
        Assert.That(deserializedQuat.Z, Is.EqualTo(quat.Z));
        Assert.That(deserializedQuat.W, Is.EqualTo(quat.W));
    }

    [Test]
    public void CanSerializeInteger()
    {
        var value = 42;
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, value);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedValue = reader.Read<int>(ref readerBitStream);
        
        Assert.That(deserializedValue, Is.EqualTo(value));
    }
    
    [Test]
    public void CanSerializeFloat()
    {
        var value = 42.0f;
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, value);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedValue = reader.Read<float>(ref readerBitStream);
        
        Assert.That(deserializedValue, Is.EqualTo(value));
    }
    
    [Test]
    public void CanSerializeDouble()
    {
        var value = 42.0;
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, value);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedValue = reader.Read<double>(ref readerBitStream);
        
        Assert.That(deserializedValue, Is.EqualTo(value));
    }
    
    [Test]
    public void CanSerializeChar()
    {
        var value = 'A';
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, value);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedValue = reader.Read<char>(ref readerBitStream);
        
        Assert.That(deserializedValue, Is.EqualTo(value));
    }
    
    [Test]
    public void CanSerializeBool()
    {
        var value = true;
        
        Span<byte> buffer = stackalloc byte[64];
        
        var writerBitStream = new BitStream(buffer);
        var writer = new EchoBitStream();
        writer.Write(ref writerBitStream, value);
        
        var readerBitStream = new BitStream(buffer);
        var reader = new EchoBitStream();
        var deserializedValue = reader.Read<bool>(ref readerBitStream);
        
        Assert.That(deserializedValue, Is.EqualTo(value));
    }
}