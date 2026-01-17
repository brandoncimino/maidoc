using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Godot;

namespace maidoc.Core;

public sealed class BoardRows<T> : IEnumerable<T> {
    public readonly  PlayerId                                           PlayerId;
    private readonly ImmutableDictionary<BoardRowId, ImmutableArray<T>> _rows;

    public int LaneCount { get; }

    public T this[BoardCoord coord] => this[coord.Row, coord.Lane];
    public T this[BoardRowId row, Index lane] => _rows[row][lane];

    public Vector2I Dimensions => new(_rows.Count, LaneCount);

    public BoardRows(
        PlayerId             playerId,
        int                  laneCount,
        Func<CellAddress, T> cellFactory
    ) {
        Require.Argument(laneCount, laneCount > 0);

        PlayerId  = playerId;
        LaneCount = laneCount;

        _rows = Enum.GetValues<BoardRowId>()
            .ToImmutableDictionary(
                row => row,
                row => Enumerable.Range(0, laneCount)
                    .Select(lane =>
                        cellFactory(
                            new CellAddress() {
                                Lane     = lane,
                                PlayerId = playerId,
                                Row      = row
                            }
                        )
                    ).ToImmutableArray()
            );
    }

    public IEnumerator<T> GetEnumerator() {
        return _rows.Values
            .SelectMany(it => it)
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}