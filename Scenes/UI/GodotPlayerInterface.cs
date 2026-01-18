using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.UI;

public partial class GodotPlayerInterface : CanvasLayer,
    ISceneRoot<GodotPlayerInterface, GodotPlayerInterface.SpawnInput> {
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

    public GodotPlayerInterface InitializeSelf(SpawnInput spawnInput) {
        _playerInterface.Enfranchise(spawnInput.PlayerInterface);

        if (DebugMenuEnabled) {
            this.SpawnChild<DebugMenu, SpawnInput>(spawnInput);
        }

        return this;
    }

    public readonly record struct SpawnInput {
        public required PlayerInterface PlayerInterface { get; init; }
        public required GodotBetween    GodotBetween    { get; init; }
        public required IPaperView      PaperView       { get; init; }
    }
}