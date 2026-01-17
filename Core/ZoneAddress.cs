namespace maidoc.Core;

public readonly record struct ZoneAddress {
    public PlayerId       PlayerId { get; }
    public DuelDiskZoneId ZoneId   { get; }
}