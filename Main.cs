using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes;
using maidoc.Scenes.GameComponents;

namespace maidoc;

public partial class Main : Node {
    private readonly LazyChild<SceneFactory> _sceneFactory = new(parent => new SceneFactory()
        .Named(nameof(SceneFactory))
        .AsChildOf(parent)
    );

    private DuelRunner? _duelRunner;

    public override void _Ready() {
        SpawnDuelRunner(Decklist.DevPlaceholder, new Ruleset());

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
        Decklist decklist,
        Ruleset  ruleset
    ) {
        var paperPusher = new PaperPusher(ruleset.LaneCount);
        var referee = Referee.PrepareFreshGame(
            new Dictionary<PlayerId, Decklist> {
                [PlayerId.Red]  = decklist,
                [PlayerId.Blue] = decklist
            },
            PlayerId.Red,
            ruleset,
            paperPusher
        );

        var playerInterface = new PlayerInterface() {
            Referee = referee
        };

        var godotBetween = new GodotBetween() {
            Referee = referee
        };

        var duelRunnerInput = new DuelRunner.SpawnInput() {
            BoardSpawnInput = new BoardView.SpawnInput {
                PlayerId         = default,
                CellSizeInMeters = new Vector2(1.5f, 1.5f),
                OnCellClick      = playerInterface.ClickCell,
                LaneCount        = 4
            },
            PlayerInterface = playerInterface,
            SceneFactory    = _sceneFactory.Get(this),
            GodotBetween    = godotBetween,
            PaperPusher     = paperPusher
        };

        _duelRunner = _sceneFactory.Get(this).SpawnDuelRunner(duelRunnerInput);
    }
}