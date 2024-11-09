namespace EchoSync.Replication
{
    public interface IServerRules
    {
        void PostLogin(IPlayer player);
        void SpawnPlayer(IPlayer player);
    }
}