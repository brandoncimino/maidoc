using Godot;

namespace maidoc.Scenes.GameComponents;

public partial class Deck : Node2D, ISceneRoot<Deck, Deck.SpawnInput> {
    public Deck InitializeSelf(SpawnInput input) {
        return this;
    }

    public static Deck InstantiateRawScene() {
        return new Deck();
    }

    public readonly record struct SpawnInput;
}