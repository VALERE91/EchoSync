using EchoSync.Transport;

namespace EchoSync.Replication.Server
{
    public class Player
    {
        public int PlayerId { get; }

        public IPeer NetworkPeer { get; }

        public Player(int playerId, IPeer networkPeer)
        {
            PlayerId = playerId;
            NetworkPeer = networkPeer;
        }
    }
}