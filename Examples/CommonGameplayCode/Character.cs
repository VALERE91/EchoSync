﻿using EchoSync;
using EchoSync.Inputs;
using EchoSync.Replication;
using EchoSync.Replication.Server;
using EchoSync.Serialization;
using EchoSync.Utils;

namespace CommonGameplayCode;

public struct Vector3
{
    public float X;
    public float Y;
    public float Z;
}

public class Character : NetObject<Character>, IWorldObject
{
    private readonly PlayerController _controller;

    [NetProperty] 
    public uint Health { get; protected set; }

    [NetProperty]
    public uint Mana { get; protected set; }
    
    [NetProperty]
    public Vector3 Position { get; protected set; }
    
    private Vector3 _speed = new Vector3 { X = 0, Y = 0, Z = 0 };
    
    public static Func<uint, NetObject<Character>> Factory() => (uint objectId) =>
    {
        IWorld world = ServiceLocator.Get<IWorld>();
        var playerController = world.SpawnObject<GamePlayerController>()!;
        IReplicationEngine replicationEngine = ServiceLocator.Get<IReplicationEngine>();
        var player = new Player(0, replicationEngine.GetLocalPeer());
        playerController.SetPlayer(player);
        return world.SpawnObject<Character>(objectId, playerController)!;
    };
    
    public Character(PlayerController controller) : base(Factory())
    {
        _controller = controller;
    }

    public Character(uint objectId, PlayerController controller) : base(objectId)
    {
        _controller = controller;
        Console.WriteLine("Character created from Linking Context");
    }
    
    public void Tick(float deltaTimeSeconds)
    {
        var rand = new Random();
        if (!HasAuthority())
        {
            _controller.AddInput("move_x", new InputValue { Type = InputValueType.Number, NumberValue = rand.NextSingle() });
            _controller.AddInput("move_y", new InputValue { Type = InputValueType.Number, NumberValue = rand.NextSingle() });
            
            Console.WriteLine("Position: " + Position.X + ", " + Position.Y + ", " + Position.Z);
            
            return;
        }
        
        // Update character logic here
        Position = new Vector3
        {
            X = Position.X + _speed.X * deltaTimeSeconds, 
            Y = Position.Y + _speed.Y * deltaTimeSeconds, 
            Z = Position.Z + _speed.Z * deltaTimeSeconds
        };

        Health = (uint)rand.Next(0, 10);
        Mana = (uint)rand.Next(0, 10);
    }

    public void Start()
    {
        // Initialize character here
        _controller.AddInputHandler("move_x", (value) =>
        {
            if (value.Type != InputValueType.Number) return;
            _speed.X += value.NumberValue;
        });
        _controller.AddInputHandler("move_y", (value) =>
        {
            if (value.Type != InputValueType.Number) return;
            _speed.Y += value.NumberValue;
        });
    }
}