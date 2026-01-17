namespace maidoc.Core.NormalCreatures;

public readonly record struct NormalCreatureStats(
    int AttackPower,
    int MaxHealth,
    int MovesPerTurn
);

public sealed class NormalCreature(
    CellAddress        myCell,
    NormalCreatureCard myCard
) : ICellOccupant, ISelectable {
    public CellAddress MyCell { get; init; } = myCell;

    /// <summary>
    /// The <see cref="NormalCreatureStats"/> that I was born with.
    /// </summary>
    public NormalCreatureStats PrintedStats => MyCard.CreatureData.PrintedStats;

    public NormalCreatureCard MyCard { get; } = myCard;

    /// <summary>
    /// The <see cref="NormalCreatureStats"/> that I currently have, which may have been modified from my <see cref="PrintedStats"/>.
    /// </summary>
    public NormalCreatureStats CurrentStats { get; set; } = myCard.CreatureData.PrintedStats;

    public int CurrentHealth  { get; set; } = myCard.CreatureData.PrintedStats.MaxHealth;
    public int RemainingMoves { get; set; } = myCard.CreatureData.PrintedStats.MovesPerTurn;

    public void OnTurnStart() {
        RemainingMoves = CurrentStats.MovesPerTurn;
    }
}