using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes;

namespace maidoc;

public partial class Main : Node2D {
    private readonly LazyChild<SceneFactory> _sceneFactory = new(parent => new SceneFactory()
        .Named(nameof(SceneFactory))
        .AsChildOf(parent)
    );

    private DuelRunner? _duelRunner;

    public override void _Ready() {
        SpawnDuelRunner(Decklist.DevPlaceholder);

        GD.Print("YOLO");

        GD.Print(_duelRunner);
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventKey {
                Keycode: Key.Space
            } spaceKey) {
            GD.Print($"Spacebar pressed: {spaceKey}");
            GD.Print($"Current _duelRunner: {_duelRunner}");
            GD.Print($"SceneFactory: {_sceneFactory.Get(this)}");
            GD.Print($"-> children: {_sceneFactory.Get(this).GetChildren()}");
        }
    }

    private void SpawnDuelRunner(
        // TODO: Early placeholder - eventually, the scene will be instantiated externally and the `Referee` will be provided by someone else.
        Decklist decklist
    ) {
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

        var duelRunnerInput = new DuelRunner.SpawnInput() {
            BoardSpawnInput = new CellView.BoardSpawnInput {
                BoardGrid        = referee.Board,
                CellSizeInMeters = new Vector2(1.5f, 1.5f),
                OnClick          = _ => { }
            },
            PlayerInterface = playerInterface,
            SceneFactory    = _sceneFactory.Get(this)
        };

        _duelRunner = _sceneFactory.Get(this).SpawnDuelRunner(duelRunnerInput);
    }
}