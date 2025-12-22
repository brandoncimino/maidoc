namespace maidoc.Core.NormalCreatures;

public readonly record struct NormalCreatureStats(
    int AttackPower,
    int MaxHealth,
    int MovesPerTurn
);

public sealed class NormalCreature(
    BoardCell myCell,
    NormalCreatureStats printedStats
) : ICellOccupant, ISelectable {
    public BoardCell MyCell { get; set; } = myCell;

    /// <summary>
    /// The <see cref="NormalCreatureStats"/> that I was born with.
    /// </summary>
    public NormalCreatureStats PrintedStats { get; }      = printedStats;

    /// <summary>
    /// The <see cref="NormalCreatureStats"/> that I currently have, which may have been modified from my <see cref="PrintedStats"/>.
    /// </summary>
    public NormalCreatureStats Stats        { get; set; } = printedStats;

    public int CurrentHealth  { get; set; } = printedStats.MaxHealth;
    public int RemainingMoves { get; set; } = printedStats.MovesPerTurn;

    public void OnTurnStart() {
        RemainingMoves = Stats.MovesPerTurn;
    }
}