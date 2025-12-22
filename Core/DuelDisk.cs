using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using maidoc.Core.Cards;

namespace maidoc.Core;

/// <summary>
///
/// </summary>
public sealed class DuelDisk {
    private readonly PaperCardGroup _deck;
    private readonly PaperCardGroup _hand;
    private readonly PaperCardGroup _graveyard;

    public DuelDisk(
        IEnumerable<PaperCard> startingDeck
    ) {
        _deck      = new PaperCardGroup(startingDeck);
        _hand      = new PaperCardGroup([]);
        _graveyard = new PaperCardGroup([]);
    }

    public PaperCard Draw() => DrawAt(Index.Start);

    public ReadOnlySpan<PaperCard> DrawRange(Range range) {
        var drawn = _deck.DrawRange(range);
        _hand.AddRange(drawn);
        return drawn;
    }

    public PaperCard DrawAt(Index index) {
        var drawn = _deck.DrawAt(index);
        _hand.Add(drawn);
        return drawn;
    }
}