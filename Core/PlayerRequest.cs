using maidoc.Core.Cards;

namespace maidoc.Core;

public interface IPlayerRequest {
    PlayerId RequestingPlayer { get; }

    public StepResult CanExecute(Referee referee);

    public void Execute(Referee referee);
}

public abstract class PlayerRequest : IPlayerRequest {
    public required PlayerId   RequestingPlayer { get; init; }
    public abstract StepResult CanExecute(Referee referee);
    public abstract void       Execute(Referee    referee);
}

public sealed class NormalSummonRequest : PlayerRequest {
    public required SerialNumber PaperCardSerialNumber { get; init; }
    public required CellAddress  Destination           { get; init; }

    public override StepResult CanExecute(Referee referee) {
        // var destinationCell = referee.Board[Destination];
        // if (destinationCell.Occupant != null) {
        //     return new($"Cell already occupied by: {destinationCell.Occupant}");
        // }
        //
        // if (destinationCell.OwnerId != RequestingPlayer) {
        //     return new($"Cell is owned by player {destinationCell.OwnerId}, not {RequestingPlayer}");
        // }

        return default;
    }

    public override void Execute(Referee referee) {
        referee.NormalSummon(this);
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