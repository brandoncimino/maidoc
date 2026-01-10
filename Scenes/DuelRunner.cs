using System;
using System.Collections.Generic;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.GameComponents;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public partial class DuelRunner : SceneRoot2D<DuelRunner, DuelRunner.SpawnInput>, ISceneRoot<DuelRunner, DuelRunner.SpawnInput> {
    [Export]
    public PackedScene PlayerInterfaceScene;

    [Export]
    public PackedScene BoardScene;

    [Export]
    public PackedScene CardScene;

    [Export]
    public PackedScene HandScene;

    private readonly Disenfranchised<GodotPlayerInterface> _playerInterface = new();

    public DuelRunner InitializeSelf(SpawnInput input) {
        var playerInterface = new PlayerInterface() {
            Referee = input.Referee
        };

        MyInput.SceneFactory.SpawnBoardCells(input.BoardSpawnInput);

        _playerInterface.Enfranchise(
            new GodotPlayerInterface() {
                
            }
            );

        PlayerInterfaceScene
            .Instantiate<UI.GodotPlayerInterface>()
            .Initialize(playerInterface)
            .AsChildOf(this)
            ;

        return this;
    }

    public readonly record struct SpawnInput(
        SceneFactory SceneFactory,
        Referee      Referee,
        CellView.BoardSpawnInput BoardSpawnInput
    );
}