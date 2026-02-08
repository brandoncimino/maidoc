using Godot;

namespace maidoc.Scenes;

public static partial class GodotHelpers {
    public static TSceneRoot SpawnChild<TSceneRoot, TInput>(
        this Node parent,
        TInput    input
    ) where TSceneRoot : Node, ISceneRoot<TSceneRoot, TInput> {
        // 📎 `_Ready` will be called as soon as the `Node` has a parent.
        //    Therefore, it is vital that `InitializeSelf` is called *FIRST*.
        return TSceneRoot.InstantiateRawScene()
                         .InitializeSelf(input)
                         .AsChildOf(parent);
    }

    public static T Named<T>(
        this T     node,
        StringName name
    ) where T : Node {
        node.Name = name;
        return node;
    }

    public static T AtPosition<T>(
        this T     gameNode2D,
        Distance2D position
    ) where T : IGameNode2D {
        gameNode2D.Center = position;
        return gameNode2D;
    }

    public static T AtPosition<T>(
        this T  node,
        Vector2 position
    ) where T : Node2D {
        node.Position = position;
        return node;
    }

    public static T AsChildOf<T>(
        this T            self,
        Node              parent,
        bool keepGlobalTransform = true
    ) where T : Node {
        // 🙋‍♀️ Is there any particular reason to use `Reparent` instead?
        // ✅ YES! It lets you maintain the object's global transform, rather than "moving" the object to be relative to its new parent.
        if (self.GetParent() != null) {
            self.Reparent(parent, keepGlobalTransform);
        }
        else {
            parent.AddChild(self);
        }
        
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

    public static T WithChild<T>(
        this T            self,
        Node              child,
        bool              forceReadableName = false,
        Node.InternalMode internalMode      = Node.InternalMode.Disabled
    ) where T : Node {
        self.AddChild(child, forceReadableName, internalMode);
        return self;
    }
}