namespace EchoSync.Replication
{
    public class DefaultObjectIdGenerator : IObjectIdGenerator
    {
        private uint _serverCounter = 1;
        private uint _clientCounter = 1;
        
        public uint GenerateServerId()
        {
            uint id = _serverCounter++;
            return id & 0x7FFFFFFF;
        }

        public uint GenerateClientId()
        {
            uint id = _clientCounter++;
            return id | 0x80000000;
        }

        public bool IsClientGenerated(uint objectId)
        {
            return (objectId & 0x80000000) != 0;
        }

        public bool IsServerGenerated(uint objectId)
        {
            return (objectId & 0x80000000) == 0;
        }
    }
}