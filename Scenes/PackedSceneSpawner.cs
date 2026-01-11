using System.Collections.Immutable;
using Godot;

namespace maidoc.Scenes;

/// <summary>
/// Instantiated <see cref="TSceneRoot"/>s are tracked in 3 places:
/// <ul>
/// <li>As children of <see cref="GroupNode"/>.</li>
/// <li>As members of the <see cref="GroupName"/> via <see cref="Node.AddToGroup">group</see>.</li>
/// <li>As elements of <see cref="Instances"/>.</li>
/// </ul>
/// </summary>
/// <typeparam name="TSceneRoot">The type of the root-level <see cref="Node"/> in the <see cref="PackedScene"/>.</typeparam>
/// <typeparam name="TInput">The stuff needed to <see cref="ISceneRoot{TSelf,TInput}.InitializeSelf"/>.</typeparam>
public class PackedSceneSpawner<TSceneRoot, TInput> where TSceneRoot : Node, ISceneRoot<TSceneRoot, TInput> {
    public required string PackedScenePath { get; init; }

    private PackedScene? _packedScene;

    public PackedScene PackedScene => _packedScene ??= ResourceLoader.Load<PackedScene>(PackedScenePath);

    private Node? _parentNode;

    /// <summary>
    /// The Godot "<see cref="Node.AddToGroup">group</see>" that my <see cref="Instances"/> are added to.
    /// </summary>
    /// <remarks>
    /// This defaults to the name of <typeparamref name="TSceneRoot"/>, meaning that it is <b>NOT</b> unique to this <see cref="PackedSceneSpawner{TSceneRoot,TInput}"/> instance.
    /// </remarks>
    public StringName GroupName { get; init; } = typeof(TSceneRoot).Name;

    /// <summary>
    /// A vanilla <see cref="Node"/> that all of my <see cref="Instances"/> will be <see cref="Node.AddChild">children of</see>.
    /// </summary>
    public Node GroupNode {
        get => _parentNode ??= new() {
            Name = GroupName
        };
        init => _parentNode = value;
    }

    /// <summary>
    /// Am <b>immutable snapshot</b> of everything I've <see cref="Spawn"/>ed.
    /// </summary>
    public ImmutableArray<TInput> Instances { get; private set; } = ImmutableArray<TInput>.Empty;

    public TSceneRoot Spawn(TInput input) {
        var instance = PackedScene.Instantiate<TSceneRoot>()
            .Initialize(input)
            .AsChildOf(GroupNode);

        instance.AddToGroup(GroupName);
        Instances = Instances.Add(input);
        return instance;
    }
}