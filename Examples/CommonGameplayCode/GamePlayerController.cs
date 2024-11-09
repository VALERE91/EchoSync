using EchoSync.Inputs;
using EchoSync.Replication;
using EchoSync.Replication.Server;

namespace CommonGameplayCode;

public class GamePlayerController : PlayerController
{
    public static Func<uint, NetObject<PlayerController>> Factory => objectId => new GamePlayerController(objectId);
    
    public GamePlayerController() : base(Factory)
    {
    }

    public GamePlayerController(uint objectId) : base(objectId)
    {
    }
}