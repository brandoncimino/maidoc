using System;
using Godot;

namespace maidoc.Scenes.GameComponents;

public readonly record struct PaddingRatio {
    public float Left       { get; init; }
    public float Right      { get; init; }
    public float Top        { get; init; }
    public float Bottom     { get; init; }

    public float Horizontal {
        init {
            Left  = value;
            Right = value;
        }
    }

    public float Vertical {
        init {
            Top    = value;
            Bottom = value;
        }
    }

    public float All {
        init {
            Horizontal = value;
            Vertical   = value;
        }
    }

    public PaddingAmount ComputePaddingAmount(Vector2 size) {
        return new PaddingAmount() {
            Left   = size.X * Left,
            Right  = size.X * Right,
            Top    = size.Y * Top,
            Bottom = size.Y * Bottom
        };
    }

    public static Rect2 operator +(in Rect2 rect2, in PaddingRatio paddingRatio) {
        var paddingAmount = paddingRatio.ComputePaddingAmount(rect2.Size);
        return rect2 + paddingAmount;
    }

    public static Rect2 operator -(in Rect2 rect2, in PaddingRatio paddingRatio) {
        var paddingAmount = paddingRatio.ComputePaddingAmount(rect2.Size);
        return rect2 - paddingAmount;
    }

    public float this[Side side] => side switch {
        Side.Left   => Left,
        Side.Right  => Right,
        Side.Top    => Top,
        Side.Bottom => Bottom,
        _           => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };
}

public readonly record struct PaddingAmount {
    public float Left   { get; init; }
    public float Right  { get; init; }
    public float Top    { get; init; }
    public float Bottom { get; init; }

    public float Horizontal {
        init {
            Left  = value;
            Right = value;
        }
    }

    public float Vertical {
        init {
            Top    = value;
            Bottom = value;
        }
    }

    public Vector2 TopLeft     => new(Left, Top);
    public Vector2 BottomRight => new(Right, Bottom);

    public static Rect2 operator +(in Rect2 rect2, in PaddingAmount paddingAmount) {
        var topLeft     = rect2.Position + paddingAmount.TopLeft;
        var bottomRight = rect2.End      - paddingAmount.BottomRight;

        return new Rect2(
            topLeft,
            bottomRight - topLeft
        );
    }

    public static Rect2 operator -(in Rect2 rect2, in PaddingAmount paddingAmount) {
        var topLeft     = rect2.Position - paddingAmount.TopLeft;
        var bottomRight = rect2.End      + paddingAmount.BottomRight;

        return new Rect2(
            topLeft,
            bottomRight - topLeft
        );
    }
}

public static class PaddingExtensions {

}

public readonly record struct SideMap<T> {
    public T? Left   { get; init; }
    public T? Right  { get; init; }
    public T? Top    { get; init; }
    public T? Bottom { get; init; }

    public T? Horizontal {
        init {
            Left  = value;
            Right = value;
        }
    }

    public T? Vertical {
        init {
            Top    = value;
            Bottom = value;
        }
    }

    public T? All {
        init {
            Horizontal = value;
            Vertical   = value;
        }
    }

    public T? this[Side side] => side switch {
        Side.Left   => Left,
        Side.Top    => Top,
        Side.Right  => Right,
        Side.Bottom => Bottom,
        _           => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };
}