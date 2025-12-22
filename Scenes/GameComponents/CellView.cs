using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

public partial class CellView : Area2D, ISceneRoot<CellView, (BoardCell, Action<BoardCell>)> {
    private readonly Disenfranchised<BoardCell>         _model      = new();
    private readonly Disenfranchised<Sprite2D>          _background = new();
    private readonly Disenfranchised<Action<BoardCell>> _onClick    = new();

    [Signal]
    public delegate void OnCellClickedEventHandler(CellView cell);

    public CellView InitializeSelf((BoardCell, Action<BoardCell>) input) {
        var (model, onClick) = input;
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
}