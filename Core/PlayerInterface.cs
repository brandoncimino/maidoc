using System;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace maidoc.Core;

public sealed class PlayerInterface {
    public required Referee Referee { get; init; }

    public IPlayerAction? CurrentAction { get; private set; }

    public void Cancel() {
        CurrentAction?.OnCancel();
        CurrentAction = null;
    }

    public void ClickCell(CellAddress cellAddress) {
        GD.Print($"TODO: click {cellAddress}");
    }

    public StepResult<ValueTuple> TrySelect(ISelectable selection) {
        GD.Print($"Attempting to select: {selection}");

        if (CurrentAction is not null) {
            return CurrentAction.TrySelect(Referee, selection);
        }

        if (selection is IActionStarter actionStarter) {
            return actionStarter.TryStart(Referee)
                .Then(started => CurrentAction = started);
        }

        return new($"Nothing to be done with {selection}.");
    }

    public StepResult<ValueTuple> TryConfirm() {
        if (CurrentAction is null) {
            return new("There is no action currently in progress.");
        }

        return CurrentAction.TryConfirm(Referee);
    }
}

public interface ISelectable;

public interface IActionStarter : ISelectable {
    public StepResult<ValueTuple>    CanStart(Referee referee);
    public StepResult<IPlayerAction> TryStart(Referee referee);
}

public interface IPlayerAction {
    public PlayerId ActingPlayer { get; }

    public StepResult<ValueTuple> CanSelect(Referee  referee, ISelectable selectable);
    public StepResult<ValueTuple> TrySelect(Referee  referee, ISelectable selectable);
    public StepResult<ValueTuple> CanConfirm(Referee referee);
    public StepResult<ValueTuple> TryConfirm(Referee referee);

    public void OnCancel() { }
}

public readonly struct WhyNot {
    [MaybeNull]
    public string Reason { get; init; }

    public static implicit operator WhyNot(string reason) => new() { Reason = reason };
}

public static class PlayerInterfaceExtensions { }