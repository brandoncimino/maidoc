using System;
using System.Linq;
using Godot;

namespace maidoc.Scenes.GameComponents;

public partial class HandView : Node2D, ISceneRoot<HandView, ValueTuple> {
    public CardSceneRoot? FocusedCard { get; private set; }

    public float WidthInMeters { get; set; }

    public HandView InitializeSelf(ValueTuple input) {
        return this;
    }

    private void OrganizeCards() {
        var handCards = this.EnumerateChildren(1)
            .OfType<CardSceneRoot>()
            .ToList();

        if (handCards.Count == 0) {
            return;
        }

        var cardSizeInMeters = handCards.First().CardSizeInMeters;

        var (start, interval) = CalculateLayoutFromCenter(WidthInMeters, cardSizeInMeters.X, handCards.Count);

        for (var i = 0; i < handCards.Count; i++) {
            var card = handCards[i];
            card.Position = new Vector2(
                Position.X + start + interval * i,
                card == FocusedCard ? Position.Y + cardSizeInMeters.Y / 2 : Position.Y
            );
        }
    }

    private static (float start, float interval) CalculateLayoutFromCenter(
        float availableSpace,
        float sizePerItem,
        int   itemCount
    ) {
        var totalItemSize = sizePerItem * itemCount;
        if (totalItemSize <= availableSpace) {
            return (-totalItemSize / 2, sizePerItem);
        }

        return (-availableSpace / 2, availableSpace / sizePerItem);
    }
}