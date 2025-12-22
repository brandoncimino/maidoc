using Godot;

namespace maidoc.Scenes;

public partial class SceneRoot2D<TSelf, TInput> : Node2D where TSelf : SceneRoot2D<TSelf, TInput>, ISceneRoot<TSelf, TInput> {
    private readonly Disenfranchised<TInput> _myInput = new();

    protected TInput MyInput                  => _myInput.Value;

    public TSelf Initialize(TInput input) {
        var myself = (TSelf)this;
        _myInput.Enfranchise(input);
        myself.InitializeSelf(input);
        return myself;
    }
}