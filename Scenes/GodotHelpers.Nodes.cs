using System.Collections.Generic;
using System.Linq;
using Godot;

namespace maidoc.Scenes;

public partial class GodotHelpers {
    #region Finding Nodes

    public static IEnumerable<Node> EnumerateChildren(
        this Node self,
        int       depth           = int.MaxValue,
        bool      includeInternal = false
    ) {
        for (int i = 0; i < self.GetChildCount(includeInternal); i++) {
            var child = self.GetChild(i);
            yield return child;

            if (depth > 1) {
                foreach (var grandChild in
                         child.EnumerateChildren(depth: depth - 1, includeInternal: includeInternal)) {
                    yield return grandChild;
                }
            }
        }
    }

    public static T RequireOnlyChild<T>(this Node self, int depth = int.MaxValue, bool includeInternal = false)
        where T : Node {
        return self.EnumerateChildren(depth, includeInternal)
                   .OfType<T>()
                   .Single();
    }

    public static IEnumerable<Node> EnumerateSiblings(this Node self, bool includeInternal = false) {
        return self.GetParent()?.EnumerateChildren(depth: 1, includeInternal: includeInternal) ?? [];
    }

    public static IEnumerable<Node> EnumerateAncestors(this Node node) {
        var currentChild = node;

        while (currentChild.GetParent() is { } parent) {
            yield return parent;
            currentChild = parent;
        }
    }

    #endregion
}