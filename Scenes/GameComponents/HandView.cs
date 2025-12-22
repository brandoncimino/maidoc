using System;

namespace maidoc.Scenes.GameComponents;

public partial class HandView : SceneRoot2D<HandView, ValueTuple>, ISceneRoot<HandView, ValueTuple> {
    public HandView InitializeSelf(ValueTuple input) {
        return this;
    }
}