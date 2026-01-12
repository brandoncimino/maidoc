using Godot;
using maidoc.Core;

namespace maidoc.Scenes.UI;

public partial class GodotPlayerInterface : Node2D, ISceneRoot<GodotPlayerInterface, PlayerInterface> {
    [Export]
    public bool DebugMenuEnabled = true;

    private readonly Disenfranchised<PlayerInterface> _playerInterface = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/UI/godot_player_interface.tscn");

    public static GodotPlayerInterface InstantiateRawScene() {
        return PackedScene.Instantiate<GodotPlayerInterface>();
    }

    public override void _Ready() { }

    public GodotPlayerInterface InitializeSelf(PlayerInterface playerInterface) {
        _playerInterface.Enfranchise(playerInterface);

        if (DebugMenuEnabled) {
            this.SpawnChild<DebugMenu, PlayerInterface>(playerInterface);
        }

        return this;
    }
}