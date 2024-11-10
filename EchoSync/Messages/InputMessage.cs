using System;
using System.Collections.Generic;
using EchoSync.Inputs;
using EchoSync.Serialization;

namespace EchoSync.Messages
{
    public class InputMessage
    {
        public MessageTypes Type { get; set; } = MessageTypes.Input;

        public int FrameNumber { get; set; }

        public List<InputFrame> Frames { get; set; } = new List<InputFrame>();
        
        public Span<byte> Serialize(ref Span<byte> buffer, List<InputFrame> frames)
        {
            var inputStream = new BitStream(buffer);
            var inputWriter = new EchoBitStream();
            
            inputWriter.Write(ref inputStream, (byte)Type);
            inputWriter.Write<int>(ref inputStream, frames.Count);
            foreach (var frame in frames)
            {
                inputWriter.Write<uint>(ref inputStream, frame.FrameNumber);
                inputWriter.Write<int>(ref inputStream, frame.Values.Count);
                foreach (var input in frame.Values)
                {
                    inputWriter.Write<string>(ref inputStream, input.Key);
                    inputWriter.Write<uint>(ref inputStream, (uint)input.Value.Type);
                    switch (input.Value.Type)
                    {
                        case InputValueType.Boolean:
                            inputWriter.Write<bool>(ref inputStream, input.Value.BooleanValue);
                            break;
                        case InputValueType.Number:
                            inputWriter.Write<float>(ref inputStream, input.Value.NumberValue);
                            break;
                    }
                }
            }
            
            return buffer.Slice(0, inputStream.BytePosition);
        }
        
        public void Deserialize(ref Span<byte> buffer)
        {
            var inputBitStream = new BitStream(buffer);
            var inputReader = new EchoBitStream();

            var messageType = inputReader.Read<byte>(ref inputBitStream);
            FrameNumber = inputReader.Read<int>(ref inputBitStream);
            
            for(int frameIt = 0; frameIt < FrameNumber; frameIt++)
            {
                var frame = new InputFrame
                {
                    FrameNumber = inputReader.Read<uint>(ref inputBitStream),
                    Values = new Dictionary<string, InputValue>()
                };
                int inputCount = inputReader.Read<int>(ref inputBitStream);
                
                for (int i = 0; i < inputCount; i++)
                {
                    string inputName = inputReader.Read<string>(ref inputBitStream);
                    InputValueType type = (InputValueType)inputReader.Read<uint>(ref inputBitStream);
                    switch (type)
                    {
                        case InputValueType.Boolean:
                            frame.Values.TryAdd(inputName, new InputValue
                            {
                                Type = type,
                                BooleanValue = inputReader.Read<bool>(ref inputBitStream)
                            });
                            break;
                        case InputValueType.Number:
                            frame.Values.TryAdd(inputName, new InputValue
                            {
                                Type = type,
                                NumberValue = inputReader.Read<float>(ref inputBitStream)
                            });
                            break;
                    }
                }
                Frames.Add(frame);
            }
        }
    }
}