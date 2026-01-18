using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace maidoc.Core.Cards;

/// <summary>
/// A place where <see cref="PaperCard"/>s can live.
/// </summary>
public interface IPaperZone : IEnumerable<SerialNumber> {
    public int         Count   { get; }
    public ZoneAddress Address { get; }

    public string? CanAdd(SerialNumber card) => null;
    public void    Add(SerialNumber    card);

    public string? CanRemove(SerialNumber card) => null;
    public void    Remove(SerialNumber    card);

    public string?      CanRemoveAt(Index index) => null;
    public SerialNumber RemoveAt(Index    index);

    public string? CanDrawAt(Index index) {
        var offset = index.GetOffset(Count);
        if (offset < 0 || offset >= Count) {
            return "Not enough cards.";
        }

        return null;
    }

    public ImmutableArray<SerialNumber> Snapshot();

    public void Shuffle(Random random);
}