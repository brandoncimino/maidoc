using maidoc.Core.NormalCreatures;

namespace maidoc.Core.Cards;

public interface ICardData {
    string  CanonicalName { get; }
    int     Cost          { get; }
    string? FlavorText    { get; }
}

public readonly record struct CreatureData : ICardData {
    public required string              CanonicalName { get; init; }
    public required int                 Cost          { get; init; }
    public required NormalCreatureStats PrintedStats  { get; init; }
    public          string?             FlavorText    { get; init; }

    public static readonly CreatureData Chump = new() {
        CanonicalName = "Chump",
        Cost          = 1,
        PrintedStats = new NormalCreatureStats {
            AttackPower  = 1,
            MaxHealth    = 1,
            MovesPerTurn = 1
        }
    };

    public static readonly CreatureData Bear = new() {
        CanonicalName = "Bear",
        Cost          = 2,
        PrintedStats = new NormalCreatureStats {
            AttackPower  = 2,
            MaxHealth    = 2,
            MovesPerTurn = 1
        }
    };

    public static readonly CreatureData JunkRhino = new() {
        CanonicalName = "Junk Rhino",
        Cost          = 4,
        PrintedStats = new NormalCreatureStats() {
            AttackPower  = 4,
            MaxHealth    = 5,
            MovesPerTurn = 1
        }
    };

    public static readonly CreatureData BlueEyesWhiteDragon = new() {
        CanonicalName = "Blue-Eyes White Dragon",
        Cost          = 8,
        PrintedStats = new NormalCreatureStats {
            AttackPower  = 3000,
            MaxHealth    = 3000,
            MovesPerTurn = 1,
        },
        FlavorText =
            "This legendary dragon is a powerful engine of destruction. Virtually invincible, very few have faced this awesome creature and lived to tell the tale."
    };
}

public readonly record struct SpellData;