using System;
using maidoc.Core.Cards;

namespace maidoc.Core.NormalCreatures;

public sealed class NormalCreatureCard : PaperCard, IActionStarter {
    public CreatureData CreatureData { get => (CreatureData)Data; init => Data = value; }

    public StepResult<ValueTuple> CanStart(Referee referee) {
        if (referee.ActivePlayer != OwnerId) {
            return new StepResult<ValueTuple>(
                $"The card {this} is owned by {OwnerId}, not the active player, {referee.ActivePlayer}."
            );
        }

        return default;
    }

    public StepResult<IPlayerAction> TryStart(Referee referee) {
        return CanStart(referee)
            .Then(_ => new NormalSummoning() {
                    ActingPlayer = referee.ActivePlayer,
                    ToSummon     = this
                }
            );
    }
}