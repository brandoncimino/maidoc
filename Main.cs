using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes;

namespace maidoc;

public partial class Main : Node2D
{
    [Export]
    public required PackedScene DuelRunnerScene;



    public override void _Ready()
    {
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

        DuelRunnerScene
            .Instantiate<DuelRunner>()
            .AsChildOf(this)
            .InitializeSelf(referee);

        GD.Print("YOLO");
    }
}
