using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

public partial class CellView : Area2D, ISceneRoot<CellView, CellView.SpawnInput> {
    private readonly Disenfranchised<BoardCell>         _model      = new();
    private readonly Disenfranchised<Sprite2D>          _background = new();
    private readonly Disenfranchised<Action<BoardCell>> _onClick    = new();

    [Signal]
    public delegate void OnCellClickedEventHandler(CellView cell);

    public CellView InitializeSelf(SpawnInput input) {
        var (model, onClick, rectInMeters) = input;
        _model.Enfranchise(model);
        _background.Enfranchise(GetNode<Sprite2D>("Sprite2D"));

        Modulate = model.OwnerId switch {
            PlayerId.Red  => Colors.Red,
            PlayerId.Blue => Colors.Blue,
            _               => throw new ArgumentOutOfRangeException(nameof(model.OwnerId), model.OwnerId, null)
        };

        _background.Value.Centered = true;
        _background.Value.Position = default;

        _onClick.Enfranchise(onClick);

        this.AdjustSizeAndPosition(rectInMeters);

        return this;
    }

    public override void _Ready() {
        InputEvent += OnInputEvent;
    }

    private void OnInputEvent(Node viewport, InputEvent e, long shapeIdx) {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) {
            _onClick.Value(_model.Value);
        }
    }

    public readonly record struct SpawnInput(
        BoardCell         MyCell,
        Action<BoardCell> OnClick,
        Rect2 RectInMeters
    );

    /// <summary>
    /// The input for spawning <i>multiple</i> <see cref="CellView"/>s aligned with each other.
    /// </summary>
    /// <param name="CellSizeInMeters"></param>
    /// <param name="OnClick"></param>
    public readonly record struct BoardSpawnInput(
        BoardGrid BoardGrid,
        Vector2           CellSizeInMeters,
        Action<BoardCell> OnClick
    );
}