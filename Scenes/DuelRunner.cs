using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class DuelRunner : Node2D, ISceneRoot<DuelRunner, DuelRunner.SpawnInput> {
    private readonly Disenfranchised<PlayerMap<BoardView>> _playerBoards = new();
    private readonly Disenfranchised<GodotBetween>         _godotBetween = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/duel_runner.tscn");

    public static DuelRunner InstantiateRawScene() {
        return PackedScene.Instantiate<DuelRunner>();
    }

    public DuelRunner InitializeSelf(SpawnInput input) {
        _godotBetween.Enfranchise(input.GodotBetween);

        _playerBoards.Enfranchise(() =>
            PlayerMap.Create(id =>
                input.SceneFactory.SpawnPlayerBoard(input.BoardSpawnInput with { PlayerId = id })
            )
        );

        _playerBoards.Value.Blue.RotationDegrees = 180;

        return this;
    }

    public override void _Process(double delta) {
        _godotBetween.Value.ConsumeAllEvents();
    }

    /// <summary>
    /// TODO: The responsibilities between <see cref="ActionManager"/>, <see cref="GodotBetween"/>, <see cref="Referee"/>, <see cref="PaperPusher"/>...it's a mess
    /// </summary>
    public readonly record struct SpawnInput {
        public required SceneFactory         SceneFactory    { get; init; }
        public required BoardView.SpawnInput BoardSpawnInput { get; init; }
        public required GodotBetween         GodotBetween    { get; init; }
        public required PaperPusher          PaperPusher     { get; init; }
    }
}