using System;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes.GameComponents;

public partial class BoardView : SceneRoot2D<BoardView, (BoardGrid, Action<BoardCell>)>, ISceneRoot<BoardView, (
    BoardGrid, Action<BoardCell>)> {
    [Export]
    public required PackedScene CellScene;

    public static readonly Vector2                    CellSizeInMeters = 1.5f.ToVector2();

    public override void _Ready() {
        GD.Print($"Board is READY with: {MyInput}");
    }

    private void InstantiateCell(BoardCell boardCell, Action<BoardCell> onCellClicked) {
        CellScene
            .Instantiate<CellView>()
            .Initialize((boardCell, onCellClicked))
            .AsChildOf(this)
            .AdjustSizeAndPosition(
                new Rect2(
                    CellSizeInMeters * new Vector2(boardCell.Coord.X, boardCell.Coord.Y),
                    CellSizeInMeters
                )
            );
    }

    public BoardView InitializeSelf((BoardGrid, Action<BoardCell>) input) {

        Helpers.ForEachCellIndex(
            input.Item1.Width,
            input.Item1.Height,
            (x, y) => { InstantiateCell(input.Item1[x,y], input.Item2); }
        );

        return this;
    }
}