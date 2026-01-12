using Godot;

namespace maidoc.Scenes;

public static partial class GodotHelpers {
    public static TSceneRoot SpawnChild<TSceneRoot, TInput>(
        this Node parent,
        TInput    input
    ) where TSceneRoot : Node, ISceneRoot<TSceneRoot, TInput> {
        return TSceneRoot.InstantiateRawScene()
            .AsChildOf(parent)
            .InitializeSelf(input);
    }

    public static T Named<T>(
        this T     node,
        StringName name
    ) where T : Node {
        node.Name = name;
        return node;
    }

    public static T AsChildOf<T>(
        this T            self,
        Node              parent,
        bool              forceReadableName = false,
        Node.InternalMode internalMode      = Node.InternalMode.Disabled
    ) where T : Node {
        parent.AddChild(self, forceReadableName, internalMode);
        return self;
    }

    public static T AsSiblingOf<T>(
        this T self,
        Node   sibling,
        bool   forceReadableName = false
    ) where T : Node {
        sibling.AddSibling(self, forceReadableName);
        return self;
    }
}