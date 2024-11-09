using EchoSync.Inputs;
using EchoSync.Replication;
using EchoSync.Replication.Server;

namespace EchoSyncTest.Mocks;

public class ServerRulesMock : IServerRules
{
    public bool Login(Span<byte> buffer)
    {
        return true;
    }

    public PlayerController PostLogin(Player player)
    {
        throw new NotImplementedException();
    }

    public void SpawnPlayer(PlayerController playerController)
    {
        
    }

    public void DespawnPlayer(Player player)
    {
        
    }
}