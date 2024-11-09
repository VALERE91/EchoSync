using System;
using System.Collections.Generic;
using EchoSync.Replication;
using EchoSync.Replication.Server;
using EchoSync.Serialization;

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
        InputValueType Type { get; set; }
        
        bool BooleanValue { get; set; }
        
        float NumberValue { get; set; }
    }
    
    public abstract class PlayerController : NetObject<PlayerController>, ITickable
    {
        public Player? Player { get; private set; }

        [NetProperty]
        public uint PlayerObjectId { get; private set; }

        public Dictionary<uint, InputFrame> InputFrames { get; private set; } = new Dictionary<uint, InputFrame>();
        
        protected PlayerController(Func<uint, NetObject<PlayerController>> factory) : base(factory)
        {
        }

        protected PlayerController(uint objectId) : base(objectId)
        {
        }
        
        public void SetPlayer(Player player)
        {
            Player = player;
        }

        public void Possess(NetObject playerObject)
        {
            
        }
        
        public void Unpossess(NetObject playerObject)
        {
            
        }
        
        public void Tick(float deltaTimeSeconds)
        {
            if (HasAuthority())
            {
                //Queue up the input frame
            }
            else
            {
                //Create and send the input frame
            }
        }
    }
}