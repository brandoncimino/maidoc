using Godot;
using maidoc.Core;
using maidoc.Scenes.GameComponents;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class DuelRunner : Node2D, ISceneRoot<DuelRunner, DuelRunner.SpawnInput> {
    private readonly Disenfranchised<GodotPlayerInterface> _playerInterface = new();
    private readonly Disenfranchised<PlayerMap<BoardView>> _playerBoards    = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/duel_runner.tscn");

    public static DuelRunner InstantiateRawScene() {
        return PackedScene.Instantiate<DuelRunner>();
    }

    public DuelRunner InitializeSelf(SpawnInput input) {
        _playerInterface.Enfranchise(() => input.SceneFactory.SpawnPlayerInterface(input.PlayerInterface));

        _playerBoards.Enfranchise(() =>
            PlayerMap.Create(id =>
                input.SceneFactory.SpawnPlayerBoard(input.BoardSpawnInput with { PlayerId = id })
            )
        );

        _playerBoards.Value.Blue.RotationDegrees = 180;

        return this;
    }

    public readonly record struct SpawnInput {
        public required SceneFactory         SceneFactory    { get; init; }
        public required BoardView.SpawnInput BoardSpawnInput { get; init; }
        public required PlayerInterface      PlayerInterface { get; init; }
    }
}