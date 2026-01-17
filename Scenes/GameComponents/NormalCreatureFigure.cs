using Godot;
using maidoc.Core.NormalCreatures;

namespace maidoc.Scenes.GameComponents;

public partial class NormalCreatureFigure : Node2D, ISceneRoot<NormalCreatureFigure, NormalCreature> {
    public NormalCreatureFigure InitializeSelf(NormalCreature input) {
        return this;
    }
}