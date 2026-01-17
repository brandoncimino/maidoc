using maidoc.Core.NormalCreatures;

namespace maidoc.Core;

public sealed class NormalMoveRequest : PlayerRequest {
    public required NormalCreature Creature    { get; init; }
    public required BoardCoord     From        { get; init; }
    public required BoardCoord     Destination { get; init; }

    public override StepResult CanExecute(Referee referee) {
        // if (Creature.RemainingMoves <= 0) {
        //     return new("No remaining movement.");
        // }
        //
        // if (Destination.IsOrthogonallyAdjacentTo(From) is false) {
        //     return new($"Destination {Destination} is not orthogonally adjacent to {From}.");
        // }
        //
        // var destinationCell = referee.Board[Destination];
        // if (destinationCell.OwnerId != RequestingPlayer) {
        //     return new($"Destination is owned by a different player: {destinationCell.OwnerId}");
        // }
        //
        // if (destinationCell.Occupant is not null) {
        //     return new($"Destination is already occupied by {destinationCell.Occupant}.");
        // }

        return default;
    }

    public override void Execute(Referee referee) {
        referee.NormalMove(this);
    }
}