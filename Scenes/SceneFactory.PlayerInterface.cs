using Godot;
using maidoc.Core;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class SceneFactory {
    [Export]
    public required PackedScene PlayerInterfaceScene { get; set; }

    private readonly PackedSceneSpawner<GodotPlayerInterface, PlayerInterface> _playerInterfaceSpawner = new();

    public GodotPlayerInterface SpawnPlayerInterface(PlayerInterface playerInterface) {
        return _playerInterfaceSpawner.Spawn(PlayerInterfaceScene, playerInterface);
    }
}