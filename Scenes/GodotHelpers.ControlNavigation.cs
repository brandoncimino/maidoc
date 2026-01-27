using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Godot;
using maidoc.Scenes.Navigation;
using static maidoc.Scenes.GodotHelpers.BoundaryNavigation;
using Side = Godot.Side;

namespace maidoc.Scenes;

public static partial class GodotHelpers {
    /// <summary>
    /// What to do when we try to navigate outside of a defined scope, e.g. past the last item in a list.
    /// </summary>
    public enum BoundaryNavigation {
        /// <summary>
        /// Do not allow navigation outside of the boundary.
        /// </summary>
        None,

        /// <summary>
        /// Navigate back to the first item in the list.
        /// </summary>
        Loop
    }

    public static (int previous, int next) GetNeighbors(
        this int           current,
        int                length,
        BoundaryNavigation boundaryNavigation
    ) => (
        current.PreviousIndex(length, boundaryNavigation),
        current.NextIndex(length, boundaryNavigation)
    );

    public static int NextIndex(this int current, int length, BoundaryNavigation boundaryNavigation) {
        if (current < length - 1) {
            return current + 1;
        }

        return boundaryNavigation switch {
            None => current,
            Loop => 0,
            _    => throw new ArgumentOutOfRangeException(nameof(boundaryNavigation), boundaryNavigation, null)
        };
    }

    public static int PreviousIndex(this int current, int length, BoundaryNavigation boundaryNavigation) {
        if (current > 0) {
            return current - 1;
        }

        return boundaryNavigation switch {
            None => current,
            Loop => length - 1,
            _    => throw new ArgumentOutOfRangeException(nameof(boundaryNavigation), boundaryNavigation, null)
        };
    }

    public static void ConfigureLinearNavigation(
        this IEnumerable<FocusWrapper> stuff,
        Vector2.Axis                   axis,
        BoundaryNavigation             boundaryNavigation
    ) {
        var controls = stuff.ToImmutableArray();

        if (controls.IsEmpty) {
            return;
        }

        for (var i = 0; i < controls.Length; i++) {
            ConfigureLinearNavigation(controls, i, axis, boundaryNavigation);
        }
    }

    public static void ConfigureLinearNavigation(
        ImmutableArray<FocusWrapper> allItems,
        int                          currentIndex,
        Vector2.Axis                 axis,
        BoundaryNavigation           boundaryNavigation
    ) {
        var current  = allItems[currentIndex];
        var previous = allItems[currentIndex.PreviousIndex(allItems.Length, boundaryNavigation)];
        var next     = allItems[currentIndex.NextIndex(allItems.Length, boundaryNavigation)];

        ConfigureLinearNavigation(current, axis, previous, next);
    }

    public static void ConfigureLinearNavigation(
        FocusWrapper current,
        Vector2.Axis axis,
        FocusWrapper previous,
        FocusWrapper next
    ) {
        var (previousSide, nextSide) = axis.GetNavigationSides();

        current.SetFocusNeighbor(previousSide, previous.GetPath());
        current.FocusPrevious = previous.GetPath();

        current.SetFocusNeighbor(nextSide, next.GetPath());
        current.FocusNext = next.GetPath();

        var disabledSides = axis.Other().GetNavigationSides();
        current.SetFocusNeighbor(disabledSides.previous, current.GetPath());
        current.SetFocusNeighbor(disabledSides.next,     current.GetPath());
    }

    private static (Side previous, Side next) GetNavigationSides(this Vector2.Axis axis) => axis switch {
        Vector2.Axis.X => (Side.Left, Side.Right),
        Vector2.Axis.Y => (Side.Top, Side.Bottom),
        _              => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
    };

    public static FocusWrapper FocusWrapper(this Control control) => new(control);
}