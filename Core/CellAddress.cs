namespace maidoc.Core;

public readonly record struct CellAddress {
    public required PlayerId   PlayerId { get; init; }
    public required BoardRowId Row      { get; init; }
    public required int        Lane     { get; init; }

    public static implicit operator BoardCoord(CellAddress cellAddress) =>
        new BoardCoord() {
            Row  = cellAddress.Row,
            Lane = cellAddress.Lane,
        };
}