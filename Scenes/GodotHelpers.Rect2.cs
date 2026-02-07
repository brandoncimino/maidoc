using System;
using System.Collections.Generic;
using Godot;

namespace maidoc.Scenes;

public static partial class GodotHelpers {
    public static Vector2.Axis Axis(this Side side) => side switch {
        Side.Left or Side.Right => Vector2.Axis.X,
        Side.Top or Side.Bottom => Vector2.Axis.Y,
        _                       => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };

    public static Vector2.Axis Other(this Vector2.Axis axis) => axis switch {
        Vector2.Axis.X => Vector2.Axis.Y,
        Vector2.Axis.Y => Vector2.Axis.X,
        _              => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
    };

    public static Corner Inverse(this Corner corner) => corner switch {
        Corner.TopLeft     => Corner.BottomRight,
        Corner.TopRight    => Corner.BottomLeft,
        Corner.BottomRight => Corner.TopLeft,
        Corner.BottomLeft  => Corner.TopRight,
        _                  => throw new ArgumentOutOfRangeException(nameof(corner), corner, null)
    };

    public static float GetSide(this Rect2 rect2, Side side) => side switch {
        Side.Left   => rect2.Position.X,
        Side.Top    => rect2.Position.Y,
        Side.Right  => rect2.End.X,
        Side.Bottom => rect2.End.Y,
        _           => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };

    public static Distance2D GetSidePoint(
        this in RectDistance rect,
        Side                 side,
        float                sideLerpAmount = .5f
    ) {
        return rect.Meters.GetSidePoint(side, sideLerpAmount).Meters;
    }

    public static Vector2 GetSidePoint(
        this in Rect2 rect2,
        Side          side,
        float         sideLerpAmount = .5f
    ) {
        return side switch {
            Side.Top    => rect2.Position + new Vector2(sideLerpAmount * rect2.Size.X, 0),
            Side.Left   => rect2.Position + new Vector2(0,                             sideLerpAmount * rect2.Size.Y),
            Side.Right  => rect2.End      - new Vector2(0,                             sideLerpAmount * rect2.Size.Y),
            Side.Bottom => rect2.End      - new Vector2(sideLerpAmount * rect2.Size.X, 0),
            _           => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    /// <summary>
    /// Similar to the standard <see cref="Rect2(Vector2,Vector2)"/> constructor, but uses any given <see cref="Corner"/> instead of <see cref="Rect2.Position"/> (i.e. <see cref="Corner.TopLeft"/>)
    /// </summary>
    public static Rect2 Rect2ByCorner(
        Corner     corner,
        in Vector2 cornerPosition,
        in Vector2 size
    ) {
        return corner switch {
            Corner.TopLeft => new Rect2(cornerPosition, size),
            Corner.TopRight => Rect2ByCornersUphill(
                bottomLeft: cornerPosition + new Vector2(-size.X, size.Y),
                topRight: cornerPosition
            ),
            Corner.BottomRight => new Rect2(cornerPosition - size, size),
            Corner.BottomLeft => Rect2ByCornersUphill(
                bottomLeft: cornerPosition,
                topRight: cornerPosition + new Vector2(size.X, -size.Y)
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(corner), corner, null)
        };
    }

    public static RectDistance Rect2BySide(
        Side       side,
        Distance2D sidePosition,
        Distance2D size,
        float      sideLerpAmount = .5f
    ) {
        var meters = Rect2BySide(
            side,
            sidePosition.Meters,
            size.Meters,
            sideLerpAmount
        );

        return meters.Meters;
    }

    public static Rect2 Rect2BySide(
        Side    side,
        Vector2 sidePosition,
        Vector2 size,
        float   sideLerpAmount = .5f
    ) {
        if (sideLerpAmount is < 0 or > 1) {
            throw new ArgumentOutOfRangeException(
                nameof(sideLerpAmount),
                sideLerpAmount,
                "Must be between 0 and 1, inclusive."
            );
        }

        var topLeft = side switch {
            Side.Top => new Vector2(
                sidePosition.X - (size.X * sideLerpAmount),
                sidePosition.Y
            ),
            Side.Bottom => new Vector2(
                sidePosition.X - (size.X * sideLerpAmount),
                sidePosition.Y - size.Y
            ),
            Side.Left => new Vector2(
                sidePosition.X,
                sidePosition.Y - (size.Y * sideLerpAmount)
            ),
            Side.Right => new Vector2(
                sidePosition.X - size.X,
                sidePosition.Y - (size.Y * sideLerpAmount)
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };

        return new Rect2(topLeft, size);
    }

    public static Vector2 Mirror(
        in this Vector2 vector2,
        Vector2.Axis    axisOfSymmetry
    ) {
        return axisOfSymmetry switch {
            Vector2.Axis.X => vector2 * new Vector2(-1, 1),
            Vector2.Axis.Y => vector2 * new Vector2(1,  -1),
            _              => throw new ArgumentOutOfRangeException(nameof(axisOfSymmetry), axisOfSymmetry, null)
        };
    }

    public static Rect2 Mirror(this Rect2 rect2, Vector2.Axis axisOfSymmetry) {
        return axisOfSymmetry switch {
            Vector2.Axis.X => Rect2ByCorners(
                rect2.Position * new Vector2(1, -1),
                rect2.End      * new Vector2(1, -1)
            ),
            Vector2.Axis.Y => Rect2ByCorners(
                rect2.Position * new Vector2(-1, 1),
                rect2.End      * new Vector2(-1, 1)
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(axisOfSymmetry), axisOfSymmetry, null)
        };
    }

    /// <summary>
    /// Constructs a <see cref="Rect2"/> by its <see cref="Corner.TopLeft"/> and <see cref="Corner.BottomRight"/> corners.
    /// </summary>
    /// <param name="topLeft">aka <see cref="Rect2.Position"/></param>
    /// <param name="bottomRight">aka <see cref="Rect2.End"/></param>
    /// <seealso cref="Rect2ByCornersUphill"/>
    public static Rect2 Rect2ByCorners(
        in Vector2 topLeft,
        in Vector2 bottomRight
    ) {
        return new Rect2(
            topLeft,
            bottomRight - topLeft
        );
    }

    /// <summary>
    /// Constructs a <see cref="Rect2"/> with the given <see cref="Corner.BottomLeft"/> and <see cref="Corner.TopRight"/> corners.
    /// </summary>
    /// <seealso cref="Rect2ByCorners"/>
    public static Rect2 Rect2ByCornersUphill(
        in Vector2 bottomLeft,
        in Vector2 topRight
    ) {
        var size = new Vector2(
            topRight.X   - bottomLeft.X,
            bottomLeft.Y - topRight.Y
        );

        var position = new Vector2(
            bottomLeft.X,
            topRight.Y
        );

        return new Rect2(position, size);
    }

    public static float    Width(this Rect2        rect2) => rect2.Size.X;
    public static Distance Width(this RectDistance rect)  => rect.Size.X;

    public static float    Height(this Rect2        rect2) => rect2.Size.Y;
    public static Distance Height(this RectDistance rect)  => rect.Size.Y;

    public static Vector2 GetCorner(this Rect2 rect2, Corner corner) {
        return corner switch {
            Corner.TopLeft     => rect2.Position,
            Corner.TopRight    => rect2.Position + new Vector2(rect2.Size.X, 0),
            Corner.BottomRight => rect2.End,
            Corner.BottomLeft  => rect2.Position + new Vector2(0, rect2.Size.Y),
            _                  => throw new ArgumentOutOfRangeException(nameof(corner), corner, null)
        };
    }

    /// <summary>
    /// Imagine that you clicked and dragged <paramref name="corner"/> to <see cref="cornerDestination"/>.
    /// <p/>
    /// The "inverse" of this operation - i.e. defining the <see cref="Corner"/> that you <b><i>DON'T</i></b> want to change, instead of the one that you <i>do</i> - is <see cref="WithSizeFromCorner"/>.
    /// </summary>
    /// <param name="rect2"></param>
    /// <param name="corner"></param>
    /// <param name="cornerDestination"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Rect2 DragCorner(
        in this Rect2 rect2,
        Corner        corner,
        in Vector2    cornerDestination
    ) {
        return corner switch {
            Corner.TopLeft     => Rect2ByCorners(cornerDestination, rect2.End),
            Corner.TopRight    => Rect2ByCornersUphill(rect2.GetCorner(Corner.BottomLeft), cornerDestination),
            Corner.BottomRight => Rect2ByCorners(rect2.Position, cornerDestination),
            Corner.BottomLeft  => Rect2ByCornersUphill(cornerDestination, rect2.GetCorner(Corner.TopRight)),
            _                  => throw new ArgumentOutOfRangeException(nameof(corner), corner, null)
        };
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="rect2"></param>
    /// <param name="newSize"></param>
    /// <param name="keeping"></param>
    /// <returns></returns>
    public static Rect2 WithSizeFromCorner(in this Rect2 rect2, in Vector2 newSize, Corner keeping) {
        return Rect2ByCorner(
            keeping,
            rect2.GetCorner(keeping),
            newSize
        );
    }

    /// <summary>
    /// <see cref="Vector2.op_Multiply(Godot.Vector2,float)">Multiplies</see> my <see cref="Rect2.Size"/> while maintaining one of my <see cref="GetCorner"/>s.
    /// </summary>
    public static Rect2 ResizeFromCorner(in this Rect2 rect2, Vector2 multiplier, Corner keeping) {
        return rect2.WithSizeFromCorner(rect2.Size * multiplier, keeping);
    }

    /// <summary>
    /// <see cref="Vector2.op_Multiply(Godot.Vector2,float)">Multiplies</see> my <see cref="Rect2.Size"/> while maintaining one of my <see cref="GetCorner"/>s.
    /// </summary>
    public static Rect2 ResizeFromCorner(in this Rect2 rect2, float multiplier, Corner keeping) {
        return rect2.WithSizeFromCorner(rect2.Size * multiplier, keeping);
    }

    /// <summary>
    /// <see cref="Vector2.op_Multiply(Godot.Vector2,float)">Multiplies</see> my <see cref="Rect2.Size"/> while maintaining my <see cref="Rect2.GetCenter"/>.
    /// </summary>
    /// <seealso cref="ResizeFromCorner"/>
    /// <seealso cref="WithSizeFromCorner"/>
    public static Rect2 ResizeFromCenter(in this Rect2 rect2, float multiplier) {
        return Rect2ByCenter(rect2.GetCenter(), rect2.Size * multiplier);
    }

    /// <summary>
    /// <see cref="Vector2.op_Multiply(Godot.Vector2,float)">Multiplies</see> my <see cref="Rect2.Size"/> while maintaining my <see cref="Rect2.GetCenter"/>.
    /// </summary>
    /// <seealso cref="ResizeFromCorner"/>
    /// <seealso cref="WithSizeFromCorner"/>
    public static Rect2 ResizeFromCenter(in this Rect2 rect2, Vector2 multiplier) {
        return Rect2ByCenter(rect2.GetCenter(), rect2.Size * multiplier);
    }

    public static Rect2 Scale(in this Rect2 rect2, float multiplier) {
        return new Rect2(
            rect2.Position * multiplier,
            rect2.Size     * multiplier
        );
    }

    public static IEnumerable<Rect2> EnumerateGridCells(in this Rect2 rect, Vector2 gridSize) {
        throw new NotImplementedException();
    }

    public static Distance GetProjectScreenWidth() =>
        ProjectSettings.GetSetting("display/window/size/viewport_width").AsSingle().GodotPixels;

    public static Distance GetProjectScreenHeight() =>
        ProjectSettings.GetSetting("display/window/size/viewport_height").AsSingle().GodotPixels;

    public static Distance2D GetProjectScreenSize() => new(GetProjectScreenWidth(), GetProjectScreenHeight());
}