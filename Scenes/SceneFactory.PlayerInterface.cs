using maidoc.Core;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class SceneFactory {
    private readonly PackedSceneSpawner<GodotPlayerInterface, PlayerInterface> _playerInterfaceSpawner = new() {
        PackedScenePath = "res://Scenes/UI/godot_player_interface.tscn"
    };

    public GodotPlayerInterface SpawnPlayerInterface(PlayerInterface playerInterface) {
        return _playerInterfaceSpawner.Spawn(playerInterface);
    }
}