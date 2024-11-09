using System;
using System.Collections.Generic;
using System.IO;

namespace EchoSync.Serialization
{
    public class EchoBitStream : IBitWriter, IBitReader
    {
        public delegate void WriteDelegate<in T>(IBitWriter writer, ref BitStream stream, T value);
        public delegate T ReadDelegate<out T>(IBitReader reader, ref BitStream stream);
        
        // Type registry for custom types
        private static readonly Dictionary<Type, (WriteDelegate<object> Write, ReadDelegate<object> Read)> _typeRegistry 
            = new Dictionary<Type, (WriteDelegate<object>, ReadDelegate<object>)>();

        // Register a custom type with write and read functions
        public static void RegisterType<T>(WriteDelegate<T> writeMethod, ReadDelegate<T> readMethod)
        {
            _typeRegistry[typeof(T)] = (
                (IBitWriter writer, ref BitStream bs, object obj) =>
                {
                    writeMethod(writer, ref bs, (T)obj);
                }, 
                (IBitReader reader, ref BitStream bs) =>
                {
                    return readMethod(reader, ref bs) ?? throw new InvalidOperationException("Unable to read object.");
                });
        }

        // Generic write method
        public void Write<T>(ref BitStream bitStream, T value)
        {
            if (value == null) return;
            
            if (_typeRegistry.TryGetValue(typeof(T), out var handlers))
            {
                handlers.Write(this, ref bitStream, value);
                return;
            }

            // Built-in types
            switch (value)
            {
                case bool b:
                    bitStream.WriteBits(b ? 1u : 0u, 1);
                    break;
                case int i:
                    bitStream.WriteBits((uint)i, 32);
                    break;
                case float f:
                {
                    int quantized = (int)Math.Truncate(f * 1000);
                    WriteBytes(ref bitStream, BitConverter.GetBytes(quantized));
                    break;
                }
                case double d:
                {
                    long quantized = (long)Math.Truncate(d * 1000);
                    WriteBytes(ref bitStream, BitConverter.GetBytes(quantized));
                    break;
                }
                case char c:
                    bitStream.WriteBits(c, 16);
                    break;
                case uint u:
                    bitStream.WriteBits((uint)u, 32);
                    break;
                default:
                    throw new NotSupportedException($"Type {typeof(T)} is not supported.");
            }
        }

        public void Write(Type type, ref BitStream bitStream, object value)
        {
            if (_typeRegistry.TryGetValue(type, out var handlers))
            {
                handlers.Write(this, ref bitStream, value);
                return;
            }

            // Built-in types
            if (type == typeof(bool))
            {
                bitStream.WriteBits((bool)value ? 1u : 0u, 1);
            }
            else if (type == typeof(int))
            {
                bitStream.WriteBits((uint)value, 32);
            }
            else if (type == typeof(float))
            {
                int quantized = (int)Math.Truncate((float)value * 1000);
                WriteBytes(ref bitStream, BitConverter.GetBytes(quantized));
            }
            else if (type == typeof(double))
            {
                long quantized = (long)Math.Truncate((float)value * 1000);
                WriteBytes(ref bitStream, BitConverter.GetBytes(quantized));
            }
            else if (type == typeof(char))
            {
                bitStream.WriteBits((char)value, 16);
            }
            else if (type == typeof(uint))
            {
                bitStream.WriteBits((uint)value, 32);
            }
            else
            {
                throw new NotSupportedException($"Type {type} is not supported.");
            }
        }

        // Generic read method
        public T Read<T>(ref BitStream bitStream)
        {
            if (_typeRegistry.TryGetValue(typeof(T), out var handlers))
            {
                return (T)handlers.Read(this, ref bitStream);
            }

            // Built-in types
            if (typeof(T) == typeof(bool))
                return (T)(object)(bitStream.ReadBits(1) != 0);
            if (typeof(T) == typeof(int))
                return (T)(object)bitStream.ReadBits(32);
            if (typeof(T) == typeof(float))
                return (T)(object)(BitConverter.ToInt32(ReadBytes(ref bitStream, 4))/1000f);
            if (typeof(T) == typeof(double))
                return (T)(object)(BitConverter.ToInt64(ReadBytes(ref bitStream, 8))/1000.0);
            if (typeof(T) == typeof(char))
                return (T)(object)(char)bitStream.ReadBits(16);
            if (typeof(T) == typeof(uint))
                return (T)(object)(uint)bitStream.ReadBits(32);

            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }

        public object Read(Type type, ref BitStream bitStream)
        {
            if (_typeRegistry.TryGetValue(type, out var handlers))
            {
                return handlers.Read(this, ref bitStream);
            }

            // Built-in types
            if (type == typeof(bool))
                return bitStream.ReadBits(1) != 0;
            if (type == typeof(int))
                return bitStream.ReadBits(32);
            if (type == typeof(float))
                return BitConverter.ToInt32(ReadBytes(ref bitStream, 4))/1000f;
            if (type == typeof(double))
                return BitConverter.ToInt64(ReadBytes(ref bitStream, 8))/1000.0;
            if (type == typeof(char))
                return (char)bitStream.ReadBits(16);
            if (type == typeof(uint))
                return (uint)bitStream.ReadBits(32);

            throw new NotSupportedException($"Type {type} is not supported.");
        }

        private void WriteBytes(ref BitStream bitStream, ReadOnlySpan<byte> bytes)
        {
            foreach (var b in bytes)
            {
                bitStream.WriteBits(b, 8);
            }
        }

        private ReadOnlySpan<byte> ReadBytes(ref BitStream bitStream, int byteCount)
        {
            return bitStream.ReadBytes(byteCount);
        }
    }
}