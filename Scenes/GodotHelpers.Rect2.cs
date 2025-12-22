using System;
using Godot;

namespace maidoc.Scenes;

public static partial class GodotHelpers {
    public static Vector2.Axis Axis(this Side side) => side switch {
        Side.Left or Side.Right => Vector2.Axis.X,
        Side.Top or Side.Bottom => Vector2.Axis.Y,
        _                       => throw new ArgumentOutOfRangeException(nameof(side), side, null)
    };

    public static Corner Inverse(this Corner corner) => corner switch {
        Corner.TopLeft     => Corner.BottomRight,
        Corner.TopRight    => Corner.BottomLeft,
        Corner.BottomRight => Corner.TopLeft,
        Corner.BottomLeft  => Corner.TopRight,
        _                  => throw new ArgumentOutOfRangeException(nameof(corner), corner, null)
    };

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

    public static float Width(this  Rect2 rect2) => rect2.Size.X;

    public static float Height(this Rect2 rect2) => rect2.Size.Y;

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
    /// Imagine that you clicked and dragged <paramref name="corner"/> to <see cref="cornerPosition"/>.
    /// <p/>
    /// The "inverse" of this operation - i.e. defining the <see cref="Corner"/> that you <b><i>DON'T</i></b> want to change, instead of the one that you <i>do</i> - is <see cref="Resize"/>.
    /// </summary>
    /// <param name="rect2"></param>
    /// <param name="corner"></param>
    /// <param name="cornerPosition"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static Rect2 DragCorner(
        in this Rect2 rect2,
        Corner        corner,
        in Vector2    cornerPosition
    ) {
        return corner switch {
            Corner.TopLeft     => Rect2ByCorners(cornerPosition, rect2.End),
            Corner.TopRight    => Rect2ByCornersUphill(rect2.GetCorner(Corner.BottomLeft), cornerPosition),
            Corner.BottomRight => Rect2ByCorners(rect2.Position, cornerPosition),
            Corner.BottomLeft  => Rect2ByCornersUphill(cornerPosition, rect2.GetCorner(Corner.TopRight)),
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
    public static Rect2 Resize(in this Rect2 rect2, in Vector2 newSize, Corner keeping) {
        return Rect2ByCorner(
            keeping,
            rect2.GetCorner(keeping),
            newSize
        );
    }

    /// <summary>
    /// <see cref="Vector2.op_Multiply(Godot.Vector2,float)">Multiplies</see> my <see cref="Rect2.Size"/> while maintaining one of my <see cref="GetCorner"/>s.
    /// </summary>
    public static Rect2 Resize(in this Rect2 rect2, float multiplier, Corner keeping) {
        return rect2.Resize(rect2.Size * multiplier, keeping);
    }

    /// <summary>
    /// <see cref="Vector2.op_Multiply(Godot.Vector2,float)">Multiplies</see> my <see cref="Rect2.Size"/> while maintaining my <see cref="Rect2.GetCenter"/>.
    /// </summary>
    /// <seealso cref="Resize(in Rect2,float,Corner)"/>
    /// <seealso cref="Resize(in Rect2,in Vector2,Corner)"/>
    public static Rect2 Resize(in this Rect2 rect2, float multiplier) {
        return Rect2ByCenter(rect2.GetCenter(), rect2.Size * multiplier);
    }

    public static Rect2 Scale(in this Rect2 rect2, float multiplier) {
        return new Rect2(
            rect2.Position * multiplier,
            rect2.Size     * multiplier
        );
    }
}