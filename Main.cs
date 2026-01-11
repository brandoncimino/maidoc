using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes;

namespace maidoc;

public partial class Main : Node2D {
    [Export]
    public required PackedScene DuelRunnerScene;

    private readonly LazyChild<SceneFactory> _sceneFactory = new();

    public override void _Ready() {
        // TODO: Early placeholder - eventually, the scene will be instantiated externally and the `Referee` will be provided by someone else.
        var decklist = Decklist.DevPlaceholder;

        var referee = Referee.PrepareFreshGame(
            new Dictionary<PlayerId, Decklist> {
                [PlayerId.Red]  = decklist,
                [PlayerId.Blue] = decklist
            },
            PlayerId.Red,
            new Ruleset()
        );

        var playerInterface = new PlayerInterface() {
            Referee = referee
        };

        DuelRunnerScene
            .Instantiate<DuelRunner>()
            .AsChildOf(this)
            .InitializeSelf(
                new DuelRunner.SpawnInput() {
                    BoardSpawnInput = new CellView.BoardSpawnInput {
                        BoardGrid        = referee.Board,
                        CellSizeInMeters = new Vector2(1.1f, 1.1f),
                        OnClick          = _ => { }
                    },
                    PlayerInterface = playerInterface,
                    SceneFactory    = _sceneFactory.Get(this)
                }
            );

        GD.Print("YOLO");
    }
}