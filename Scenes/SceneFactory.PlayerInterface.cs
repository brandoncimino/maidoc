using maidoc.Core;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class SceneFactory {
    private readonly SceneSpawner<GodotPlayerInterface, PlayerInterface> _playerInterfaceSpawner = new();

    public GodotPlayerInterface SpawnPlayerInterface(PlayerInterface playerInterface) {
        _playerInterfaceSpawner.GroupNode.TryEnfranchise(this);

        return _playerInterfaceSpawner.Spawn(playerInterface);
    }
}