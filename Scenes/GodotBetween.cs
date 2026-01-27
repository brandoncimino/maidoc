using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using maidoc.Core;
using maidoc.Scenes.UI;

namespace maidoc.Scenes;

public sealed class GodotBetween {
    /// <summary>
    /// All <see cref="IGameEvent"/>s that I have recorded.
    /// </summary>
    private readonly List<IGameEvent> _eventLog = [];

    /// <summary>
    /// The <see cref="IGameEvent"/>s I have recorded that have not yet been <see cref="Consume"/>d.
    /// </summary>
    private readonly Queue<IGameEvent> _eventQueue = [];

    public ImmutableArray<IGameEvent> CurrentEvents => [.._eventQueue];

    public required Referee              Referee              { get; init; }
    public required GodotPlayerInterface GodotPlayerInterface { get; init; }
    public required SceneFactory         SceneFactory         { get; init; }

    public void DrawFromDeck(PlayerId playerId) {
        _eventQueue.AddRange(Referee.DrawFromDeck(playerId));
    }

    public void ShuffleDeck(PlayerId playerId, Random? random = null) {
        _eventQueue.AddRange(Referee.ShuffleDeck(playerId, random ?? Random.Shared));
    }

    public bool TryConsumeNextEvent(
        Action<IGameEvent> consumer
    ) {
        if (_eventQueue.TryDequeue(out var gameEvent)) {
            consumer(gameEvent);
            return true;
        }

        return false;
    }

    public void ConsumeAllEvents(
        Action<IGameEvent> consumer
    ) {
        while (_eventQueue.Count > 0) {
            consumer(_eventQueue.Dequeue());
        }
    }
}