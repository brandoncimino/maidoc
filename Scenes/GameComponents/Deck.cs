using System;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes.GameComponents;

public partial class Deck : Node2D, ISceneRoot<Deck, Deck.SpawnInput>, ICardZoneNode {
    private readonly Disenfranchised<Node2D>  _cardHolder = new();
    private readonly Disenfranchised<Control> _clickBox   = new();

    private readonly Disenfranchised<ZoneAddress> _zoneAddress = new();
    public           ZoneAddress                  ZoneAddress => _zoneAddress.Value;

    public Node2D AsNode2D => this;

    private readonly Disenfranchised<Distance2D> _unscaledSize = new();
    public           Distance2D                  UnscaledSize => _unscaledSize.Value;

    public Deck InitializeSelf(SpawnInput input) {
        _zoneAddress.Enfranchise(
            new ZoneAddress() {
                PlayerId = input.PlayerId,
                ZoneId   = DuelDiskZoneId.Deck
            }
        );

        _unscaledSize.Enfranchise(input.UnscaledSizeInMeters.Meters());

        _cardHolder.Enfranchise(() => {
                var node = new Node2D() {
                    Name = "Cards"
                }.AsChildOf(this);

                node.Position = default;
                node.Scale    = Vector2.One;
                return node;
            }
        );

        _clickBox.Enfranchise(() => {
                var button = new Button();
                AddChild(button);
                button.Position = default;
                button.Scale    = Vector2.One;
                button.AdjustSizeAndPosition(
                    GodotHelpers.Rect2ByCenter(Vector2.Zero, input.UnscaledSizeInMeters)
                );
                button.Pressed += () => input.OnZoneClick(ZoneAddress);
                return button;
            }
        );

        return this;
    }

    public void AddCard(ICardSceneRoot card) {
        _cardHolder.Value.AddChild(card.AsNode2D);

        card.AnimatePosition(
            this.PositionInMeters(),
            .2,
            tween => {
                GD.Print($"I ({card}) made it to the deck; disappearing.");
                tween.TweenCallback(Callable.From(() => card.AsNode2D.Visible = false));
            }
        );
    }

    public static Deck InstantiateRawScene() {
        return new Deck();
    }

    public readonly record struct SpawnInput() {
        public required PlayerId            PlayerId             { get; init; }
        public          Vector2             UnscaledSizeInMeters { get; init; } = new Vector2(1.05f, 1.05f);
        public required Action<ZoneAddress> OnZoneClick          { get; init; }
    }
}