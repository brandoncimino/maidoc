using System;
using Godot;

namespace maidoc.Scenes;

public partial class InteractiveSceneRoot2D<TSelf, TInput> : Area2D
    where TSelf : InteractiveSceneRoot2D<TSelf, TInput>, ISceneRoot<TSelf, TInput> {
    private readonly Disenfranchised<TInput> _myInput = new();
    private readonly Disenfranchised<Action> _onClick = new();

    protected TInput MyInput => _myInput.Value;

    public TSelf Initialize(TInput input, Action? onClick = null) {
        var myself = (TSelf)this;
        _myInput.Enfranchise(input);
        myself.InitializeSelf(input);

        _onClick.Enfranchise(onClick ?? (() => { }));

        return myself;
    }

    public sealed override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx) {
        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left }) {
            _onClick.Value.Invoke();
        }

        if(@event is InputEventMouseButton { ButtonIndex: MouseButton.Right }) {}
    }
}