using Godot;
using maidoc.Core;

namespace maidoc.Scenes.UI;

public partial class GodotPlayerInterface : Node2D, ISceneRoot<GodotPlayerInterface, PlayerInterface> {
    [Export]
    public required PackedScene DebugMenuScene;

    [Export]
    public bool DebugMenuEnabled = true;

    private readonly Disenfranchised<PlayerInterface> _playerInterface = new();

    public override void _Ready() {

    }

    public GodotPlayerInterface InitializeSelf(PlayerInterface playerInterface) {
        _playerInterface.Enfranchise(playerInterface);

        if (DebugMenuEnabled) {
            GodotHelpers.Instantiate<DebugMenu, PlayerInterface>(DebugMenuScene, playerInterface, this);
            var debugMenu = DebugMenuScene.Instantiate<DebugMenu>();
            debugMenu.InitializeSelf(_playerInterface.Value);
        }

        return this;
    }
}