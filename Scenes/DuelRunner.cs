using System;
using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class DuelRunner : SceneRoot2D<DuelRunner, Referee>, ISceneRoot<DuelRunner, Referee> {
    [Export]
    public PackedScene PlayerInterfaceScene;

    [Export]
    public PackedScene BoardScene;

    [Export]
    public PackedScene CardScene;

    [Export]
    public PackedScene HandScene;

    public DuelRunner InitializeSelf(Referee referee) {
        var playerInterface = new PlayerInterface() {
            Referee = referee
        };

        BoardScene
            .Instantiate<GameComponents.BoardView>()
            .Initialize((referee.Board, cell => playerInterface.TrySelect(cell)))
            .AsChildOf(this)
            ;

        foreach (var playerId in Enum.GetValues<PlayerId>()) {
            HandScene
                .Instantiate<HandView>()
                .Initialize(default)
                .AsChildOf(this)
                ;


        }

        PlayerInterfaceScene
            .Instantiate<UI.GodotPlayerInterface>()
            .Initialize(playerInterface)
            .AsChildOf(this)
            ;

        return this;
    }

    public void SpawnCard(CardView.Input input) {
        CardScene.Instantiate<CardView>()
            .Initialize(input)
            .AsChildOf(this)
            ;
    }
}