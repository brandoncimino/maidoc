namespace maidoc.Core;

/// <summary>
/// Locates a <see cref="BoardCell"/> within <see cref="BoardRows{T}"/>, which correspond to a <b>specific player</b>.
/// <br/>
/// Compare this with <see cref="CellAddress"/>, which can locate <b><i>ANY</i></b> cell, by including the <see cref="CellAddress.PlayerId"/>.
/// </summary>
public readonly record struct BoardCoord {
    public required BoardRowId Row  { get; init; }
    public required int        Lane { get; init; }

    public bool IsOrthogonallyAdjacentTo(BoardCoord other) {
        return (other.Row - Row).Abs() == 1 ^ (other.Lane - Lane).Abs() == 1;
    }

    public override string ToString() {
        return $"({Row}, {Lane})";
    }
}