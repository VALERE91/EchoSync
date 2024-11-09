namespace EchoSync.Replication
{
    public class DefaultReplicationEngine : IReplicationEngine
    {
        private readonly bool _authoritative;
        
        public DefaultReplicationEngine(bool authoritative)
        {
            _authoritative = authoritative;
        }
        
        public bool HasAuthority()
        {
            return _authoritative;
        }
    }
}