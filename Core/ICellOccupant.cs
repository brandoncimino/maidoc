namespace maidoc.Core;

/// <summary>
/// Something that can occupy a <see cref="BoardCell"/>.
/// </summary>
public interface ICellOccupant : ITurnLifecycle {
    /// <summary>
    /// TODO: <see cref="BoardCell.Occupant"/> and <see cref="ICellOccupant.MyCell"/> have a circular relationship. Is that OK? If not, how should it be avoided?
    /// </summary>
    public BoardCell MyCell { get; }
}