namespace maidoc.Core;

public readonly record struct ZoneAddress {
    public required PlayerId       PlayerId { get; init; }
    public required DuelDiskZoneId ZoneId   { get; init; }
}