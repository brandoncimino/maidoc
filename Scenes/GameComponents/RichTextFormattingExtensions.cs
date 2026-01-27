using System;
using Godot;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

public static class RichTextFormattingExtensions {
    public static RichTextLabel AppendStatValue<T>(this RichTextLabel label, T value) {
        return label.Append(value + "", TextHelpers.TextDecoration.Bold);
    }

    public static RichTextLabel AppendStatValue<T>(
        this RichTextLabel label,
        T                  printedValue,
        T                  currentValue
    ) where T : IEquatable<T> {
        if (currentValue.Equals(printedValue) == false) {
            label.Append(
                printedValue + "",
                TextHelpers.TextDecoration.Strikethrough | TextHelpers.TextDecoration.Italics
            );
            label.Append(" ");
        }

        return label.AppendStatValue(currentValue);
    }

    public enum StatIcon {
        Attack,
        Health,
        Movement
    }

    public static RichTextLabel AppendStatIcon(
        this RichTextLabel label,
        StatIcon           icon
    ) {
        return label.Append(
            icon switch {
                StatIcon.Attack   => "⚔️",
                StatIcon.Health   => "❤️",
                StatIcon.Movement => "👟",
                _                 => throw new ArgumentOutOfRangeException(nameof(icon), icon, null)
            }
        );
    }

    public static RichTextLabel AppendCardFaceText(
        this RichTextLabel face,
        ICardData          cardData,
        int?               nameFontSize = null
    ) {
        face.Text = "";
        face.PushFontSize(10);
        // face.PushFontSize(nameFontSize ?? face.GetThemeFontSize("CardName"));
        face.AddText(cardData.CanonicalName);
        face.Pop();
        face.AddText($" 🔵 {cardData.Cost}");
        face.AddText("\n");

        face.AddHr(width: 20, widthInPercent: true);
        face.AddText("\n");

        if (cardData is CreatureData creatureData) {
            face.AppendLine($"⚔️ {creatureData.PrintedStats.AttackPower}");
            face.AppendLine($"❤️ {creatureData.PrintedStats.MaxHealth}");
            face.AppendText($"👟 {creatureData.PrintedStats.MovesPerTurn}");
        }

        if (cardData.FlavorText is not null) {
            face.AddHr(width: 20, widthInPercent: true);
            face.AddText("\n");
            face.PushItalics();
            face.AddText(cardData.FlavorText);
            face.Pop();
        }

        return face;
    }
}