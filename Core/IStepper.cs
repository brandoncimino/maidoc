using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using maidoc.Core.Cards;

namespace maidoc.Core;

public static class Step {
    public static StepResult<TOut> Success<TIn, TOut>(this IStepper<TIn, TOut> stepper, TOut output)
        where TOut : notnull => new(output);

    public static StepResult<TOut> Failure<TIn, TOut>(this IStepper<TIn, TOut> stepper, in WhyNot whyNot)
        where TOut : notnull => new(in whyNot);
}

public delegate StepResult<TOut> StepFunc<in TIn, TOut>(TIn input) where TOut : notnull;

public interface IStepper<TIn, TOut> where TIn : notnull where TOut : notnull {
    public abstract StepResult<TIn>  CanProceed(Referee referee, ISelectable selectable);
    public abstract StepResult<TOut> Proceed(Referee referee, TIn    input);
}

public class SelectionStep<TIn, TOut> where TOut : notnull where TIn : notnull {
    public required StepFunc<ISelectable, TIn> CanSelect { get; init; }
    public required StepFunc<TIn, TOut>        Select    { get; init; }
}

public readonly struct StepResult {
    public WhyNot? WhyNot { get; }

    public bool IsSuccess => WhyNot is null;

    public StepResult(WhyNot whyNot) {
        WhyNot = whyNot;
    }


    public static implicit operator StepResult(WhyNot whyNot) => new(whyNot);
}

public readonly struct StepResult<TOut> where TOut : notnull {
    public readonly TOut?  Result;
    public readonly WhyNot WhyNot;

    [MemberNotNullWhen(true, nameof(Result))]
    public bool IsSuccess => Result != null;

    public StepResult(in WhyNot whyNot) {
        WhyNot = whyNot;
    }

    public StepResult(TOut result) {
        Result = result;
    }

    public static implicit operator StepResult<TOut>(TOut      success) => new(success);
    public static implicit operator StepResult<TOut>(in WhyNot whyNot)  => new(in whyNot);

    public static implicit operator StepResult(in StepResult<TOut> resultWithOutput) =>
        resultWithOutput.IsSuccess ? default : resultWithOutput.WhyNot;

    public StepResult<T> Then<T>(StepFunc<TOut, T> nextStep) where T : notnull {
        return IsSuccess ? nextStep(Result) : WhyNot;
    }
}

public static class StepResultExtensions {
    public static StepResult<IPlayerAction> Then<T>(
        this StepResult<T> previousStep,
        StepFunc<T, IPlayerAction> actionStarter
    ) where T : notnull {
        return previousStep.IsSuccess ? actionStarter(previousStep.Result) : previousStep.WhyNot;
    }

    public static StepResult<ValueTuple> Then<T>(
        this StepResult<T> previousStep,
        Action<T>          nextStep
    ) where T : notnull {
        if (previousStep.IsSuccess) {
            nextStep(previousStep.Result);
            return default(ValueTuple);
        }
        else {
            return previousStep.WhyNot;
        }
    }
}

public class ChooseCard<TCard> : IStepper<TCard, TCard> where TCard : PaperCard {
    public StepResult<TCard> CanProceed(
        Referee     referee,
        ISelectable selectable
    ) {
        if (selectable is not TCard card) {
            return new($"{selectable} is not an instance of {typeof(TCard)}.");
        }

        if (card.OwnerId != referee.ActivePlayer) {
            return new($"{card}'s owner ({card.OwnerId}) is not the active player ({referee.ActivePlayer}).");
        }

        return card;
    }

    public StepResult<TCard> Proceed(Referee referee, TCard input) {
        return CanProceed(referee, input);
    }
}
