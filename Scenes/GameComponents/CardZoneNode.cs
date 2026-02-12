using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public partial class CardZoneNode<TInput> : Node2D, ICardZoneNode, ISceneRoot<CardZoneNode<TInput>, TInput>
    where TInput : ZoneSpawnInput {
    public Node2D AsNode2D => this;

    private readonly Disenfranchised<Distance2D>          _unscaledSize = new();
    private readonly Disenfranchised<ZoneAddress>         _zoneAddress  = new();
    private readonly Disenfranchised<Action<ZoneAddress>> _onClick      = new();
    private readonly Disenfranchised<Control>             _clickBox     = new();
    public           Distance2D                           UnscaledSize => _unscaledSize;
    public           ZoneAddress                          ZoneAddress  => _zoneAddress;

    private ImmutableArray<ICardSceneRoot> _myCards = [];

    public static CardZoneNode<TInput> InstantiateRawScene() => new();

    public void AddCard(ICardSceneRoot card) {
        _myCards += card;
        card.AsNode2D.Reparent(this);
    }

    public bool TryGetCard(SerialNumber serialNumber, [NotNullWhen(true)] out ICardSceneRoot? card) {
        card = _myCards.SingleOrDefault(it => it.SerialNumber == serialNumber);
        return card != null;
    }

    public void RemoveCard(ICardSceneRoot card) {
        _myCards -= card;
    }

    public CardZoneNode InitializeSelf(ZoneSpawnInput input) {
        _zoneAddress.Enfranchise(input.ZoneAddress);
        _unscaledSize.Enfranchise(input.ZoneRect.Size);
        _onClick.Enfranchise(input.OnClick);
        this.Center = input.ZoneRect.Center;

        _clickBox.Enfranchise(() => {
                var button = new Button().AsChildOf(this);
                button.Position = default;
                button.Scale    = Vector2.One;
                button.AdjustSizeAndPosition(
                    GodotHelpers.Rect2ByCenter(Vector2.Zero, input.ZoneRect.Size.GodotPixels)
                );
                button.Pressed += ClickZone;
                return button;
            }
        );

        return this;
    }

    public void ClickZone() {
        _onClick.Value(ZoneAddress);
    }
}

public record ZoneSpawnInput {
    public required ZoneAddress         ZoneAddress { get; init; }
    public required RectDistance        ZoneRect    { get; init; }
    public          Action<ZoneAddress> OnClick     { get; init; } = address => GD.Print($"CLICKED zone: {address}");
}