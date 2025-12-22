using System;
using Godot;

namespace maidoc.Scenes;

public static partial class GodotHelpers {
    public static TSceneRoot Instantiate<TSceneRoot, TInput>(
        PackedScene packedScene,
        TInput      input,
        Node?       parent = null
    ) where TSceneRoot : Node, ISceneRoot<TSceneRoot, TInput> {
        var child = packedScene.Instantiate<TSceneRoot>();
        parent?.AddChild(child);
        child.InitializeSelf(input);
        return child;
    }

    public static T SpawnChild<T, TInput>(
        this Node   parent,
        PackedScene packedScene,
        TInput      input
    ) where T : Node, ISceneRoot<T, TInput> {
        return Instantiate<T, TInput>(packedScene, input, parent);
    }

    public static T AsChildOf<T>(
        this T self,
        Node        parent,
        bool forceReadableName = false,
        Node.InternalMode internalMode = Node.InternalMode.Disabled
    ) where T : Node {
        parent.AddChild(self, forceReadableName, internalMode);
        return self;
    }

    public static T AsSiblingOf<T>(
        this T self,
        Node   sibling,
        bool forceReadableName = false
    ) where T : Node {
        sibling.AddSibling(self, forceReadableName);
        return self;
    }

    public static TScene Initialize<TScene, TInput>(
        this TScene sceneRoot,
        TInput      input
        ) where TScene : Node, ISceneRoot<TScene, TInput> {
        sceneRoot.InitializeSelf(input);
        GD.Print($"Initialized: {sceneRoot}");
        return sceneRoot;
    }
}