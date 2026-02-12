using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public sealed class CardZoneBehavior : ICardZoneNode {
    public required ZoneAddress ZoneAddress  { get; init; }
    public required Distance2D  UnscaledSize { get; init; }
    public required Node2D      AsNode2D     { get; init; }

    private ImmutableArray<ICardSceneRoot> _myCards = [];

    public void AddCard(ICardSceneRoot card) {
        if (_myCards.Contains(card)) {
            throw new ArgumentException($"Can't add {card} because it's already here!");
        }

        _myCards += card;
        card.AsNode2D.AsChildOf(AsNode2D);
    }

    public bool TryGetCard(SerialNumber serialNumber, [NotNullWhen(true)] out ICardSceneRoot? card) {
        card = _myCards.SingleOrDefault(it => it.SerialNumber == serialNumber);
        return card != null;
    }

    public void SendCard(ICardSceneRoot card, ICardZoneNode destination) {
        if (_myCards.Contains(card) == false) {
            throw new ArgumentException($"Can't send {card} because it isn't here!");
        }

        destination.AddCard(card);
    }
}