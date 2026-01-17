using System;
using Godot;

namespace maidoc.Scenes.GameComponents;

public partial class HandView : Node2D, ISceneRoot<HandView, ValueTuple> {
    public HandView InitializeSelf(ValueTuple input) {
        return this;
    }
}