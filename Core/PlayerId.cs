using System;

namespace maidoc.Core;

public enum PlayerId
{
    Red,
    Blue
}

public static class PlayerIdExtensions {
    public static PlayerId Other(this PlayerId playerId) => playerId switch {
        PlayerId.Red  => PlayerId.Blue,
        PlayerId.Blue => PlayerId.Red,
        _             => throw new ArgumentOutOfRangeException(nameof(playerId), playerId, null)
    };
}