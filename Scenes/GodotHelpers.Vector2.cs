using System;
using Godot;

namespace maidoc.Scenes;

public partial class GodotHelpers {
    public static float GetAxis(in this Vector2 vector2, Vector2.Axis axis) => axis switch {
        Vector2.Axis.X => vector2.X,
        Vector2.Axis.Y => vector2.Y,
        _              => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
    };

    /// <returns>(<see cref="Vector2.Y">Y</see>, <see cref="Vector2.X">X</see>)</returns>
    public static Vector2 Reversed(in this Vector2 vector2) => new(vector2.Y, vector2.X);

    /// <returns>(<see cref="MinAxisValue"/>, <see cref="MaxAxisValue"/>)</returns>
    public static Vector2 Sorted(in this Vector2 vector2) {
        return vector2.Y < vector2.X ? vector2.Reversed() : vector2;
    }

    /// <returns>The value of my <see cref="Vector2.MaxAxisIndex"/>.</returns>
    public static float MaxAxisValue(in this Vector2 vector2) => vector2.GetAxis(vector2.MaxAxisIndex());

    /// <returns>The value of my <see cref="Vector2.MinAxisIndex"/>.</returns>
    public static float MinAxisValue(in this Vector2 vector2) => vector2.GetAxis(vector2.MinAxisIndex());

    public static Vector2 WithAxis(in this Vector2 vector2, Vector2.Axis axis, float newValue) => axis switch {
        Vector2.Axis.X => vector2 with { X = newValue },
        Vector2.Axis.Y => vector2 with { Y = newValue },
        _              => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
    };

    public static Vector2 MultiplyAxis(in this Vector2 vector2, Vector2.Axis axis, float multiplier) =>
        vector2 * Vector2.One.WithAxis(axis, multiplier);

    /// <returns>A new <see cref="Vector2"/> with the same <see cref="Vector2.Aspect"/> as me, but whose <see cref="MaxAxisValue"/> is 1.</returns>
    /// <example>
    /// <code><![CDATA[
    /// (8, 4) => ( 1, .5)
    /// (2, 5) => (.4,  1)
    /// ]]></code>
    /// </example>
    public static Vector2 NormalizeLargerAxis(in this Vector2 vector2) {
        var smallerAxis           = vector2.MinAxisIndex();
        var smallerAxisMultiplier = vector2.Sorted().Aspect();
        return Vector2.One.WithAxis(smallerAxis, smallerAxisMultiplier);
    }
}