using System;
using System.Collections.Generic;
using maidoc.Core;

namespace maidoc.Scenes;

public sealed class GodotBetween {
    /// <summary>
    /// All <see cref="IGameEvent"/>s that I have recorded.
    /// </summary>
    private readonly List<IGameEvent> _eventLog = [];

    /// <summary>
    /// The <see cref="IGameEvent"/>s I have recorded that have not yet been <see cref="Consume"/>d.
    /// </summary>
    internal readonly List<IGameEvent> _eventQueue = [];

    public required Referee Referee { get; init; }

    public void DrawFromDeck(PlayerId playerId) {
        _eventQueue.AddRange(Referee.DrawFromDeck(playerId));
    }

    public void ShuffleDeck(PlayerId playerId, Random? random = null) {
        _eventQueue.AddRange(Referee.ShuffleDeck(playerId, random ?? Random.Shared));
    }

    private void Consume(IGameEvent gameEvent) {
        // TODO
    }
}