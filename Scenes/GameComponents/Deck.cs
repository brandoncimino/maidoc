using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public partial class Deck : Node2D, ISceneRoot<Deck, Deck.SpawnInput>, ICardZoneNode {
    private readonly Disenfranchised<Control> _clickBox = new();

    private readonly Disenfranchised<CardZoneBehavior> _behavior = new();
    public           ZoneAddress                       ZoneAddress => _behavior.Value.ZoneAddress;

    public Node2D AsNode2D => this;

    public Distance2D UnscaledSize => _behavior.Value.UnscaledSize;

    public Deck InitializeSelf(SpawnInput input) {
        _behavior.Enfranchise(() =>
            new CardZoneBehavior() {
                AsNode2D     = this,
                UnscaledSize = input.UnscaledSizeInMeters.Meters,
                ZoneAddress = new ZoneAddress() {
                    ZoneId   = DuelDiskZoneId.Deck,
                    PlayerId = input.PlayerId
                }
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
        _behavior.Value.AddCard(card);

        card.AnimatePosition(
            this.Center,
            .2,
            tween => {
                GD.Print($"I ({card}) made it to the deck; disappearing.");
                tween.TweenCallback(Callable.From(() => card.AsNode2D.Visible = false));
            }
        );
    }

    public bool TryGetCard(SerialNumber serialNumber, [NotNullWhen(true)] out ICardSceneRoot? card) {
        return _behavior.Value.TryGetCard(serialNumber, out card);
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