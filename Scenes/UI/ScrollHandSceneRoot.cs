using System.Collections.Generic;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Scenes.GameComponents;

namespace maidoc.Scenes.UI;

/// <summary>
/// The simplest - and ugliest - way to handle an arbitrary number of cards being placed into the hand.
/// TODO: Replace this with a more sophisticated "fanning" system, e.g. a working <see cref="HandBoxContainer"/>.
/// </summary>
public partial class ScrollHandSceneRoot : ScrollContainer {
    private readonly Disenfranchised<Container> _cardContainer = new();

    public override void _Ready() {
        _cardContainer.Enfranchise(() => this.EnumerateChildren(1)
                                             .OfType<Container>()
                                             .Single()
        );
    }

    public void AddCard(ICardSceneRoot card) {
        card.AsNode2D.AsChildOf(_cardContainer.Value);
    }

    private IEnumerable<Control> CardsInHand => _cardContainer.Value.EnumerateChildren(1)
                                                              .OfType<Control>()
                                                              .Where(it => it.Visible);

    public override void _Notification(int what) {
        if (what == NotificationSortChildren) {
            CardsInHand
                .Peek(it => it.FocusMode = FocusModeEnum.All)
                .NeighborPairs()
                .ForEach(neighbors => {
                        var (previous, next)        = neighbors;
                        previous.FocusNeighborRight = previous.FocusNext           = next.GetPath();
                        previous.FocusNeighborTop   = previous.FocusNeighborBottom = previous.GetPath();
                        next.FocusNeighborLeft      = next.FocusPrevious           = previous.GetPath();
                        next.FocusNeighborTop       = next.FocusNeighborBottom     = next.GetPath();
                    }
                );
        }
    }
}