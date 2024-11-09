using EchoSync.Replication;
using EchoSync.Replication.Server;

namespace EchoSyncTest.Mocks;

public class ServerRulesMock : IServerRules
{
    public bool Login(Span<byte> buffer)
    {
        return true;
    }

    public void PostLogin(Player player)
    {
        
    }

    public void SpawnPlayer(Player player)
    {
        
    }

    public void DespawnPlayer(Player player)
    {
        
    }
}