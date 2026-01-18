using System;

namespace maidoc.Core.Cards;

/// <summary>
/// A physical card that can be shuffled, drawn, etc.
/// </summary>
/// <remarks>
/// <see cref="PaperCard"/>s should generally be compared via their <see cref="SerialNumber"/>s.
/// </remarks>
public abstract class PaperCard : ISelectable {
    public required SerialNumber SerialNumber { get; init; }

    public required PlayerId OwnerId { get; init; }

    public required PaperPusher Pusher { get; init; }

    public IPaperZone Zone => Pusher.GetZoneOfCard(SerialNumber);

    public required ICardData Data { get; init; }
}

/// <summary>
/// Uniquely identifies a <see cref="PaperCard"/>.
/// </summary>
public readonly record struct SerialNumber(Guid Value) {
    public static SerialNumber Random() => new(Guid.NewGuid());
}

public static class SerialNumberExtensions {
    public static SerialNumber OrRandom(this SerialNumber? serialNumber) {
        if (serialNumber is { } present) {
            return present;
        }

        return SerialNumber.Random();
    }
}