using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using maidoc.Core.NormalCreatures;
using static maidoc.Core.DuelDiskZoneId;

namespace maidoc.Core.Cards;

public sealed class PaperPusher : IPaperView {
    private readonly Dictionary<SerialNumber, PaperCard> _allCards = new();

    public readonly ImmutableDictionary<PlayerId, DuelDisk> DuelDisks;

    // TODO: This is an exceptionally brute-force way to do things.
    public IEnumerable<IPaperZone> EnumerateZones() {
        foreach (var duelDisk in DuelDisks.Values) {
            foreach (var zone in duelDisk.EnumerateZones()) {
                yield return zone;
            }
        }
    }

    public PaperCard GetCard(SerialNumber serialNumber) {
        return _allCards[serialNumber];
    }

    public T GetCard<T>(SerialNumber serialNumber) where T : PaperCard {
        return (T)_allCards[serialNumber];
    }

    public IPaperZone GetZone(ZoneAddress zoneAddress) {
        return DuelDisks[zoneAddress.PlayerId].PaperZones[zoneAddress.ZoneId];
    }

    public BoardCell GetCell(CellAddress cellAddress) {
        return DuelDisks[cellAddress.PlayerId].Rows[cellAddress];
    }

    public IPaperZone GetZoneOfCard(SerialNumber serialNumber) {
        return EnumerateZones()
            .First(it => it.Contains(serialNumber));
    }

    public ICellOccupant? GetCellOccupant(CellAddress cellAddress) {
        return DuelDisks[cellAddress.PlayerId].Rows[cellAddress].Occupant;
    }

    public ImmutableArray<ICardData> GetZoneSnapshot(ZoneAddress zoneAddress) {
        return GetZone(zoneAddress)
            .Snapshot()
            .Select(it => GetCard(it).Data)
            .ToImmutableArray();
    }

    public PaperCard PrintCard<T>(
        PlayerId       owner,
        T              cardData,
        SerialNumber   serialNumber,
        DuelDiskZoneId zone
    ) where T : ICardData {
        if (_allCards.TryGetValue(serialNumber, out var existingCard)) {
            throw new InvalidOperationException($"A card with the {serialNumber} already exists: {existingCard}");
        }

        var paperCard = cardData switch {
            CreatureData cd => new NormalCreatureCard {
                OwnerId      = owner,
                CreatureData = cd,
                SerialNumber = serialNumber,
                Pusher       = this,
                Data         = cd
            },
            _ => throw new ArgumentOutOfRangeException(nameof(cardData), cardData, null)
        };

        _allCards[paperCard.SerialNumber] = paperCard;
        GetZone(
            new ZoneAddress() {
                PlayerId = owner,
                ZoneId   = zone
            }
        ).Add(serialNumber);
        return paperCard;
    }

    public PaperPusher(int laneCount) {
        DuelDisks = Enum.GetValues<PlayerId>()
            .ToImmutableDictionary(
                it => it,
                it => new DuelDisk(it, laneCount)
            );
    }

    public IGameEvent MoveCard(SerialNumber serialNumber, ZoneAddress from, ZoneAddress to) {
        var fromZone = GetZone(from);
        var toZone   = GetZone(to);

        if (fromZone.CanRemove(serialNumber) is { } whyNotRemove) {
            return new AdmonitionEvent() {
                Message = whyNotRemove
            };
        }

        if (toZone.CanAdd(serialNumber) is { } whyNotAdd) {
            return new AdmonitionEvent() {
                Message = whyNotAdd
            };
        }

        fromZone.Remove(serialNumber);
        toZone.Add(serialNumber);

        return new CardMovedEvent() {
            Card = GetCard(serialNumber),
            From = from,
            To   = to
        };
    }

    public IGameEvent DrawFromDeck(PlayerId playerId, Index index) {
        var deck = DuelDisks[playerId][Deck];
        if (deck.CanDrawAt(index) is { } whyNot) {
            return new AdmonitionEvent() {
                Message = whyNot
            };
        }

        var drawn = deck.RemoveAt(0);

        var hand = DuelDisks[playerId][Hand];
        hand.Add(drawn);

        return new CardMovedEvent() {
            Card = GetCard(drawn),
            From = deck.Address,
            To   = hand.Address
        };
    }

    public IGameEvent ShuffleDeck(PlayerId playerId, Random random) {
        var deck = DuelDisks[playerId][Deck];
        deck.Shuffle(random);
        return new DeckShuffledEvent() {
            PlayerId = playerId,
        };
    }

    /// <summary>
    /// <paramref name="activePlayer"/>'s <see cref="DuelDisk.EnumerateCells"/>, then the <see cref="Players.Other"/> player's.
    /// </summary>
    /// <param name="activePlayer"></param>
    /// <returns></returns>
    public IEnumerable<BoardCell> EnumerateBoardCells(
        PlayerId activePlayer
    ) {
        return DuelDisks[activePlayer]
            .EnumerateCells()
            .Concat(
                DuelDisks[activePlayer.Other()]
                    .EnumerateCells()
            );
    }
}