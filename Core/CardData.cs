using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Core;

public interface ICardData {
    string   CanonicalName { get; }
}

public readonly record struct CreatureData(
    string        CanonicalName,
    NormalCreatureStats PrintedStats
) : ICardData {
    public static readonly CreatureData Chump = new() {
        CanonicalName = "Chump",
        PrintedStats = new NormalCreatureStats {
            AttackPower  = 1,
            MaxHealth    = 1,
            MovesPerTurn = 1
        }
    };

    public static readonly CreatureData Bear = new() {
        CanonicalName = "Bear",
        PrintedStats = new NormalCreatureStats {
            AttackPower = 2,
            MaxHealth = 2,
            MovesPerTurn = 1
        }
    };

    public static readonly CreatureData JunkRhino = new() {
        CanonicalName = "Junk Rhino",
        PrintedStats  = new NormalCreatureStats() {
            AttackPower = 4,
            MaxHealth  = 5,
            MovesPerTurn = 1
        }
    };
}

public readonly record struct SpellData;