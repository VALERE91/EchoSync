namespace EchoSync.Replication
{
    public interface IObjectIdGenerator
    {
        public uint GenerateServerId();

        public uint GenerateClientId();

        public bool IsClientGenerated(uint objectId);

        public bool IsServerGenerated(uint objectId);
    }
}