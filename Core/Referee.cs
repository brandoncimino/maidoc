using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Core;

[SuppressMessage("ReSharper", "RedundantBoolCompare" /* Why can't I get this inspection to stop showing up?! */)]
public sealed class Referee {
    public readonly BoardGrid Board;
    private         Ruleset   Ruleset { get; }

    private PlayerId _activePlayer;

    public PlayerId ActivePlayer {
        get => _activePlayer;
        private set => _activePlayer = Require.Argument(value, _duelDisks.ContainsKey(value));
    }

    private readonly ImmutableDictionary<PlayerId, DuelDisk> _duelDisks;

    public GamePhase Phase { get; private set; } = GamePhase.BeforeGame;

    public enum GamePhase {
        BeforeGame,
        InGame,
        PostGame
    }

    private Referee(
        PlayerId                                activePlayer,
        ImmutableDictionary<PlayerId, DuelDisk> duelDisks,
        Ruleset ruleset
        ) {
        _duelDisks    = duelDisks;
        ActivePlayer  = activePlayer;
        Ruleset = ruleset.Validate();
        GD.Print($"Ruleset, hopefully validated, is: {ruleset}");
        Board        = new BoardGrid(ruleset.RowsPerPlayer, ruleset.Columns);

        Require.Argument(duelDisks, Enum.GetValues<PlayerId>().All(duelDisks.ContainsKey));
    }

    public void StartGame() {
        Require.State(Phase == GamePhase.BeforeGame);
        Phase = GamePhase.InGame;

        _duelDisks.ForEach(kvp => kvp.Value.DrawRange(..Ruleset.StartingHandSize));
    }

    public static Referee PrepareFreshGame(
        IDictionary<PlayerId, Decklist> decklists,
        PlayerId                        startingPlayer,
        Ruleset ruleset
    ) {
        return new Referee(
            activePlayer: startingPlayer,
            duelDisks: decklists.ToImmutableDictionary(
                it => it.Key,
                it => new DuelDisk(
                    it.Value.Cards.Select(cardData => PrintCard(it.Key, cardData))
                )
            ),
            ruleset
        );
    }

    private static PaperCard PrintCard<T>(PlayerId owner, T cardData) where T : ICardData {
        return cardData switch {
            CreatureData cd => new NormalCreatureCard() {
                OwnerId      = owner,
                CreatureData = cd
            },
            _ => throw new ArgumentOutOfRangeException(nameof(cardData), cardData, null)
        };
    }

    public void EndTurn() {
        Require.State(Phase == GamePhase.InGame);

        Board.Where(it => it.OwnerId == ActivePlayer)
            .ForEach(it => it.Occupant?.OnTurnEnd());

        StartTurn(ActivePlayer.Other());
    }

    public void StartTurn(PlayerId playerId) {
        Require.State(Phase == GamePhase.InGame);

        // 📎 the new player might be the same as the `ActivePlayer`, in the event that they're taking a bonus turn
        ActivePlayer = playerId;

        // untap/upkeep
        Board.Where(it => it.OwnerId == playerId)
            .ForEach(it => it.Occupant?.OnTurnStart());

        // draw
        _duelDisks[playerId].Draw();
    }

    public ValueTuple Summon(
        NormalSummonRequest request
    ) {
        var creature = new NormalCreature(
            Board[request.Destination],
            request.CreatureData.PrintedStats
        );

        Board[request.Destination].Occupant = creature;

        return default;
    }

    public void Move(NormalMoveRequest request) {
        Require.Argument(request.Creature, request.Creature.RemainingMoves > 0);
        request.Creature.RemainingMoves -= 1;

        var destinationCell = Board[request.Destination];
        request.Creature.MyCell  = destinationCell;
        destinationCell.Occupant = request.Creature;
    }
}