using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Core;

public interface ICardData {
    string  CanonicalName { get; }
    string? FlavorText    { get; }
}

public readonly record struct CreatureData(
    string        CanonicalName,
    NormalCreatureStats PrintedStats,
    string? FlavorText = null
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

    public static readonly CreatureData BlueEyesWhiteDragon = new() {
        CanonicalName = "Blue-Eyes White Dragon",
        PrintedStats = new NormalCreatureStats {
            AttackPower  = 3000,
            MaxHealth    = 3000,
            MovesPerTurn = 1,
        },
        FlavorText = "This legendary dragon is a powerful engine of destruction. Virtually invincible, very few have faced this awesome creature and lived to tell the tale."
    };
}

public readonly record struct SpellData;