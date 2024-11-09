namespace EchoSync.Replication
{
    public class ReplicationEngine : IReplicationEngine
    {
        private readonly bool _authoritative;
        
        public ReplicationEngine(bool authoritative)
        {
            _authoritative = authoritative;
        }
        
        public bool HasAuthority()
        {
            return _authoritative;
        }
    }
}