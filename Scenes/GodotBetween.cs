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

    public void DrawFromDeck(PlayerId playerId) {
        _eventQueue.AddRange(Referee.DrawFromDeck(playerId));
    }

    public void ShuffleDeck(PlayerId playerId, Random? random = null) {
        _eventQueue.AddRange(Referee.ShuffleDeck(playerId, random ?? Random.Shared));
    }

    public void ConsumeAllEvents() {
        while (_eventQueue.Count > 0) {
            var next = _eventQueue.Dequeue();
            Consume(next);
        }
    }

    private bool Consume(IGameEvent gameEvent) {
        return gameEvent switch {
            AdmonitionEvent admonitionEvent => ConsumeAdmonitionEvent(admonitionEvent),
            DeckShuffledEvent deckShuffledEvent => ConsumeDeckShuffledEvent(deckShuffledEvent),
            CardAddedToHand cardAddedToHand => ConsumeCardAddedToHandEvent(),
            CardMovedEvent cardMovedEvent => ConsumeCardMovedEvent(),
            _ => throw new NotImplementedException($"I don't know how to handle: {gameEvent}")
        };
    }

    private static bool ConsumeCardMovedEvent() {
        throw new NotImplementedException();
    }

    private static bool ConsumeCardAddedToHandEvent() {
        throw new NotImplementedException();
    }

    private bool ConsumeDeckShuffledEvent(DeckShuffledEvent deckShuffledEvent) {
        return NotifyPlayer(
            new() {
                Message = $"{deckShuffledEvent.PlayerId} player deck shuffled."
            }
        );
    }

    private bool ConsumeAdmonitionEvent(AdmonitionEvent admonitionEvent) {
        return NotifyPlayer(
            new Notification() {
                Message = admonitionEvent.Message,
                Tone    = Notification.NotificationTone.Negative
            }
        );
    }

    private bool NotifyPlayer(Notification notification) {
        GodotPlayerInterface.NotifyPlayer(notification);
        return true;
    }
}