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
    
    private static readonly Func<NetObject<Character>> Factory = () => new Character();
    
    public Character() : base(Factory)
    {
        Console.WriteLine("Character created");
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