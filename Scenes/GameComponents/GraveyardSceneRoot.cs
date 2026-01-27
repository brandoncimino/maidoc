using Godot;

namespace maidoc.Scenes.GameComponents;

public partial class GraveyardSceneRoot : Node2D, ISceneRoot<GraveyardSceneRoot, GraveyardSceneRoot.SpawnInput> {
    public readonly record struct SpawnInput;

    public GraveyardSceneRoot InitializeSelf(SpawnInput input) {
        return new GraveyardSceneRoot();
    }
}