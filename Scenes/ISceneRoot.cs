using Godot;

namespace maidoc.Scenes;

public interface ISceneRoot<TSelf, in TInput> where TSelf : Node, ISceneRoot<TSelf, TInput> {
    TSelf InitializeSelf(TInput input);
}

// public interface ISceneRoot2<TSelf, in TInput>
// : ISceneRoot<TSelf, TInput>
//     where TSelf : Node, ISceneRoot2<TSelf, TInput> {
//     public static override virtual TSelf Instantiate(
//         PackedScene packedScene,
//         TInput      input,
//         Node?       parent       = null
//     ) {
//         var child = packedScene.Instantiate<TSelf>();
//         parent?.AddChild(child);
//         child.Initialize(input);
//         return child;
//     }
// }