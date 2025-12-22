using Godot;
using maidoc.Core;

namespace maidoc.Scenes.GameComponents;

public static class DevText {
    public static void Append(
        this RichTextLabel label,
        CreatureData       creatureData
    ) {
        label.PushContext();

        label.Append(creatureData.CanonicalName, TextHelpers.TextDecoration.Bold);

        label.AddHr();

        label.PushList(0, RichTextLabel.ListType.Dots, false);

        label.AppendLine($"Attack: {creatureData.PrintedStats.AttackPower}");
        label.AppendLine($"Health: {creatureData.PrintedStats.MaxHealth}");
        label.AppendLine($"Moves per turn: {creatureData.PrintedStats.MovesPerTurn}");

        label.PopContext();
    }
}