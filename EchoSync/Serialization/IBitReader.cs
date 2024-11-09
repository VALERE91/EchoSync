using System;

namespace EchoSync.Serialization
{
    public interface IBitReader
    {
        public T Read<T>(ref BitStream bitStream);
        
        public object Read(Type type, ref BitStream bitStream);
    }
}