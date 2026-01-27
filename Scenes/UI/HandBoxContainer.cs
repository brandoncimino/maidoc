using System;
using System.Collections.Immutable;
using System.Linq;
using Godot;
using maidoc.Core;

namespace maidoc.Scenes.UI;

/// <summary>
/// A variation on <see cref="HBoxContainer"/> that, if it contains contents that exceed <see cref="HandBoxContainer.CustomMinimumSize"/>,
/// "squishes" its children together so that they overlap left to right - the way you would fan out physical cards in your hand.
/// </summary>
/// <remarks>
/// <ul>
/// <li>This operates under the assumption that each child has the same size.</li>
/// <li>We can't control this container's <see cref="Control.Size"/>, so we have to position it based solely on its <see cref="Control.Position"/> and <see cref="Control.CustomMinimumSize"/>.</li>
/// </ul>
/// </remarks>
[Tool]
public partial class HandBoxContainer : HBoxContainer {
    /// <summary>
    /// Which of my child <see cref="Control.Size"/>s is assumed to be the size of each of my children.
    /// </summary>
    [Export]
    public ReferenceChild SpaceAccordingTo { get; set; } = ReferenceChild.First;

    [Export]
    public bool FanItems { get; set; } = true;

    [Export]
    public float MaxFanAngleDegrees { get; set; } = 10f;

    public enum ReferenceChild {
        First,
        Largest
    }

    public override void _Notification(int what) {
        if (what == NotificationSortChildren) {
            SquishChildren();
        }
    }

    private void SquishChildren() {
        var widthLimit = CustomMinimumSize.X;
        var children = this.EnumerateChildren(1)
                           .OfType<Control>()
                           .Where(it => it.Visible)
                           .ToImmutableArray();

        if (children.IsEmpty) {
            return;
        }

        var itemWidth = SpaceAccordingTo switch {
            ReferenceChild.First   => children.First().Size.X,
            ReferenceChild.Largest => children.Max(it => it.Size.X),
            _                      => throw new ArgumentOutOfRangeException()
        };

        if (itemWidth * children.Length <= widthLimit) {
            return;
        }

        var maxStart = widthLimit - itemWidth;

        var startInterval = maxStart / (children.Length - 1);

        var minChildCenter = itemWidth / 2;
        var maxChildCenter = widthLimit - (itemWidth / 2);

        for (int i = 0; i < children.Length; i++) {
            var child = children[i];
            var xNew  = startInterval * i;
            child.Position = child.Position with {
                X = xNew
            };

            if (FanItems) {
                var childCenter = xNew + itemWidth / 2;
                var maxFan      = Math.Abs(MaxFanAngleDegrees);
                var childAngle = Helpers.LerpProportional(
                    (minChildCenter, maxChildCenter, childCenter),
                    (-maxFan, maxFan)
                );

                child.PivotOffset     = child.Size / 2;
                child.RotationDegrees = childAngle;
            }
        }

        // TODO: Setting this container's size doesn't actually work, which means that even though we've adjusted the children, this container will still report the un-adjusted size of the children to its parent.
        //       That means that the parent will treat this container as bigger than it visually is.
        //       It is tempting to override `_GetMinimumSize`, but that is - as per the documentation - specifically bypassed for built-in container types like `HBoxContainer`.
        //       Right now, I don't actually NEED to worry about the reported minimum size, because the hand should be a top-level element.
        //       If I wind up needing a more "correct" behavior, then the solution would probably be something like:
        //         - Extract this behavior into a direct `Control` descendant
        //         - Re-parent the children to either my custom `Control` or an `HBoxContainer`
        //       However, this feels super gross.
    }

    [ExportToolButton(nameof(SquishChildren))]
    public Callable ArrangeChildrenButton => Callable.From(SquishChildren);
}