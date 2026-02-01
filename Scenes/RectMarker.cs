using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

namespace maidoc.Scenes;

[Tool]
public partial class RectMarker : Node2D {
    private readonly float _screenWidth = ProjectSettings.GetSetting("display/window/size/viewport_width").AsSingle();
    private readonly float _screenHeight = ProjectSettings.GetSetting("display/window/size/viewport_height").AsSingle();
    private          ReferenceRect? Rect => GetChild<ReferenceRect>(0);

    private RichTextLabel? Label =>
        Rect?.GetChild<RichTextLabel>(0); //GetNode<RichTextLabel>("ReferenceRect/RichTextLabel");

    private const string SizeGroup  = "Size";
    private const string SizePrefix = "SizeIn";

    private const string Meters  = "suffix:m";
    private const string Pixels  = "suffix:px";
    private const string Screens = "suffix:screens";

    #region Rect2

    public Rect2 RectInPixels  => new(PositionInPixels, SizeInPixels);
    public Rect2 RectInMeters  => new(PositionInMeters, SizeInMeters);
    public Rect2 RectInScreens => new(PositionInScreens, SizeInScreens);

    #endregion

    /// <summary>
    /// What the <see cref="RectMarker.SizeInScreens"/> values are proportional to, i.e. what you multiply them by to get the <see cref="RectMarker.SizeInPixels"/>.
    /// </summary>
    public enum ReferenceAxis {
        /// <summary>
        /// The <see cref="RectMarker.SizeInScreens"/>.<see cref="Vector2.X"/> and <see cref="Vector2.Y"/> will be multiplied by the <see cref="RectMarker._screenWidth"/> and <see cref="RectMarker._screenHeight"/>, respectively.
        /// </summary>
        Corresponding,

        /// <summary>
        /// Both the <see cref="RectMarker.SizeInScreens"/>.<see cref="Vector2.X"/> and <see cref="Vector2.Y"/> will be multiplied by the <see cref="RectMarker._screenWidth"/>.
        /// </summary>
        X,

        /// <summary>
        /// Both the <see cref="RectMarker.SizeInScreens"/>.<see cref="Vector2.X"/> and <see cref="Vector2.Y"/> will be multiplied by the <see cref="RectMarker._screenHeight"/>.
        /// </summary>
        Y
    }

    /// <inheritdoc cref="ReferenceAxis"/>
    [Export]
    public ReferenceAxis ScreenRatioAxis { get; set; }

    /// <inheritdoc cref="ReferenceAxis"/>
    [Export]
    public ReferenceAxis SelfRatioAxis { get; set; }


    [Export]
    private Color BorderColor { get => Rect?.BorderColor ?? default; set => Rect?.SetBorderColor(value); }

    [Export]
    private Color FillColor { get => Label?.SelfModulate ?? default; set => Label?.SetSelfModulate(value); }


    [Export(hintString: Pixels)]
    private float BorderWidth { get => Rect?.BorderWidth ?? default; set => Rect?.SetBorderWidth(value); }

    [Export]
    public bool EditorOnly { get => Rect?.EditorOnly ?? default; set => Rect?.SetEditorOnly(value); }

    [Export]
    public bool Fill { get => Label?.Visible ?? default; set => Label?.SetVisible(value); }

    #region Size

    /// <summary>
    /// Based on <see cref="GodotHelpers.GodotUnitsPerMeter"/>
    /// </summary>
    [Export(hintString: Meters)]
    [ExportGroup(SizeGroup, SizePrefix)]
    public Vector2 SizeInMeters {
        get => SizeInPixels / GodotHelpers.GodotUnitsPerMeter;
        set => SizeInPixels = value * GodotHelpers.GodotUnitsPerMeter;
    }


    [Export(hintString: "suffix:px")]
    [ExportGroup(SizeGroup, SizePrefix)]
    public Vector2 SizeInPixels { get => Rect?.Size ?? default; set => UpdateSizeInPixels(value, OffsetInPixels); }


    [Export(hintString: "suffix:screens")]
    [ExportGroup(SizeGroup, SizePrefix)]
    public Vector2 SizeInScreens {
        get => Ratio2D.CalculateRatio(SizeInPixels, GetScreenSize(), ScreenRatioAxis);
        set => SizeInPixels = Ratio2D.CalculateActual(value, GetScreenSize(), ScreenRatioAxis);
    }

    [Export(hintString: "suffix:parents")]
    [ExportGroup(SizeGroup, SizePrefix)]
    public Vector2 SizeInParents {
        get =>
            GetParentSize() switch {
                { } parentSize => SizeInPixels.GetByRatio(parentSize),
                _              => default
            };
        set {
            if (GetParentSize() is { } parentSize) {
                GD.Print($"Setting {nameof(SizeInPixels)} to {value} * {nameof(parentSize)} {parentSize}");
                SizeInPixels = Ratio2D.CalculateActual(value, parentSize, ReferenceAxis.Corresponding);
            }
        }
    }

    #endregion

    #region Position

    private const string PositionGroup  = "Position";
    private const string PositionPrefix = "PositionIn";

    /// <summary>
    /// Based on <see cref="GodotHelpers.GodotUnitsPerMeter"/>
    /// </summary>
    [Export(hintString: Meters)]
    [ExportGroup(PositionGroup, PositionPrefix)]
    // [ExportCategory("Junk")]
    public Vector2 PositionInMeters {
        get => PositionInPixels / GodotHelpers.GodotUnitsPerMeter;
        set => PositionInPixels = value * GodotHelpers.GodotUnitsPerMeter;
    }

    [Export(hintString: Pixels)]
    [ExportGroup(PositionGroup, PositionPrefix)]
    public Vector2 PositionInPixels { get => Position; set => Position = value; }

    [Export(hintString: Screens)]
    [ExportGroup(PositionGroup, PositionPrefix)]
    public Vector2 PositionInScreens {
        get => Ratio2D.CalculateRatio(PositionInPixels, GetScreenSize(), ScreenRatioAxis);
        set => PositionInPixels = Ratio2D.CalculateActual(value, GetScreenSize(), ScreenRatioAxis);
    }

    #region Offset

    private const string OffsetGroup  = "Offset";
    private const string OffsetPrefix = "OffsetIn";

    private Vector2 _offsetInPixels;

    [Export(hintString: Pixels)]
    [ExportGroup(PositionGroup)]
    [ExportSubgroup(OffsetGroup, OffsetPrefix)]
    public Vector2 OffsetInPixels {
        get => _offsetInPixels;
        set {
            _offsetInPixels = value;
            UpdateSizeInPixels(SizeInPixels, value);
        }
    }

    [Export(hintString: Meters)]
    [ExportGroup(PositionGroup)]
    [ExportSubgroup(OffsetGroup, OffsetPrefix)]
    public Vector2 OffsetInMeters {
        get => OffsetInPixels / GodotHelpers.GodotUnitsPerMeter;
        set => OffsetInPixels = value / GodotHelpers.GodotUnitsPerMeter;
    }

    [Export(hintString: Screens)]
    [ExportGroup(PositionGroup)]
    [ExportSubgroup(OffsetGroup, OffsetPrefix)]
    public Vector2 OffsetInScreens {
        get => Ratio2D.CalculateRatio(OffsetInPixels, GetScreenSize(), ScreenRatioAxis);
        set => OffsetInPixels = Ratio2D.CalculateActual(value, GetScreenSize(), ScreenRatioAxis);
    }

    [Export(hintString: "suffix:selves")]
    [ExportGroup(PositionGroup)]
    [ExportSubgroup(OffsetGroup, OffsetPrefix)]
    public Vector2 OffsetInSelves {
        get => Ratio2D.CalculateRatio(OffsetInPixels, SizeInPixels, SelfRatioAxis);
        set => OffsetInPixels = Ratio2D.CalculateActual(value, SizeInPixels, SelfRatioAxis);
    }

    public override Array<Dictionary> _GetPropertyList() {
        IEnumerable<GodotProperty> properties = [
            GodotProperty.Header(SizeGroup),
            SizeInPixels.CreateGodotProperty(),
            SizeInMeters.CreateGodotProperty(),
            SizeInScreens.CreateGodotProperty(),
            SizeInParents.CreateGodotProperty(),

            GodotProperty.Header(PositionGroup),
            PositionInPixels.CreateGodotProperty(),
            PositionInMeters.CreateGodotProperty(),
            PositionInScreens.CreateGodotProperty(),

            GodotProperty.Header(OffsetGroup, GodotProperty.HeaderType.Subgroup),
            OffsetInPixels.CreateGodotProperty(),
            OffsetInMeters.CreateGodotProperty(),
            OffsetInScreens.CreateGodotProperty(),
            OffsetInSelves.CreateGodotProperty()
        ];

        return [
            ..properties.Select(it => it.ToGodotDictionary())
        ];
    }

    public Vector2 BonusProperty { get; set; }

    // public override Variant _Get(StringName property) {
    //     if (property == PropertyName.SizeInMeters) {
    //
    //     }
    // }

    #endregion

    #endregion

    // // 📎 Note: Setting "revert" defaults isn't necessary here, since we intend to use this as a packed scene,
    //    // and the values that were packed into the scene are considered the "defaults" by the parent scene.
    //
    // public override bool _PropertyCanRevert(StringName property) {
    //     return property    == PropertyName.BorderColor
    //            || property == PropertyName.SizeInMeters
    //            || property == PropertyName.SizeInPixels
    //            || property == PropertyName.BorderWidth
    //            || property == PropertyName.FillColor
    //            || property == PropertyName.EditorOnly
    //         ;
    // }
    //
    // public override Variant _PropertyGetRevert(StringName property) {
    //     return property switch {
    //         _ when property == PropertyName.BorderColor  => Colors.Teal,
    //         _ when property == PropertyName.SizeInMeters => Vector2.One,
    //         _ when property == PropertyName.SizeInPixels => Vector2.One * GodotHelpers.GodotUnitsPerMeter,
    //         _ when property == PropertyName.BorderWidth  => 1f,
    //         _ when property == PropertyName.FillColor    => Colors.White with { A = .2f },
    //         _ when property == PropertyName.EditorOnly   => true,
    //         _                                            => base._PropertyGetRevert(property),
    //     };
    // }

    private string? CanUpdate() {
        if (!IsInsideTree()) {
            return $"{nameof(IsInsideTree)} is false.";
        }

        return null;
    }

    private void UpdateSizeInPixels(
        Vector2                   size,
        Vector2                   offset,
        [CallerMemberName] string _caller = ""
    ) {
        if (CanUpdate() is { } whyNot) {
            GD.Print($"[{_caller}] cannot {nameof(UpdateSizeInPixels)} because: {whyNot}");
            return;
        }

        if (!size.IsFinite()) {
            GD.Print(
                $"[{_caller}] {nameof(size)} of {size} isn't finite, which implies that the scene isn't ready for prime time yet. Skipping {nameof(UpdateSizeInPixels)}."
            );
        }

        if (Rect is not { } rect) {
            return;
        }

        var targetRect = GodotHelpers.Rect2ByCenter(
            offset,
            size
        );

        rect.Position = targetRect.Position;
        rect.Size     = targetRect.Size;

        RefreshText();
    }

    private Vector2 GetScreenSize() {
        return new Vector2(_screenWidth, _screenHeight);
    }


    private Vector2? GetParentSize() {
        return GetParent() switch {
            Control control       => control.Size,
            RectMarker rectMarker => rectMarker.SizeInPixels,
            _                     => null
        };
    }

    public Node2D AsNode2D => this;

    private void RefreshText() {
        if (Label is not { Visible: true } label || Rect is not { } rect) {
            return;
        }

        var globalRect         = rect.GetGlobalRect();
        var globalSizeInMeters = globalRect.Size / GodotHelpers.GodotUnitsPerMeter;
        label.Text = $"""
                      {Name}
                      🌍 {globalSizeInMeters.X:0.#} ⨉ {globalSizeInMeters.Y:0.#} m
                      """;
    }

    public override void _Notification(int what) {
        if (what == NotificationPathRenamed) {
            RefreshText();
        }
    }
}

public static class Ratio2D {
    public static Vector2 CalculateActual(
        Vector2                  ratio,
        Vector2                  reference,
        RectMarker.ReferenceAxis referenceAxis
    ) {
        return referenceAxis switch {
            RectMarker.ReferenceAxis.Corresponding => reference * ratio,
            RectMarker.ReferenceAxis.X => reference * ratio.X,
            RectMarker.ReferenceAxis.Y => reference * ratio.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(referenceAxis), referenceAxis, null)
        };
    }

    public static Vector2 GetByRatio(
        this in Vector2          backer,
        in      Vector2          reference,
        RectMarker.ReferenceAxis referenceAxis = RectMarker.ReferenceAxis.Corresponding
    ) {
        return CalculateRatio(backer, reference, referenceAxis);
    }

    [Pure]
    public static Vector2 SetByRatio(
        this in Vector2          backer,
        in      Vector2          ratio,
        Func<Vector2>            reference,
        RectMarker.ReferenceAxis referenceAxis = RectMarker.ReferenceAxis.Corresponding,
        [CallerArgumentExpression(nameof(reference))]
        string _reference = ""
    ) {
        var got = reference();
        if (!got.IsFinite()) {
            return backer;
        }

        var actual = CalculateActual(ratio, got, referenceAxis);
        if (!actual.IsFinite()) {
            GD.Print(
                $"Computed value from ratio {ratio} of {_reference} {got} was {actual}, which is not finite - this implies that the scene isn't ready for prime time yet. Returning the original value of {backer}."
            );
            return backer;
        }

        return actual;
    }

    public static Vector2 CalculateRatio(
        Vector2                  actual,
        Vector2                  reference,
        RectMarker.ReferenceAxis referenceAxis
    ) {
        return referenceAxis switch {
            RectMarker.ReferenceAxis.Corresponding => actual / reference,
            RectMarker.ReferenceAxis.X => actual / reference.X,
            RectMarker.ReferenceAxis.Y => actual / reference.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(referenceAxis), referenceAxis, null)
        };
    }
}