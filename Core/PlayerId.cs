using System;
using System.Collections.Immutable;

namespace maidoc.Core;

public enum PlayerId {
    Red,
    Blue
}

public static class Players {
    public static readonly ImmutableArray<PlayerId> Ids = [..Enum.GetValues<PlayerId>()];

    public static PlayerId Other(this PlayerId playerId) => playerId switch {
        PlayerId.Red  => PlayerId.Blue,
        PlayerId.Blue => PlayerId.Red,
        _             => throw new ArgumentOutOfRangeException(nameof(playerId), playerId, null)
    };
}

public static class PlayerMap {
    public static PlayerMap<T> Create<T>(
        Func<PlayerId, T> valueFactory
    ) {
        return new PlayerMap<T> {
            Red  = valueFactory(PlayerId.Red),
            Blue = valueFactory(PlayerId.Blue)
        };
    }
}

public readonly record struct PlayerMap<T> {
    public required T Red  { get; init; }
    public required T Blue { get; init; }

    public T this[PlayerId playerId] => playerId switch {
        PlayerId.Red  => Red,
        PlayerId.Blue => Blue,
        _             => throw new ArgumentOutOfRangeException(nameof(playerId), playerId, null)
    };
}