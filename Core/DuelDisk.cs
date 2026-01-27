using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using maidoc.Core.Cards;

namespace maidoc.Core;

[SuppressMessage("ReSharper", "UseCollectionExpression", Justification = "🤮")]
public sealed class DuelDisk {
    public PlayerId PlayerId  { get; }
    public int      LaneCount { get; }

    public ImmutableDictionary<DuelDiskZoneId, PaperCardGroup> PaperZones { get; }

    public IPaperZone this[DuelDiskZoneId id] => PaperZones[id];

    public BoardRows<BoardCell> Rows { get; }
    // public ImmutableDictionary<BoardRowId, ImmutableArray<BoardCell>> Rows { get; }

    public DuelDisk(
        PlayerId playerId,
        int      laneCount
    ) {
        LaneCount = laneCount;
        PlayerId  = playerId;

        PaperZones = Enum.GetValues<DuelDiskZoneId>()
                         .ToImmutableDictionary(
                             it => it,
                             it => new PaperCardGroup() {
                                 Address = new() {
                                     PlayerId = playerId,
                                     ZoneId   = it
                                 }
                             }
                         );

        Rows = new(
            playerId,
            laneCount,
            address => new BoardCell() {
                Address = address
            }
        );
    }

    public IEnumerable<IPaperZone> EnumerateZones() {
        return PaperZones.Values;
    }

    /// <summary>
    /// <b><i>From the perspective of this player</i></b>, left to right, <see cref="BoardRowId.Front"/> to <see cref="BoardRowId.Back"/>.
    /// </summary>
    public IEnumerable<BoardCell> EnumerateCells() {
        return Rows;
    }
}