using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using BSharp.Core;
using Godot;
using maidoc.Core;
using maidoc.Scenes.GameComponents;
using Side = Godot.Side;
using Vector2 = Godot.Vector2;

namespace maidoc.Scenes;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static partial class GodotHelpers {
    /// <summary>
    /// Based on Godot's default <see cref="Area2D.Gravity"/>, which is <c>980</c>, compared to the IRL value of 9.8 meters/second².
    /// </summary>
    public const float GodotPixelsPerMeter = 100;

    public static Action<string> Printer { private get; set; } = Godot.GD.Print;

    public static string OrNullPlaceholder<T>(this T? value, string nullPlaceholder = "⛔") =>
        value?.ToString() ?? nullPlaceholder;

    public static string OrNullPlaceholder<T>(this T? value, Func<T, string> formatter, string nullPlaceholder = "⛔") =>
        value switch {
            null => nullPlaceholder,
            _    => formatter(value)
        };

    #region Silly blogging

    private static class GD {
        public static void Print(string message) {
            Printer(message);
        }
    }

    private static string FormatBlogPost(
        string                        label,
        (string Key, object? Value)[] stuff,
        string                        indent            = " ↳ ",
        string                        keyValueSeparator = " → "
    ) {
        var keys       = stuff.Select(it => it.Key.TrimPrefix(label)).ToArray();
        var longestKey = keys.IsEmpty() ? 0 : keys.Max(it => it.Length);

        return stuff.Select((it, index) =>
                        $"{indent}{keys[index].PadRight(longestKey)}{keyValueSeparator}{it.Value.ToBlogString()}"
                    )
                    .JoinString(separator: "\n", prefix: $"{label}\n");
    }

    private static string ToBlogString<T>(this T value) {
        return value switch {
            Node node => $"{node.Name} {node}",
            _         => value.OrNullPlaceholder()
        };
    }

    public static T blog<T, A>(
        this T self,
        A      a,
        [CallerArgumentExpression(nameof(self))]
        string label = "",
        [CallerArgumentExpression(nameof(a))] string _a = ""
    ) {
        GD.Print(
            FormatBlogPost(
                label,
                [(_a, a)]
            )
        );

        return self;
    }

    public static T blog<T, A, B>(
        this T self,
        A      a,
        B      b,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [(_a, a), (_b, b)]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C>(
        this T self,
        A      a,
        B      b,
        C      c,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F, G>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        G      g,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = "",
        [CallerArgumentExpression(nameof(g))] string _g = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f),
                    (_g, g)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F, G, H>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        G      g,
        H      h,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = "",
        [CallerArgumentExpression(nameof(g))] string _g = "",
        [CallerArgumentExpression(nameof(h))] string _h = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f),
                    (_g, g),
                    (_h, h)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F, G, H, I>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        G      g,
        H      h,
        I      i,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = "",
        [CallerArgumentExpression(nameof(g))] string _g = "",
        [CallerArgumentExpression(nameof(h))] string _h = "",
        [CallerArgumentExpression(nameof(i))] string _i = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f),
                    (_g, g),
                    (_h, h),
                    (_i, i)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F, G, H, I, J>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        G      g,
        H      h,
        I      i,
        J      j,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = "",
        [CallerArgumentExpression(nameof(g))] string _g = "",
        [CallerArgumentExpression(nameof(h))] string _h = "",
        [CallerArgumentExpression(nameof(i))] string _i = "",
        [CallerArgumentExpression(nameof(j))] string _j = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f),
                    (_g, g),
                    (_h, h),
                    (_i, i),
                    (_j, j)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F, G, H, I, J, K>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        G      g,
        H      h,
        I      i,
        J      j,
        K      k,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = "",
        [CallerArgumentExpression(nameof(g))] string _g = "",
        [CallerArgumentExpression(nameof(h))] string _h = "",
        [CallerArgumentExpression(nameof(i))] string _i = "",
        [CallerArgumentExpression(nameof(j))] string _j = "",
        [CallerArgumentExpression(nameof(k))] string _k = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f),
                    (_g, g),
                    (_h, h),
                    (_i, i),
                    (_j, j),
                    (_k, k)
                ]
            )
        );

        return self;
    }

    public static T blog<T, A, B, C, D, E, F, G, H, I, J, K, L>(
        this T self,
        A      a,
        B      b,
        C      c,
        D      d,
        E      e,
        F      f,
        G      g,
        H      h,
        I      i,
        J      j,
        K      k,
        L      l,
        [CallerArgumentExpression(nameof(self))]
        string _self = "",
        [CallerArgumentExpression(nameof(a))] string _a = "",
        [CallerArgumentExpression(nameof(b))] string _b = "",
        [CallerArgumentExpression(nameof(c))] string _c = "",
        [CallerArgumentExpression(nameof(d))] string _d = "",
        [CallerArgumentExpression(nameof(e))] string _e = "",
        [CallerArgumentExpression(nameof(f))] string _f = "",
        [CallerArgumentExpression(nameof(g))] string _g = "",
        [CallerArgumentExpression(nameof(h))] string _h = "",
        [CallerArgumentExpression(nameof(i))] string _i = "",
        [CallerArgumentExpression(nameof(j))] string _j = "",
        [CallerArgumentExpression(nameof(k))] string _k = "",
        [CallerArgumentExpression(nameof(l))] string _l = ""
    ) {
        GD.Print(
            FormatBlogPost(
                _self,
                [
                    (_a, a),
                    (_b, b),
                    (_c, c),
                    (_d, d),
                    (_e, e),
                    (_f, f),
                    (_g, g),
                    (_h, h),
                    (_i, i),
                    (_j, j),
                    (_k, k),
                    (_l, l)
                ]
            )
        );

        return self;
    }


    public static T blog<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))]
        string label = "",
        int labelColumnWidth = 20
    ) {
        var fullLabel = $"{label} [{typeof(T).Name}]".Trim();

        var msg = new StringBuilder(fullLabel);
        if (fullLabel.Length >= labelColumnWidth) {
            msg.Append('\n')
               .Append(' ', labelColumnWidth)
               .Append('↳')
               .Append(' ');
        }
        else {
            msg.Append(' ')
               .Append('→')
               .Append(' ');
        }

        msg.Append(value.OrNullPlaceholder());

        GD.Print(msg.ToString());

        return value;
    }

    #endregion

    #region Sprite2D extensions

    public static Vector2 WorldSize(this Sprite2D sprite2D) {
        return sprite2D.Texture.GetSize() * sprite2D.GlobalScale;
    }

    public static Vector2 WorldOffset(this Sprite2D sprite2D) {
        return sprite2D.Offset * sprite2D.GlobalScale;
    }

    public static Vector2 WorldCenter(this Sprite2D sprite2D) {
        var worldPosition = sprite2D.GlobalPosition;
        var worldOffset   = sprite2D.WorldOffset();
        return worldPosition + worldOffset;
    }

    public static Rect2 WorldRect(this Sprite2D sprite2D) {
        return new Rect2(
            sprite2D.WorldCenter() - sprite2D.WorldSize() * .5f,
            sprite2D.WorldSize()
        );
    }

    private static ValueTuple AdjustSizeAndPosition(
        Control control,
        Rect2   desiredRectInMeters
    ) {
        GD.Print($"{nameof(AdjustSizeAndPosition)} of {control} to {desiredRectInMeters} meters");
        var desiredRectInGodotUnits = desiredRectInMeters.ResizeFromCenter(GodotPixelsPerMeter);

        if (control is RichTextLabel) {
            control.Position = desiredRectInGodotUnits.GetCenter();
        }
        else {
            control.Position = desiredRectInGodotUnits.Position;
        }

        // Only one of these two sizes matters, depending on whether the `Control` has a parent `Container` or not.
        control.Size              = desiredRectInGodotUnits.Size;
        control.CustomMinimumSize = desiredRectInGodotUnits.Size;

        return default;
    }

    public static T AdjustSizeAndPosition<T>(
        this T       object2D,
        RectDistance desiredRect
    ) where T : CanvasItem {
        return object2D.AdjustSizeAndPosition(desiredRect.Meters);
    }

    public static T AdjustSizeAndPosition<T>(
        this T object2D,
        Rect2  desiredRectInMeters
    ) where T : CanvasItem {
        _ = object2D switch {
            Sprite2D sprite2D => AdjustSizeAndPosition(sprite2D, desiredRectInMeters),
            Control control   => AdjustSizeAndPosition(control,  desiredRectInMeters),
            Node2D node2D     => AdjustSizeAndPosition(node2D,   desiredRectInMeters),
            _                 => throw new ArgumentOutOfRangeException(nameof(object2D), object2D, null)
        };

        return object2D;
    }

    private static ValueTuple AdjustSizeAndPosition(
        Node2D node2D,
        Rect2  desiredRectInMeters
    ) {
        Require.Argument(node2D, node2D is not Sprite2D);

        // At this point, we assume that, at scale 1, this object matches 1 meter by 1 meter.
        // For example, that would require a `CollisionShape2D` with a `Shape` that is a 100 x 100 rectangle.
        node2D.Position = desiredRectInMeters.GetCenter() * GodotPixelsPerMeter;
        node2D.Scale    = desiredRectInMeters.Size;

        return default;
    }

    private static ValueTuple AdjustSizeAndPosition(
        Sprite2D sprite2D,
        Rect2    desiredRectInMeters
    ) {
        sprite2D.NormalizeSize(
            maintainAspectRatio: false,
            desiredSizeInMeters: desiredRectInMeters.Size
        );

        GD.Print($"Old sprite2D position: {sprite2D.Position}");
        GD.Print("Desired (meters): " + desiredRectInMeters.GetCenter());
        GD.Print($"Desired (* {GodotPixelsPerMeter}: {desiredRectInMeters.GetCenter() * GodotPixelsPerMeter}");
        sprite2D.Position = desiredRectInMeters.GetCenter() * GodotPixelsPerMeter;
        GD.Print($"After moving: {sprite2D.Position}");

        return default;
    }

    /// <summary>
    /// Modifies my <see cref="Node2D.Scale"/> so that I visually have the <see cref="desiredSizeInMeters"/>, rather than being dependent on my <see cref="Texture2D.GetSize"/>.
    /// </summary>
    public static Sprite2D NormalizeSize(
        this Sprite2D sprite2D,
        bool          maintainAspectRatio,
        in Vector2?   desiredSizeInMeters = null
    ) {
        var spriteSize          = sprite2D.Texture.GetSize();
        var actualDesiredMeters = desiredSizeInMeters ?? Vector2.One;
        if (maintainAspectRatio) {
            var smallerAxis           = spriteSize.MinAxisIndex();
            var smallerAxisMultiplier = spriteSize.Sorted().Aspect();
            actualDesiredMeters *= Vector2.One.WithAxis(smallerAxis, smallerAxisMultiplier);
        }

        var desiredSizeInGodot = actualDesiredMeters * GodotPixelsPerMeter;
        var requiredScale      = desiredSizeInGodot  / spriteSize;

        sprite2D.Scale = requiredScale;

        return sprite2D;
    }

    #endregion

    public static Vector2 ToVector2(this int   xy) => new(xy, xy);
    public static Vector2 ToVector2(this float xy) => new(xy, xy);

    public static Rect2 Rect2ByCenter(
        in Vector2 center,
        in Vector2 size
    ) {
        return new Rect2(
            center - (size / 2),
            size
        );
    }

    public static Rect2 WithCenter(in this Rect2 rect2, Vector2 newCenter) {
        var offset = rect2.GetCenter() - newCenter;
        return rect2 with {
            Position = rect2.Position + offset
        };
    }

    public enum PaddingResizeMode {
        MaintainOffset,
        MaintainRatio
    }


    public readonly record struct ParentBinding {
        public float             Amount       { get; init; }
        public AmountFlavor      AmountFlavor { get; init; }
        public PaddingResizeMode ResizeMode   { get; init; }

        public float Ratio {
            init {
                Amount       = value;
                AmountFlavor = AmountFlavor.Ratio;
            }
        }

        public float Meters {
            init {
                Amount       = value;
                AmountFlavor = AmountFlavor.Meters;
            }
        }

        internal float GetOffsetInGodotUnits(float parentSizeInGodotUnits) {
            return AmountFlavor switch {
                AmountFlavor.Meters => Amount * GodotPixelsPerMeter,
                AmountFlavor.Ratio  => Amount * parentSizeInGodotUnits,
                _                   => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum AmountFlavor {
        Meters,
        Ratio
    }

    public static void BindToParent(
        this Control                control,
        in   SideMap<ParentBinding> parentBindings = default
    ) {
        GD.Print($"Binding {control} to its parent at: {parentBindings}");

        control.Scale = Vector2.One;

        var childRectInGodotUnits = control.ComputeRectInParentInGodotUnits(parentBindings);
        childRectInGodotUnits.blog();
        var childRectInMeters = childRectInGodotUnits.Scale(1f / GodotPixelsPerMeter);
        control.AdjustSizeAndPosition(childRectInMeters);

        AdjustAnchorsAndOffsets(control, parentBindings);
    }

    private static void AdjustAnchorsAndOffsets(Control control, SideMap<ParentBinding> parentBindings) {
        parentBindings.Left.ApplyToChild(Side.Left, control);
        parentBindings.Right.ApplyToChild(Side.Right, control);
        parentBindings.Top.ApplyToChild(Side.Top, control);
        parentBindings.Bottom.ApplyToChild(Side.Bottom, control);
    }

    private static Rect2 ComputeRectInParentInGodotUnits(this Control child, in SideMap<ParentBinding> parentBindings) {
        var parentRect = child.GetParentControl().GetRect();

        var leftOffset   = parentBindings.Left.GetOffsetInGodotUnits(parentRect.Size.X);
        var rightOffset  = parentBindings.Right.GetOffsetInGodotUnits(parentRect.Size.X);
        var topOffset    = parentBindings.Top.GetOffsetInGodotUnits(parentRect.Size.Y);
        var bottomOffset = parentBindings.Bottom.GetOffsetInGodotUnits(parentRect.Size.Y);

        var topLeft     = new Vector2(leftOffset, topOffset);
        var bottomRight = parentRect.Size - new Vector2(rightOffset, bottomOffset);
        return Rect2ByCorners(topLeft, bottomRight);
    }

    private static void ApplyToChild(
        in this ParentBinding parentBinding,
        Side                  side,
        Control               child
    ) {
        child._BindToParentSide(side, parentBinding.Amount, parentBinding.AmountFlavor, parentBinding.ResizeMode);
    }

    private static void _BindToParentSide(
        this Control      control,
        Side              side,
        float             amount,
        AmountFlavor      amountFlavor,
        PaddingResizeMode resizeMode
    ) {
        var actualAvailableSpace = Require.NotNull(control.GetParentControl())
                                          .Size
                                          .GetAxis(side.Axis());

        actualAvailableSpace.blog();

        var isReverseSide = side is Side.Right or Side.Bottom;

        var ratio = amountFlavor switch {
            AmountFlavor.Ratio  => amount,
            AmountFlavor.Meters => amount * GodotPixelsPerMeter / actualAvailableSpace,
            _                   => throw new ArgumentOutOfRangeException(nameof(amountFlavor), amountFlavor, null)
        };

        var anchorAmount = resizeMode switch {
            PaddingResizeMode.MaintainOffset => 0,
            PaddingResizeMode.MaintainRatio => ratio,
            _ => throw new ArgumentOutOfRangeException(nameof(resizeMode), resizeMode, null)
        };

        anchorAmount.blog();

        if (isReverseSide) {
            anchorAmount = 1 - anchorAmount;
        }

        var offsetAmount = actualAvailableSpace * ratio;

        if (isReverseSide) {
            // var prevOffsetAmount = offsetAmount;
            // offsetAmount = actualAvailableSpace - offsetAmount;
            // GD.Print($"Side {side} is a 'reverse side', so we're modifying the {nameof(offsetAmount)} from {prevOffsetAmount} -> {offsetAmount}");
            offsetAmount *= -1;
        }

        control.SetAnchorAndOffset(
            side,
            anchorAmount,
            offsetAmount,
            false
        );
    }
}