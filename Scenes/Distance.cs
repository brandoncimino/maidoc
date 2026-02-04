using System;
using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;

namespace maidoc.Scenes;

public readonly record struct Distance :
    IMultiplyOperators<Distance, float, Distance>,
    IMultiplyOperators<Distance, Vector2, Distance2D>,
    IAdditionOperators<Distance, Distance, Distance>,
    ISubtractionOperators<Distance, Distance, Distance>,
    IDivisionOperators<Distance, Distance, float>,
    IDivisionOperators<Distance, float, Distance>,
    IUnaryNegationOperators<Distance, Distance>,
    IComparable<Distance>,
    IComparisonOperators<Distance, Distance, bool> {
    public float Meters { get; init; }

    public float GodotPixels {
        get => Meters * GodotHelpers.GodotPixelsPerMeter;
        init => Meters = value / GodotHelpers.GodotPixelsPerMeter;
    }

    public enum Unit {
        Meters,
        GodotPixels,
        ScreenWidths,
        ScreenHeights,
        Screens
    }

    public override string ToString() => $"{Meters} m";

    #region Factories

    public static Distance Of(float amount, Unit unit) {
        return unit switch {
            Unit.Meters        => amount.Meters(),
            Unit.GodotPixels   => amount.GodotPixels(),
            Unit.ScreenWidths  => amount.ScreenWidths(),
            Unit.ScreenHeights => amount.ScreenHeights(),
            Unit.Screens => throw new NotSupportedException(
                $"A one-dimensional {nameof(Distance)} cannot be defined in {nameof(Unit.Screens)}. Please use {nameof(Unit.ScreenWidths)} or {nameof(Unit.ScreenHeights)} instead."
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

    public static Distance2D Of(float   x,      float y, Unit unit) => Distance2D.Of(x, y, unit);
    public static Distance2D Of(Vector2 amount, Unit  unit) => Distance2D.Of(amount, unit);

    #endregion

    #region Operators

    public static Distance operator +(Distance a, Distance b) => (a.Meters + b.Meters).Meters();
    public static Distance operator -(Distance a, Distance b) => (a.Meters - b.Meters).Meters();

    public static Distance operator *(Distance distance,   float multiplier) => (distance.Meters * multiplier).Meters();
    public static Distance operator *(float    multiplier, Distance distance) => distance * multiplier;

    public static Distance operator /(Distance distance, float    divisor)  => (distance.Meters / divisor).Meters();
    public static Distance operator /(float    divisor,  Distance distance) => distance / divisor;

    public static Distance2D operator *(Distance distance, Vector2  scale)    => (distance.Meters * scale).Meters();
    public static Distance2D operator *(Vector2  scale,    Distance distance) => distance * scale;

    public static float operator /(Distance left, Distance right) => left.Meters / right.Meters;

    public static Distance operator -(Distance value) => (-value.Meters).Meters();

    public int CompareTo(Distance other) => Meters.CompareTo(other.Meters);

    public static bool operator >(Distance  left, Distance right) => left.Meters > right.Meters;
    public static bool operator >=(Distance left, Distance right) => left.Meters >= right.Meters;

    public static bool operator <(Distance  left, Distance right) => left.Meters < right.Meters;
    public static bool operator <=(Distance left, Distance right) => left.Meters <= right.Meters;

    #endregion
}

public readonly record struct Distance2D(Distance X, Distance Y) :
    IMultiplyOperators<Distance2D, float, Distance2D>,
    IMultiplyOperators<Distance2D, Vector2, Distance2D>,
    IAdditionOperators<Distance2D, Distance2D, Distance2D>,
    ISubtractionOperators<Distance2D, Distance2D, Distance2D>,
    IDivisionOperators<Distance2D, Distance2D, Vector2>,
    IDivisionOperators<Distance2D, Vector2, Distance2D>,
    IUnaryNegationOperators<Distance2D, Distance2D> {
    public Vector2 Meters      => new(X.Meters, Y.Meters);
    public Vector2 GodotPixels => new(X.GodotPixels, Y.GodotPixels);

    public override string ToString() => $"{X.Meters} ⨉ {Y.Meters} m";

    #region Factories

    public static Distance2D Of(float x, float y, Distance.Unit unit) => Of(new Vector2(x, y), unit);

    public static Distance2D Of(Vector2 amount, Distance.Unit unit) {
        return unit switch {
            Distance.Unit.Meters        => amount.Meters(),
            Distance.Unit.GodotPixels   => amount.GodotPixels(),
            Distance.Unit.ScreenWidths  => amount.Screens(RectMarker.ReferenceAxis.X),
            Distance.Unit.ScreenHeights => amount.Screens(RectMarker.ReferenceAxis.Y),
            Distance.Unit.Screens       => amount.Screens(RectMarker.ReferenceAxis.Corresponding),
            _                           => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
    }

    #endregion

    #region Operators

    public static Distance2D operator *(Distance2D distance, float multiplier) =>
        (distance.Meters * multiplier).Meters();

    public static Distance2D operator /(Distance2D distance, float divisor) => (distance.Meters / divisor).Meters();

    public static Distance2D operator *(float multiplier, Distance2D distance) =>
        (distance.Meters * multiplier).Meters();


    public static Distance2D operator *(Distance2D distance, Vector2 multiplier) =>
        (distance.Meters * multiplier).Meters();

    public static Distance2D operator /(Distance2D distance, Vector2 divisor) => (distance.Meters / divisor).Meters();

    public static Distance2D operator *(Vector2 multiplier, Distance2D distance) =>
        (distance.Meters * multiplier).Meters();


    public static Distance2D operator +(Distance2D left, Distance2D right) => new(left.X + right.X, left.Y + right.Y);
    public static Distance2D operator -(Distance2D left, Distance2D right) => new(left.X - right.X, left.Y - right.Y);

    public static Vector2 operator /(Distance2D    left, Distance2D right) => left.Meters / right.Meters;
    public static Distance2D operator -(Distance2D value) => (-value.Meters).Meters();

    #endregion
}

public readonly record struct RectDistance(
    Distance2D Position,
    Distance2D Size
) {
    public Rect2 Meters      => new(Position.Meters, Size.Meters);
    public Rect2 GodotPixels => new(Position.GodotPixels, Size.GodotPixels);

    public Distance2D GetCorner(Corner corner) => Meters.GetCorner(corner).Meters();
    public Distance   GetSide(Side     side)   => Meters.GetSide(side).Meters();
    public Distance2D GetCenter()              => Meters.GetCenter().Meters();

    private string Icon => Size.X.CompareTo(Size.Y) switch {
        < 0 => "▯",
        0   => "▢",
        > 0 => "▭"
    };

    public override string ToString() {
        return $"{Icon} {Position}, {Size}";
    }
}

public static class DistanceExtensions {
    public static Distance Meters(this      float meters)      => new() { Meters      = meters };
    public static Distance Meters(this      int   meters)      => new() { Meters      = meters };
    public static Distance GodotPixels(this float godotPixels) => new() { GodotPixels = godotPixels };

    public static Distance ScreenWidths(this float screenWidths) => GodotHelpers.GetProjectScreenWidth() * screenWidths;

    public static Distance ScreenHeights(this float screenHeights) =>
        GodotHelpers.GetProjectScreenHeight() * screenHeights;

    public static Distance2D Meters(this Vector2 meters) => new(meters.X.Meters(), meters.Y.Meters());

    public static Distance2D GodotPixels(this Vector2 godotPixels) => new(
        godotPixels.X.GodotPixels(),
        godotPixels.Y.GodotPixels()
    );

    public static Distance2D Screens(
        this in Vector2          screens,
        RectMarker.ReferenceAxis referenceAxis = RectMarker.ReferenceAxis.Corresponding
    ) {
        var reference = GodotHelpers.GetProjectScreenSize().GodotPixels;

        return (referenceAxis switch {
            RectMarker.ReferenceAxis.Corresponding => screens * reference,
            RectMarker.ReferenceAxis.X => screens * reference.X,
            RectMarker.ReferenceAxis.Y => screens * reference.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(referenceAxis), referenceAxis, null)
        }).GodotPixels();
    }

    public static RectDistance Meters(this Rect2 meters) => new(meters.Position.Meters(), meters.Size.Meters());

    public static RectDistance GodotPixels(this Rect2 godotPixels) => new(
        godotPixels.Position.GodotPixels(),
        godotPixels.Size.GodotPixels()
    );
}