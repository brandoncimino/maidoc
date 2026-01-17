using System;
using Godot;

namespace maidoc.Scenes;

[Obsolete(
    "Not much gain in exchange for needing to remember to call the base `InitializeSelf`, and essentially requiring you to write the type signature twice (due to the curiously-repeating-type pattern)"
)]
public partial class SceneRoot2D<TSelf, TInput> : Node2D, ISceneRoot<TSelf, TInput>
    where TSelf : SceneRoot2D<TSelf, TInput>, ISceneRoot<TSelf, TInput> {
    private readonly Disenfranchised<TInput> _myInput = new();

    protected TInput MyInput => _myInput.Value;

    public TSelf InitializeSelf(TInput input) {
        var myself = (TSelf)this;
        _myInput.Enfranchise(input);
        myself.InitializeSelf(input);
        return myself;
    }
}