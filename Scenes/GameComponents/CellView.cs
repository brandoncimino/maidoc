using System;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes;

public partial class CellView : Area2D, ISceneRoot<CellView, CellView.SpawnInput> {
    private readonly Disenfranchised<CellAddress>         _address    = new();
    private readonly Disenfranchised<Sprite2D>            _background = new();
    private readonly Disenfranchised<Action<CellAddress>> _onClick    = new();

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/Cell.tscn");

    public static CellView InstantiateRawScene() {
        return PackedScene.Instantiate<CellView>();
    }

    [Signal]
    public delegate void OnCellClickedEventHandler(CellView cell);

    public CellView InitializeSelf(SpawnInput input) {
        _address.Enfranchise(input.MyCell);
        _background.Enfranchise(GetNode<Sprite2D>("Sprite2D"));

        Modulate = _address.Value.PlayerId switch {
            PlayerId.Red => Colors.Red,
            PlayerId.Blue => Colors.Blue,
            _ => throw new ArgumentOutOfRangeException(nameof(_address.Value.PlayerId), _address.Value.PlayerId, null)
        };

        _background.Value.Centered = true;
        _background.Value.Position = default;

        _onClick.Enfranchise(input.OnClick);

        this.AdjustSizeAndPosition(input.RectInMeters);

        return this;
    }

    public override void _Ready() {
        InputEvent += OnInputEvent;
    }

    private void OnInputEvent(Node viewport, InputEvent e, long shapeIdx) {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) {
            GD.Print($"CLICKING cell {this}");
            _onClick.Value(_address.Value);
        }
    }

    public readonly record struct SpawnInput {
        public required CellAddress         MyCell       { get; init; }
        public required Action<CellAddress> OnClick      { get; init; }
        public required Rect2               RectInMeters { get; init; }
    }
}