using System;

namespace maidoc.Core;

public sealed class BoardCell : ISelectable {
    public required BoardCoord        Coord         { get; init; }
    public required PlayerId          OwnerId       { get; init; }

    private ICellOccupant? _occupant;

    /// <summary>
    /// TODO: <see cref="BoardCell.Occupant"/> and <see cref="ICellOccupant.MyCell"/> have a circular relationship. Is that OK? If not, how should it be avoided?
    /// </summary>
    public ICellOccupant? Occupant {
        get => _occupant;
        set => _occupant = _occupant is null
            ? value
            : throw new InvalidOperationException($"I am already occupied by {_occupant}!");
    }

    public override string ToString() {
        return $"{nameof(BoardCell)} {Coord} (Occupant: {Occupant})";
    }
}