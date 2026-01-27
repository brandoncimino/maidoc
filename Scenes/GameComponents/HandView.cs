using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

namespace maidoc.Scenes.GameComponents;

[Tool]
public partial class HandView : Node2D, ISceneRoot<HandView, HandView.SpawnInput>, IHandSceneRoot {
    [Export]
    private GodotHelpers.BoundaryNavigation _boundaryNavigation;

    private readonly LazyChild<ReferenceRect> _referenceRect = new();

    /// <summary>
    /// The node that my <see cref="ICardSceneRoot"/>s should be immediate children of.
    /// </summary>
    private Node CardParent => this;

    public Vector2 AvailableSpaceInMeters => _referenceRect.Get(this).Size / GodotHelpers.GodotUnitsPerMeter;

    public bool FaceDown { get; set; } = true;

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/Hand.tscn");

    public static HandView InstantiateRawScene() {
        return PackedScene.Instantiate<HandView>();
    }

    private string? _needsReorganizing;

    public void RequestReorganizing([CallerMemberName] string reason = "") {
        if (_needsReorganizing is null) {
            _needsReorganizing = reason;
        }
        else {
            _needsReorganizing = $"{_needsReorganizing}, {reason}";
        }
    }

    public override void _Ready() {
        RequestReorganizing();
    }

    public override void _Process(double delta) {
        if (_needsReorganizing is not null) {
            OrganizeCards();
        }
    }

    public HandView InitializeSelf(SpawnInput input) {
        return this;
    }

    public readonly record struct SpawnInput;

    /// <summary>
    /// TODO: Using Godot's scene structure to track the cards has tradeoffs:
    ///       + It ensures that the card only has 1 parent (because Godot enforces this), without those parents ever having to talk to a central authority or each other
    ///       - It is theoretically slower than tracking them ourselves
    /// </summary>
    /// <returns></returns>
    private IEnumerable<ICardSceneRoot> GetHandCards() => CardParent.EnumerateChildren(1)
                                                                    .OfType<ICardSceneRoot>();

    [ExportToolButton(nameof(OrganizeCards))]
    private Callable OrganizeCardsToolButton => Callable.From(OrganizeCards);

    private void OrganizeCards() {
        GD.Print($"Organizing cards in {Name} (reason: {_needsReorganizing})");
        _needsReorganizing = null;

        var handCards = GetHandCards().ToImmutableArray();

        if (handCards.Length == 0) {
            return;
        }


        var cardSizeInMeters = handCards.First().SizeInMeters;
        GD.Print(
            $"Organizing {handCards.Length} cards of size {cardSizeInMeters} into {AvailableSpaceInMeters} meters"
        );

        var (start, interval) = CalculateLayoutFromCenter(
            AvailableSpaceInMeters.X,
            cardSizeInMeters.X,
            handCards.Length
        );

        var excessCardHeight = cardSizeInMeters.Y / 2 - AvailableSpaceInMeters.Y;
        var yTucked          = excessCardHeight;
        var yFocused         = -cardSizeInMeters.Y / 2;

        this.blog(
            cardSizeInMeters.Y,
            cardSizeInMeters.Y / 2,
            AvailableSpaceInMeters.Y,
            excessCardHeight,
            yTucked,
            yFocused
        );

        for (var i = 0; i < handCards.Length; i++) {
            var card = handCards[i];

            card.AnimatePosition(
                new Vector2(
                    start + (interval * i),
                    card.IsFocused ? yFocused : yTucked
                )
            );

            var neighbors = i.GetNeighbors(handCards.Length, _boundaryNavigation);

            GodotHelpers.ConfigureLinearNavigation(
                card.FocusWrapper,
                Vector2.Axis.X,
                handCards[neighbors.previous].FocusWrapper,
                handCards[neighbors.next].FocusWrapper
            );
        }
    }

    private void FlipCards(bool faceDown) {
        foreach (var card in GetHandCards()) {
            card.FaceDown = faceDown;
        }
    }

    [Pure]
    public static (float start, float interval) CalculateLayoutFromCenter(
        float availableSpace,
        float sizePerItem,
        int   itemCount,
        float itemCenter01 = .5f
    ) {
        var totalItemSize = sizePerItem * itemCount;
        if (totalItemSize <= availableSpace) {
            var start = -(totalItemSize / 2) + itemCenter01 * sizePerItem;
            return (start, sizePerItem);
        }

        var interval = availableSpace / sizePerItem;
        return (-availableSpace       / 2, availableSpace / sizePerItem);
    }

    public void FocusOnFirstCard() {
        CardParent.EnumerateChildren()
                  .OfType<CardScene3>()
                  .FirstOrDefault()?
                  .GrabFocus();
    }

    public override void _Notification(int what) {
        if (what == NotificationChildOrderChanged) {
            RequestReorganizing();
        }
    }

    public void AddCard(ICardSceneRoot card) {
        card.AsNode2D.AsChildOf(CardParent);
        card.FocusWrapper.GrabFocus();
    }
}