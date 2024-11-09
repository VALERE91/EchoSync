using CommonGameplayCode;
using EchoSync.Replication;
using EchoSync.Replication.Server;

namespace EchoSyncServerExample;

public class ServerRules(World world) : IServerRules
{
    private readonly Dictionary<int, Character> _players = new();

    public bool Login(Span<byte> buffer)
    {
        return true;
    }

    public void PostLogin(Player player)
    {
        
    }

    public void SpawnPlayer(Player player)
    {
        _players.Add(player.PlayerId, world.SpawnObject<Character>());
    }

    public void DespawnPlayer(Player player)
    {
        if (!_players.TryGetValue(player.PlayerId, out var playerCharacter)) return;
        world.UnspawnObject(playerCharacter);
    }
}