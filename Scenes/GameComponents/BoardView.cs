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

    public BoardView InitializeSelf((BoardGrid, Action<BoardCell>) input) {
        throw new NotImplementedException();
    }
}