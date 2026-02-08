using System.Diagnostics.CodeAnalysis;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public interface ICardZoneNode : IGameNode2D {
    public ZoneAddress ZoneAddress { get; }

    public void AddCard(ICardSceneRoot card);
    
    public bool TryGetCard(SerialNumber serialNumber, [NotNullWhen(true)] out ICardSceneRoot? card);
}