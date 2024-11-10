using System;
using EchoSync.Inputs;

namespace EchoSync.Replication.Server
{
    public interface IServerRules
    {
        bool Login(Span<byte> buffer);
        PlayerController PostLogin(Player player);
        void SpawnPlayer(PlayerController playerController);
        void DespawnPlayer(Player player);
    }
}