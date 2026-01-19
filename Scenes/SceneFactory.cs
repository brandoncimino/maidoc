using System;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

/// <remarks>
/// Separate <see cref="SceneSpawner{TSceneRoot,TInput}"/>s are in their own files,
/// but are combined into a single <c>partial class</c> so that all of the <see cref="PackedScene"/> fields can be assigned
/// to one <see cref="Node"/> in the editor.
/// <br/>
/// <br/>
/// Note that there must be at least one file whose name is exactly "<see cref="SceneFactory"/>.cs" for Godot to be able to open from the editor.
/// </remarks>
public partial class SceneFactory : Node2D {
    private readonly SceneSpawner<DuelRunner, DuelRunner.SpawnInput> _duelRunnerSpawner = new();

    public DuelRunner SpawnDuelRunner(DuelRunner.SpawnInput input) {
        _duelRunnerSpawner.GroupNode.TryEnfranchise(this);

        if (_duelRunnerSpawner.Instances.SingleOrDefault() is { } existing) {
            throw new InvalidOperationException(
                $"Cannot spawn multiple {nameof(DuelRunner)}s - one already exists: {existing}"
            );
        }

        return _duelRunnerSpawner.Spawn(input);
    }

    private readonly SceneSpawner<BoardView, BoardView.SpawnInput> _boardSpawner = new() {
        NamingConvention = (input, i) => $"Board {input.PlayerId}"
    };

    public BoardView SpawnPlayerBoard(
        BoardView.SpawnInput input
    ) {
        _boardSpawner.UseGroupNode(this);
        return _boardSpawner.Spawn(input);
    }

    public CellView SpawnCell(CellView.SpawnInput spawnInput) {
        _cellSpawner.UseGroupNode(this);

        return _cellSpawner.Spawn(spawnInput);
    }

    private readonly SceneSpawner<CellView, CellView.SpawnInput> _cellSpawner = new() {
        NamingConvention = (input, i) => $"Cell {input.MyCell}"
    };

    #region Card

    private readonly SceneSpawner<CardSceneRoot, CardSceneRoot.Input> _cardSpawner = new();

    public CardSceneRoot SpawnCard(
        CardSceneRoot.Input input
    ) {
        _cardSpawner.UseGroupNode(this);

        Require.Argument(
            input.MyCard,
            _cardSpawner.Instances.SingleOrDefault(it => it.MyCard == input.MyCard) is null,
            $"Can't spawn a {nameof(CardSceneRoot)} for {input.MyCard} because one already exists!"
        );

        return _cardSpawner.Spawn(input);
    }

    #endregion
}