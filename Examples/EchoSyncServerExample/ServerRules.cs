using CommonGameplayCode;
using EchoSync.Inputs;
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

    public PlayerController PostLogin(Player player)
    {
        var playerController = new GamePlayerController();
        playerController.SetPlayer(player);
        return playerController;
    }

    public void SpawnPlayer(PlayerController playerController)
    {
        if (playerController.Player == null)
        {
            return;
        }
        
        _players.Add(playerController.Player.PlayerId, world.SpawnObject<Character>(playerController)!);
    }

    public void DespawnPlayer(Player player)
    {
        if (!_players.TryGetValue(player.PlayerId, out var playerCharacter)) return;
        world.UnspawnObject(playerCharacter);
    }
}