using System;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class SceneFactory : Node2D {
    // TODO: can this be `private`? It should only be accessible from the editor.
    [Export]
    public required PackedScene CardScene { get; set; }

    private readonly PackedSceneSpawner<CardSceneRoot, CardSceneRoot.Input> _cardSpawner = new();

    public CardSceneRoot SpawnCard(
        CardSceneRoot.Input input
    ) {
        Require.Argument(
            input.MyCard,
            _cardSpawner.Instances.SingleOrDefault(it => it.MyCard == input.MyCard) is null,
            $"Can't spawn a {nameof(CardSceneRoot)} for {input.MyCard} because one already exists!"
        );

        return _cardSpawner.Spawn(CardScene, input);
    }
}