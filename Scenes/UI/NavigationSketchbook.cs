using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Godot;
using maidoc.Scenes.Navigation;

namespace maidoc.Scenes.UI;

public partial class NavigationSketchbook : Control {
    [Export]
    public Control? InitialFocus { get; set; }

    public override void _Ready() {
        var focus = InitialFocus ?? FindNextValidFocus();
        focus.GrabFocus();

        focus.blog(
            focus.FocusNeighborLeft
        );

        focus.FocusNeighborLeft = null;

        focus.FocusNeighborLeft.blog(label: "after setting to null");

        focus.FocusNeighborLeft = new NodePath();
        focus.FocusNeighborLeft.blog(label: "after setting to new NodePath()");

        focus.FocusNeighborLeft = focus.GetPath();
        focus.FocusNeighborLeft.blog(label: "After setting to this.GetPath()");

        // RefreshFocusNavigation();

        var allKids = focus.GetParent()
                           .EnumerateChildren()
                           .OfType<Control>()
                           .ToImmutableArray();

        var first = allKids[0];

        var second = allKids[1];

        this.blog(first, second);

        GodotHelpers.ConfigureLinearNavigation(
            new FocusWrapper(first),
            Vector2.Axis.X,
            new(first),
            new(second)
        );

        GD.Print($"after all is said and done:\n{first.FocusWrapper().DescribeFocus()}");

        Debug.Assert(
            first is {
                FocusNeighborLeft.IsEmpty  : false,
                FocusNeighborRight.IsEmpty : false,
                FocusNeighborTop.IsEmpty   : false,
                FocusNeighborBottom.IsEmpty: false,
            }
        );
    }

    [Export]
    private GodotHelpers.BoundaryNavigation BoundaryNavigation { get; set; }

    public void RefreshFocusNavigation() {
        var children = this.EnumerateChildren(1)
                           .OfType<Control>()
                           .Where(it => it.Visible)
                           .ToImmutableArray();

        for (int i = 0; i < children.Length; i++) {
            var neighbors = i.GetNeighbors(children.Length, BoundaryNavigation);

            var current = children[i];
            this.blog(
                current,
                current.FocusWrapper(),
                neighbors
            );
            GodotHelpers.ConfigureLinearNavigation(
                current.FocusWrapper(),
                Vector2.Axis.X,
                children[neighbors.previous].FocusWrapper(),
                children[neighbors.next].FocusWrapper()
            );
        }
    }
}