namespace EchoSync.Replication
{
    public enum FrameType
    {
        Prediction,
        Correction,
        Interpolation,
        Server
    }
    
    public interface IReplicationEngine
    {
        public void RegisterNetObject(NetObject netObject);
        
        public void UnregisterNetObject(NetObject netObject);
        
        public bool HasAuthority();
        
        public uint GetFrameNumber();
        
        public FrameType GetFrameType();
    }
}