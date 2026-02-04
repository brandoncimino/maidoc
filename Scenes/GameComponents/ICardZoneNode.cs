using maidoc.Core;

namespace maidoc.Scenes.GameComponents;

public interface ICardZoneNode : IGameNode2D {
    public ZoneAddress ZoneAddress { get; }

    public void AddCard(ICardSceneRoot card);
}