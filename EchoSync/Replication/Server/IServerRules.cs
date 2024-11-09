using System;

namespace EchoSync.Replication.Server
{
    public interface IServerRules
    {
        bool Login(Span<byte> buffer);
        void PostLogin(Player player);
        void SpawnPlayer(Player player);
        void DespawnPlayer(Player player);
    }
}