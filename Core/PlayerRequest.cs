using System;
using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Core;

public interface IPlayerRequest {
    PlayerId RequestingPlayer { get; }

    public StepResult CanExecute(Referee referee);

    public void Execute(Referee referee);
}

public abstract class PlayerRequest : IPlayerRequest {
    public required PlayerId RequestingPlayer { get; init; }
    public abstract StepResult CanExecute(Referee referee);
    public abstract void Execute(Referee referee);
}

public sealed class NormalSummonRequest : PlayerRequest {
    public required CreatureData CreatureData { get; init; }
    public required BoardCoord   Destination  { get; init; }

    public override StepResult CanExecute(Referee referee) {
        var destinationCell = referee.Board[Destination];
        if (destinationCell.Occupant != null) {
            return new($"Cell already occupied by: {destinationCell.Occupant}");
        }

        if (destinationCell.OwnerId != RequestingPlayer) {
            return new($"Cell is owned by player {destinationCell.OwnerId}, not {RequestingPlayer}");
        }

        return default;
    }

    public override void Execute(Referee    referee) {
        referee.Summon(this);
    }
}

public sealed class NormalMoveRequest : PlayerRequest {
    public required NormalCreature Creature    { get; init; }
    public required BoardCoord                    Destination { get; init; }

    public override StepResult CanExecute(Referee referee) {
        if (Creature.RemainingMoves <= 0) {
            return new("No remaining movement.");
        }

        if (Destination.IsOrthogonallyAdjacentTo(Creature.MyCell.Coord) is false) {
            return new($"Destination {Destination} is not orthogonally adjacent to {Creature.MyCell.Coord}.");
        }

        var destinationCell = referee.Board[Destination];
        if (destinationCell.OwnerId != RequestingPlayer) {
            return new($"Destination is owned by a different player: {destinationCell.OwnerId}");
        }

        if (destinationCell.Occupant is not null) {
            return new($"Destination is already occupied by {destinationCell.Occupant}.");
        }

        return default;
    }

    public override void       Execute(Referee    referee) {
        referee.Move(this);
    }
}

public sealed record EndTurnRequest(
    PlayerId RequestingPlayer
    ) : IPlayerRequest {

    public StepResult CanExecute(Referee referee) {
        return referee.ActivePlayer == RequestingPlayer
            ? default
            : new($"{RequestingPlayer} is not the {nameof(referee.ActivePlayer)} ({referee.ActivePlayer}).");
    }

    public void Execute(Referee referee) {
        referee.EndTurn();
    }
}