using System.Collections.Immutable;

namespace maidoc.Core.Cards;

/// <summary>
///
/// </summary>
public readonly record struct Decklist(ImmutableArray<ICardData> Cards) {
    public static readonly Decklist DevPlaceholder = new(
        [
            CreatureData.Chump,
            CreatureData.Chump,
            CreatureData.Chump,
            CreatureData.Chump,
            CreatureData.Bear,
            CreatureData.Bear,
            CreatureData.Bear,
            CreatureData.Bear,
            CreatureData.JunkRhino,
            CreatureData.JunkRhino,
            CreatureData.JunkRhino,
            CreatureData.JunkRhino,
            CreatureData.BlueEyesWhiteDragon
        ]
    );
}