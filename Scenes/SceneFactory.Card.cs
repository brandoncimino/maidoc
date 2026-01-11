using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class SceneFactory : Node2D {
    private readonly PackedSceneSpawner<CardSceneRoot, CardSceneRoot.Input> _cardSpawner = new() {
        PackedScenePath = "res://Scenes/GameComponents/Card.tscn"
    };

    public CardSceneRoot SpawnCard(
        CardSceneRoot.Input input
    ) {
        Require.Argument(
            input.MyCard,
            _cardSpawner.Instances.SingleOrDefault(it => it.MyCard == input.MyCard) is null,
            $"Can't spawn a {nameof(CardSceneRoot)} for {input.MyCard} because one already exists!"
        );

        return _cardSpawner.Spawn(input);
    }
}