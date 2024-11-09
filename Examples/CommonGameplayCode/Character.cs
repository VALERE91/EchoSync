using EchoSync;
using EchoSync.Replication;
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
    [NetProperty] 
    public float Health { get; protected set; }

    [NetProperty]
    public float Mana { get; protected set; }
    
    [NetProperty]
    public Vector3 Position { get; protected set; }
    
    private Vector3 _speed = new Vector3 { X = 1, Y = 1, Z = 1 };
    
    public static Func<uint, NetObject<Character>> Factory() => (uint objectId) =>
    {
        IWorld world = ServiceLocator.Get<IWorld>();
        return world.SpawnObject<Character>();;
    };
    
    public Character() : base(Factory())
    {
        Console.WriteLine("Character created");
    }

    public Character(uint objectId) : base(objectId)
    {
        Console.WriteLine("Character created from Linking Context");
    }
    
    public void Tick(float deltaTimeSeconds)
    {
        if (!HasAuthority())
        {
            Console.WriteLine("Health: {3} | Mana : {4} | Position: {0}, {1}, {2}", Position.X, 
                Position.Y, 
                Position.Z, 
                Health, 
                Mana);
            return;
        }
        
        // Update character logic here
        Position = new Vector3
        {
            X = Position.X + _speed.X * deltaTimeSeconds, 
            Y = Position.Y + _speed.Y * deltaTimeSeconds, 
            Z = Position.Z + _speed.Z * deltaTimeSeconds
        };
    }

    public void Start()
    {
        // Initialize character here
    }
}