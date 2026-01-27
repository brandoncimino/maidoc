using maidoc.Core.Cards;

namespace maidoc.Core;

public interface IGameEvent;

public readonly record struct AdmonitionEvent : IGameEvent {
    public required string Message { get; init; }
}

public readonly record struct CardMovedEvent : IGameEvent {
    public required PaperCard   Card { get; init; }
    public required ZoneAddress From { get; init; }
    public required ZoneAddress To   { get; init; }
}

public readonly record struct DeckShuffledEvent : IGameEvent {
    public required PlayerId PlayerId { get; init; }
}