namespace maidoc.Core;

public readonly record struct BoardCoord(int X, int Y) {
    public bool IsOrthogonallyAdjacentTo(BoardCoord other) {
        return (other.X - X).Abs() == 1 ^ (other.Y - Y).Abs() == 1;
    }

    public BoardCell In(BoardGrid board) => board[this];
}