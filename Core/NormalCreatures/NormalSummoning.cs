using System;

namespace maidoc.Core.NormalCreatures;

public sealed class NormalSummoning : IPlayerAction {
    public required PlayerId           ActingPlayer { get; init; }
    public required NormalCreatureCard ToSummon     { get; init; }
    public          BoardCell?         Destination  { get; private set; }


    public          StepResult<ValueTuple> CanSelect(Referee referee, ISelectable selectable) {
        if (selectable is not BoardCell boardCell) {
            return new StepResult<ValueTuple>($"{selectable} is not a {typeof(BoardCell)}.");
        }

        if (boardCell.OwnerId != ActingPlayer) {
            return new($"{boardCell} is not owned by {ActingPlayer}.");
        }

        if (boardCell.Occupant is not null) {
            return new($"{boardCell} is already occupied.");
        }

        return default;
    }

    public          StepResult<ValueTuple> TrySelect(Referee referee, ISelectable selectable) {
        var canSelect = CanSelect(referee, selectable);

        if (canSelect.IsSuccess) {
            Destination = selectable as BoardCell;
            return default;
        }

        return canSelect;
    }

    public StepResult<ValueTuple> CanConfirm(Referee referee) {
        return CanConfirmInternal(referee).Then<ValueTuple>(_ => default);
    }

    private StepResult<NormalSummonRequest> CanConfirmInternal(
        Referee referee
    ) {
        if (Destination is null) {
            return new("You must choose a destination.");
        }

        return new NormalSummonRequest {
            CreatureData     = ToSummon.CreatureData,
            Destination      = Destination.Coord,
            RequestingPlayer = ActingPlayer
        };
    }

    public StepResult<ValueTuple> TryConfirm(Referee referee) {
        return CanConfirmInternal(referee)
            .Then<ValueTuple>(it => referee.Summon(it));
    }
}