using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace maidoc.Core;

public sealed class BoardGrid : IEnumerable<BoardCell> {
    public readonly int Width;
    public readonly int Height;

    private readonly BoardCell[,] _cells;

    public BoardGrid(
        int rowsPerPlayer,
        int columns
    ) {
        Require.Argument(rowsPerPlayer, rowsPerPlayer > 0);
        Require.Argument(columns,       columns       > 0);

        Width  = columns;
        Height = rowsPerPlayer * 2;
        _cells = new BoardCell[Width, Height];

        Helpers.ForEachCellIndex(
            Width,
            Height,
            (x, y) => {
                _cells[x, y] = new BoardCell() {
                    Coord = new BoardCoord {
                        X = x,
                        Y = y,
                    },
                    OwnerId = (y < rowsPerPlayer) switch {
                        true  => PlayerId.Red,
                        false => PlayerId.Blue
                    }
                };
            }
        );
    }

    public BoardCell this[int        x, int y] => _cells[x, y];
    public BoardCell this[BoardCoord coord] => _cells[coord.X, coord.Y];

    public IEnumerator<BoardCell> GetEnumerator() {
        foreach (var cell in _cells) {
            yield return cell;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}