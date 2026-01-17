using System.Collections.Generic;
using maidoc.Core;

namespace maidoc.Scenes;

public sealed class GodotBetween {
    private readonly List<IGameEvent> _eventQueue = [];

    private readonly Referee _referee;

    public void DrawFromDeck(PlayerId playerId) {
        _eventQueue.AddRange(_referee.DrawFromDeck(playerId));
    }
}