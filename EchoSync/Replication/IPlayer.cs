namespace EchoSync.Replication
{
    public interface IPlayer
    {
        int PlayerId { get; }
        
        float PlayerLatency { get; }
        
        float PlayerPacketLoss { get; }
        
        float PlayerJitter { get; }
    }
}