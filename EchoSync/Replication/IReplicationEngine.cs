namespace EchoSync.Replication
{
    public interface IReplicationEngine
    {
        public void RegisterNetObject(NetObject netObject);
        
        public void UnregisterNetObject(NetObject netObject);
        
        public bool HasAuthority();
    }
}