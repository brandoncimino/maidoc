using System;
using System.Linq;
using Godot;

namespace maidoc.Scenes;

/// <param name="truancyOfficer">Retrieves a delinquent <see cref="_child"/>.</param>
/// <typeparam name="T">The type of <see cref="_child"/>.</typeparam>
public sealed class LazyChild<T>(Func<Node, T> truancyOfficer)
    where T : Node {
    private T? _child;

    /// <summary>
    /// Assumes that there will be <b>exactly one</b> child - at <b>any depth</b> - of type <typeparamref name="T"/>.
    /// </summary>
    public LazyChild() : this(parent => parent.EnumerateChildren()
        .OfType<T>()
        .Single()
    ) { }

    /// <summary>
    /// Assumes that there will be <b>exactly one</b> child - at <b>any depth</b> - with the given <see cref="Node.Name"/>.
    /// </summary>
    /// <param name="uniqueName"></param>
    public LazyChild(StringName uniqueName) : this(parent => parent.EnumerateChildren()
        .OfType<T>()
        .Single(it => it.Name == uniqueName)
    ) { }

    public T Get(Node parent) {
        return _child ??= truancyOfficer(parent);
    }
}