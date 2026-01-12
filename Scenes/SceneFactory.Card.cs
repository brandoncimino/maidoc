using System.Linq;
using maidoc.Core;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes;

public partial class SceneFactory {
    private readonly SceneSpawner<CardSceneRoot, CardSceneRoot.Input> _cardSpawner = new();

    public CardSceneRoot SpawnCard(
        CardSceneRoot.Input input
    ) {
        _cardSpawner.UseGroupNode(this);

        Require.Argument(
            input.MyCard,
            _cardSpawner.Instances.SingleOrDefault(it => it.MyCard == input.MyCard) is null,
            $"Can't spawn a {nameof(CardSceneRoot)} for {input.MyCard} because one already exists!"
        );

        return _cardSpawner.Spawn(input);
    }
}