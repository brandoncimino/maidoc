using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Godot;
using maidoc.Core;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class SceneFactory {
    [Export]
    public required PackedScene CellScene { get; set; }

    private readonly PackedSceneSpawner<CellView, CellView.SpawnInput> _cellSpawner = new();

    public void SpawnBoardCells(
        CellView.BoardSpawnInput input
    ) {
        var (boardGrid, cellSizeInMeters, onCellClick) = input;
        Helpers.ForEachCellIndex(
            boardGrid.Width,
            boardGrid.Height,
            (x, y) => {
                SpawnCell(
                    new CellView.SpawnInput() {
                        MyCell  = boardGrid[x, y],
                        OnClick = onCellClick,
                        RectInMeters = new Rect2(
                            cellSizeInMeters * new Vector2(x, y),
                            cellSizeInMeters
                        )
                    }
                );
            }
        );
    }

    private CellView SpawnCell(CellView.SpawnInput spawnInput) => _cellSpawner.Spawn(CellScene, spawnInput);
}