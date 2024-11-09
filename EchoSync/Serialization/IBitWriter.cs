using System;

namespace EchoSync.Serialization
{
    public interface IBitWriter
    {
        public void Write<T>(ref BitStream bitStream, T value);
        
        public void Write(Type type, ref BitStream bitStream, object value);
    }
}