using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Core;

[SuppressMessage("ReSharper", "RedundantBoolCompare" /* Why can't I get this inspection to stop showing up?! */)]
public sealed class Referee {
    private          Ruleset     Ruleset { get; }
    private readonly PaperPusher _paperPusher;

    public PlayerId ActivePlayer { get; private set; }

    public PlayerId InactivePlayer => ActivePlayer switch {
        PlayerId.Red  => PlayerId.Blue,
        PlayerId.Blue => PlayerId.Red,
        _             => throw new ArgumentOutOfRangeException()
    };

    public GamePhase Phase { get; private set; } = GamePhase.BeforeGame;

    public enum GamePhase {
        BeforeGame,
        InGame,
        PostGame
    }

    private Referee(
        PlayerId    activePlayer,
        Ruleset     ruleset,
        PaperPusher paperPusher
    ) {
        ActivePlayer = activePlayer;
        Ruleset      = ruleset.Validate();

        _paperPusher = paperPusher;
    }

    public void StartGame() {
        Require.State(Phase == GamePhase.BeforeGame);
        Phase = GamePhase.InGame;

        foreach (var player in Enum.GetValues<PlayerId>()) {
            _paperPusher.DrawFromDeck(player, 0);
        }
    }

    public static Referee PrepareFreshGame(
        IDictionary<PlayerId, Decklist> decklists,
        PlayerId                        startingPlayer,
        Ruleset                         ruleset,
        PaperPusher                     paperPusher
    ) {
        return new Referee(
            activePlayer: startingPlayer,
            ruleset,
            paperPusher
        );
    }

    public void EndTurn() {
        Require.State(Phase == GamePhase.InGame);

        // _paperPusher.EnumerateBoardCells(ActivePlayer)
        //     .ForEach(it => it);

        StartTurn(ActivePlayer.Other());
    }

    public void StartTurn(PlayerId playerId) {
        Require.State(Phase == GamePhase.InGame);

        // 📎 the new player might be the same as the `ActivePlayer`, in the event that they're taking a bonus turn
        ActivePlayer = playerId;
    }

    public ValueTuple NormalSummon(
        NormalSummonRequest request
    ) {
        var normalCreatureCard = _paperPusher.GetCard<NormalCreatureCard>(request.PaperCardSerialNumber);

        var creature = new NormalCreature(
            request.Destination,
            normalCreatureCard
        );

        _paperPusher.GetCell(request.Destination).Occupant = creature;

        return default;
    }

    public void NormalMove(NormalMoveRequest request) {
        Require.Argument(request.Creature, request.Creature.RemainingMoves > 0);
        request.Creature.RemainingMoves -= 1;

        // var destinationCell = Board[request.Destination];
        // request.Creature.MyCell  = destinationCell;
        // destinationCell.Occupant = request.Creature;
    }

    public IEnumerable<IGameEvent> DrawFromDeck(PlayerId playerId) {
        yield return _paperPusher.DrawFromDeck(playerId, 0);
    }
}