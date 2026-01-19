using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes;
using maidoc.Scenes.GameComponents;
using maidoc.Scenes.UI;

namespace maidoc;

public partial class Main : Node {
    [Export]
    public bool DebugMenuEnabled { get; set; } = true;

    private readonly LazyChild<SceneFactory> _sceneFactory = new(parent => new SceneFactory()
        .Named(nameof(SceneFactory))
        .AsChildOf(parent)
    );

    private readonly LazyChild<GodotPlayerInterface> _godotPlayerInterface = new();

    private DuelRunner? _duelRunner;

    public override void _Ready() {
        SpawnDuelRunner(Decklist.DevPlaceholder, new Ruleset());

        GD.Print("YOLO");

        GD.Print(_duelRunner);
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

        var playerInterface = new ActionManager() {
            Referee = referee
        };

        var godotBetween = new GodotBetween() {
            Referee              = referee,
            GodotPlayerInterface = _godotPlayerInterface.Get(this)
        };

        var duelRunnerInput = new DuelRunner.SpawnInput() {
            BoardSpawnInput = new BoardView.SpawnInput {
                PlayerId         = default,
                CellSizeInMeters = new Vector2(1.5f, 1.5f),
                OnCellClick      = playerInterface.ClickCell,
                LaneCount        = 4
            },
            SceneFactory = _sceneFactory.Get(this),
            GodotBetween = godotBetween,
            PaperPusher  = paperPusher
        };

        _duelRunner = _sceneFactory.Get(this).SpawnDuelRunner(duelRunnerInput);

        if (DebugMenuEnabled) {
            _godotPlayerInterface.Get(this).SpawnChild<DebugMenu, DebugMenu.SpawnInput>(
                new DebugMenu.SpawnInput() {
                    GodotBetween = godotBetween,
                    PaperView    = paperPusher
                }
            );
        }
    }
}