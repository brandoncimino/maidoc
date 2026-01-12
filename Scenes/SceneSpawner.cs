using System;
using System.Collections.Immutable;
using Godot;

namespace maidoc.Scenes;

/// <summary>
/// Instantiated <see cref="TSceneRoot"/>s are tracked in the following places:
/// <ul>
/// <li>As members of the <see cref="GroupName"/> via <see cref="Node.AddToGroup">group</see>.</li>
/// <li>As elements of <see cref="Instances"/>.</li>
/// </ul>
/// </summary>
/// <typeparam name="TSceneRoot">The type of the root-level <see cref="Node"/> in the <see cref="PackedScene"/>.</typeparam>
/// <typeparam name="TInput">The stuff needed to <see cref="ISceneRoot{TSelf,TInput}.InitializeSelf"/>.</typeparam>
public class SceneSpawner<TSceneRoot, TInput> where TSceneRoot : Node, ISceneRoot<TSceneRoot, TInput>, new() {
    public readonly Disenfranchised<Node> GroupNode = new();

    /// <summary>
    /// The Godot "<see cref="Node.AddToGroup">group</see>" that my <see cref="Instances"/> are added to.
    /// </summary>
    /// <remarks>
    /// This defaults to the name of <typeparamref name="TSceneRoot"/>, meaning that it is <b>NOT</b> unique to this <see cref="SceneSpawner{TSceneRoot,TInput}"/> instance.
    /// </remarks>
    public StringName GroupName { get; init; } = typeof(TSceneRoot).Name;

    /// <summary>
    /// Generates the <see cref="Node.Name"/> based on the instance's <typeparamref name="TInput"/> and <b>index</b> within <see cref="Instances"/> (i.e. the first <see cref="Spawn"/> will have the index <c>0</c>).
    /// <br/>
    /// <br/>
    /// Defaults to "{<typeparamref name="TSceneRoot"/>}_{index}".
    /// <br/>
    /// <br/>
    /// If set to <c>null</c>, the default Godot-generated name will be used.
    /// <br/>
    /// </summary>
    public Func<TInput, int, string>? NamingConvention { get; init; } =
        (_, index) => $"{typeof(TSceneRoot).Name}_{index}";

    /// <summary>
    /// Am <b>immutable snapshot</b> of everything I've <see cref="Spawn"/>ed.
    /// </summary>
    public ImmutableArray<TSceneRoot> Instances { get; private set; } = ImmutableArray<TSceneRoot>.Empty;

    public TSceneRoot Spawn(TInput input) {
        var instance = GroupNode.Value.SpawnChild<TSceneRoot, TInput>(input);

        if (NamingConvention is not null) {
            instance.Name = NamingConvention(input, Instances.Length);
        }

        instance.AddToGroup(GroupName);
        Instances = Instances.Add(instance);

        return instance;
    }

    public SceneSpawner<TSceneRoot, TInput> UseGroupNode(Node groupNodeParent) {
        GroupNode.TryEnfranchise(() =>
            new Node()
                .Named(GroupName)
                .AsChildOf(groupNodeParent)
        );

        return this;
    }
}