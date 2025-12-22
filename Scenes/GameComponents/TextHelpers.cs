using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Scenes.GameComponents;

public static class TextHelpers {
    private static TextServer.Direction ToDirection(this Control.TextDirection textDirection) {
        return textDirection switch {
            Control.TextDirection.Inherited => TextServer.Direction.Inherited,
            Control.TextDirection.Auto => TextServer.Direction.Auto,
            Control.TextDirection.Ltr => TextServer.Direction.Ltr,
            Control.TextDirection.Rtl => TextServer.Direction.Rtl,
            _ => throw new ArgumentOutOfRangeException(nameof(textDirection), textDirection, null)
        };
    }

    private static bool IsTruncated_SingleLine(this Label label) {
        var requiredSize = MeasureSingleLine(label);
        return requiredSize.X > label.Size.X;
    }

    private static Vector2 MeasureSingleLine(Label label) {
        Debug.Assert(label.GetLineCount()           == 1);
        Require.Argument(label, label.LabelSettings != null);

        var requiredSize = label.LabelSettings.Font.GetStringSize(
            text: label.Text,
            alignment: label.HorizontalAlignment,
            justificationFlags: label.JustificationFlags,
            fontSize: label.LabelSettings.FontSize,
            direction: label.TextDirection.ToDirection()
        );

        return requiredSize;
    }

    /// <returns><see langword="true"/> if not all of my lines are showing.
    /// Note that this is meaningless when <see cref="Label.AutowrapMode"/> is <see cref="TextServer.AutowrapMode.Off"/></returns>
    /// <remarks>
    /// ⚠️ WARNING ⚠️
    /// <p/>
    /// <see cref="Label.IsClippingText"/> is actual an alias for <see cref="Label.ClipText"/>, and has nothing to do with the actual state of the <see cref="Label"/>!
    /// </remarks>
    public static bool IsTruncated(this Label label) {
        var lineCount = label.GetLineCount();
        return lineCount switch {
            <= 0 => false,
            1    => label.IsTruncated_SingleLine(),
            > 1  => lineCount > label.GetVisibleLineCount()
        };
    }

    public static void Describe(this RichTextLabel label) {
        label.blog(
            label.GetLineCount(),
            label.GetVisibleLineCount(),
            label.GetVisibleContentRect(),
            label.GetContentHeight(),
            label.AnchorRight,
            label.OffsetRight
            );
    }

    public static void Describe(this Label label) {
        label.blog(
            label.Text,
            label.GetMinimumSize(),
            label.GetCustomMinimumSize(),
            label.GetSize(),
            label.GetRect(),
            label.GetLineHeight(),
            label.GetLineCount(),
            label.GetVisibleCharacters(),
            label.GetVisibleLineCount(),
            label.IsTruncated(),
            label.IsClippingText()
        );
    }

    public static RichTextLabel Append(
        this RichTextLabel label,
        string             literalText,
        TextDecoration     decoration = TextDecoration.None
    ) {
        if (string.IsNullOrEmpty(literalText)) {
            return label;
        }

        if (decoration == TextDecoration.None) {
            label.AddText(literalText);
            return label;
        }

        label.PushContext();

        if (decoration.HasFlag(TextDecoration.Bold)) {
            label.PushBold();
        }

        if (decoration.HasFlag(TextDecoration.Italics)) {
            label.PushItalics();
        }

        if (decoration.HasFlag(TextDecoration.Underline)) {
            label.PushUnderline();
        }

        if (decoration.HasFlag(TextDecoration.Strikethrough)) {
            label.PushStrikethrough();
        }

        label.AddText(literalText);
        label.PopContext();

        return label;
    }

    public static RichTextLabel AppendLine(
        this RichTextLabel label,
        string             content,
        TextDecoration decoration = TextDecoration.None
    ) {
        label.Append(content, decoration)
            .AddText("\n");

        return label;
    }

    [Flags]
    public enum TextDecoration {
        None = 0,
        Bold= 1,
        Italics = 2,
        Underline = 4,
        Strikethrough =8
    }
}