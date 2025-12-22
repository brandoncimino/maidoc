using maidoc.Core.NormalCreatures;

namespace maidoc.Scenes.GameComponents;

public partial class NormalCreatureFigure : SceneRoot2D<NormalCreatureFigure, NormalCreature>, ISceneRoot<NormalCreatureFigure, NormalCreature> {
    public NormalCreatureFigure InitializeSelf(NormalCreature input) {
        return this;
    }
}