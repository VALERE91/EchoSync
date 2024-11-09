using EchoSync;
using EchoSync.Replication;
using EchoSync.Serialization;

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
    
    public static Func<uint, NetObject<Character>> Factory() => (uint objectId) => new Character(objectId);
    
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
        // Update character logic here
    }

    public void Start()
    {
        // Initialize character here
    }
}