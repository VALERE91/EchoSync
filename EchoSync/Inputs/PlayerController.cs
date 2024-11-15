﻿using System;
using System.Collections.Generic;
using System.Linq;
using EchoSync.Messages;
using EchoSync.Replication;
using EchoSync.Replication.Server;
using EchoSync.Serialization;
using EchoSync.Transport;
using EchoSync.Utils;

namespace EchoSync.Inputs
{
    public struct InputFrame
    {
        public uint FrameNumber { get; set; }

        public Dictionary<string, InputValue> Values { get; set; }
    }

    public enum InputValueType
    {
        Number,
        Boolean
    }
    
    public struct InputValue
    {
        public InputValueType Type { get; set; }
        
        public bool BooleanValue { get; set; }
        
        public float NumberValue { get; set; }
    }
    
    public abstract class PlayerController : ITickable
    {
        public delegate void OnInputFiredDelegate(InputValue value);
        
        public Player? Player { get; private set; }

        //Should use a circular buffer
        private readonly LinkedList<InputFrame> _inputFrameHistory = new LinkedList<InputFrame>();
        private InputFrame _currentInputFrame;
        private readonly IReplicationEngine _replicationEngine = ServiceLocator.Get<IReplicationEngine>();

        private readonly Dictionary<string, List<OnInputFiredDelegate>> _inputListeners = 
            new Dictionary<string, List<OnInputFiredDelegate>>();

        private uint _latestFramePlayed = 0;
        private uint _currentFrame = 0;

        protected PlayerController()
        {
            _currentFrame = 0;
            _currentInputFrame = new InputFrame
            {
                FrameNumber = _currentFrame,
                Values = new Dictionary<string, InputValue>()
            };
        }
        
        public void SetPlayer(Player player)
        {
            Player = player;
        }

        public void InputReceived(ref Span<byte> inputs)
        {
            //Deserialize the input frame
            InputMessage message = new InputMessage();
            message.Deserialize(ref inputs);

            foreach (var inputFrame in message.Frames)
            {
                if (_inputFrameHistory.Any(frame => frame.FrameNumber == inputFrame.FrameNumber))
                {
                    continue;
                }
                _inputFrameHistory.AddLast(inputFrame);   
            }
        }

        public void AddInputHandler(string inputName, OnInputFiredDelegate onInputFired)
        {
            if (!_inputListeners.ContainsKey(inputName))
            {
                _inputListeners.Add(inputName, new List<OnInputFiredDelegate>());
            }
            _inputListeners[inputName].Add(onInputFired);
        }
        
        public void RemoveInputHandler(string inputName, OnInputFiredDelegate onInputFired)
        {
            if (!_inputListeners.ContainsKey(inputName))
            {
                return;
            }
            _inputListeners[inputName].Remove(onInputFired);
        }
        
        public void Tick(float deltaTimeSeconds)
        {
            _currentFrame++;
            if (_replicationEngine.HasAuthority())
            {
                InputFrame inputFrame = _inputFrameHistory
                    .FirstOrDefault(inputFrame => inputFrame.FrameNumber == _latestFramePlayed + 1);
                if (inputFrame.FrameNumber == 0)
                {
                    return;
                }

                _latestFramePlayed = inputFrame.FrameNumber;
                foreach (var input in inputFrame.Values)
                {
                    if (!_inputListeners.TryGetValue(input.Key, out var inputListener)) continue;
                    foreach (var listener in inputListener)
                    {
                        listener(input.Value);
                    }
                }
                
                //Clean history of all the frames that have been played
                while (_inputFrameHistory.Count > 0 && _inputFrameHistory.First.Value.FrameNumber <= _latestFramePlayed)
                {
                    _inputFrameHistory.RemoveFirst();
                }
            }
            else
            {
                _currentInputFrame.FrameNumber = _currentFrame;
                _inputFrameHistory.AddLast(_currentInputFrame);
                _currentInputFrame = new InputFrame
                {
                    FrameNumber = _currentFrame,
                    Values = new Dictionary<string, InputValue>()
                };
                while (_inputFrameHistory.Count > 10)
                {
                    _inputFrameHistory.RemoveFirst();
                }
                //Create and send the input frame
                Span<byte> buffer = stackalloc byte[1000];
                InputMessage message = new InputMessage
                {
                    FrameNumber = _inputFrameHistory.Count
                };
                buffer = message.Serialize(ref buffer, _inputFrameHistory.ToList());
                if (Player == null)
                {
                    throw new Exception("PLAYER IS NULL");
                }
                    
                Player.NetworkPeer.Sender.SendPacket(1, Reliability.Unreliable, buffer);
            }
        }

        private Span<byte> SerializeInputs(ref Span<byte> buffer)
        {
            BitStream bitStream = new BitStream(buffer);
            EchoBitStream writer = new EchoBitStream();
            writer.Write<int>(ref bitStream, _inputFrameHistory.Count);
            foreach (var inputFrame in _inputFrameHistory)
            {
                SerializeInputFrame(inputFrame, ref bitStream, writer);
            }
            return buffer.Slice(0, bitStream.BytePosition);
        }
        
        private void SerializeInputFrame(InputFrame frame, ref BitStream bitStream, IBitWriter writer)
        {
            writer.Write<uint>(ref bitStream, frame.FrameNumber);
            writer.Write<int>(ref bitStream, frame.Values.Count);
            foreach (var input in frame.Values)
            {
                writer.Write<string>(ref bitStream, input.Key);
                writer.Write<uint>(ref bitStream, (uint)input.Value.Type);
                switch (input.Value.Type)
                {
                    case InputValueType.Boolean:
                        writer.Write<bool>(ref bitStream, input.Value.BooleanValue);
                        break;
                    case InputValueType.Number:
                        writer.Write<float>(ref bitStream, input.Value.NumberValue);
                        break;
                }
            }
        }
        
        private InputFrame DeserializeInputFrame(ref BitStream bitStream, IBitReader writer)
        {
            InputFrame frame = new InputFrame
            {
                FrameNumber = writer.Read<uint>(ref bitStream),
                Values = new Dictionary<string, InputValue>()
            };
            int inputCount = writer.Read<int>(ref bitStream);
            
            for (int i = 0; i < inputCount; i++)
            {
                string inputName = writer.Read<string>(ref bitStream);
                InputValueType type = (InputValueType)writer.Read<uint>(ref bitStream);
                switch (type)
                {
                    case InputValueType.Boolean:
                        frame.Values.TryAdd(inputName, new InputValue
                        {
                            Type = type,
                            BooleanValue = writer.Read<bool>(ref bitStream)
                        });
                        break;
                    case InputValueType.Number:
                        frame.Values.TryAdd(inputName, new InputValue
                        {
                            Type = type,
                            NumberValue = writer.Read<float>(ref bitStream)
                        });
                        break;
                }
            }
            
            return frame;
        }
        
        private void DeserializeInputs(Span<byte> buffer)
        {
            BitStream bitStream = new BitStream(buffer);
            EchoBitStream reader = new EchoBitStream();
            int frameCount = reader.Read<int>(ref bitStream);
            for (int i = 0; i < frameCount; i++)
            {
                InputFrame frame = DeserializeInputFrame(ref bitStream, reader);
                //Check if we don't already have the frame
                if (_inputFrameHistory.Any(inputFrame => inputFrame.FrameNumber == frame.FrameNumber))
                {
                    continue;
                }
                _inputFrameHistory.AddLast(frame);   
            }
        }
        
        public void AddInput(string inputName, InputValue value)
        {
            _currentInputFrame.Values.TryAdd(inputName, value);
        }
    }
}