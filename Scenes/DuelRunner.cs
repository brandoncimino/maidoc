using Godot;
using maidoc.Core;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class DuelRunner : SceneRoot2D<DuelRunner, DuelRunner.SpawnInput>,
    ISceneRoot<DuelRunner, DuelRunner.SpawnInput> {
    private readonly Disenfranchised<GodotPlayerInterface> _playerInterface = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/duel_runner.tscn");

    public static DuelRunner InstantiateRawScene() {
        return PackedScene.Instantiate<DuelRunner>();
    }

    public DuelRunner InitializeSelf(SpawnInput input) {
        input.SceneFactory.SpawnBoardCells(input.BoardSpawnInput);

        _playerInterface.Enfranchise(input.SceneFactory.SpawnPlayerInterface(input.PlayerInterface));

        return this;
    }

    public readonly record struct SpawnInput {
        public required SceneFactory             SceneFactory    { get; init; }
        public required CellView.BoardSpawnInput BoardSpawnInput { get; init; }
        public required PlayerInterface          PlayerInterface { get; init; }
    }
}