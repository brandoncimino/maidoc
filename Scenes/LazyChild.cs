using System;
using System.Linq;
using Godot;

namespace maidoc.Scenes;

/// <param name="truancyOfficer">Retrieves a delinquent <see cref="_child"/>.</param>
/// <typeparam name="T">The type of <see cref="_child"/>.</typeparam>
public sealed class LazyChild<T>(Func<Node, T> truancyOfficer)
    where T : Node {
    private T? _child;

    public LazyChild(string uniqueName) : this(parent => parent.EnumerateChildren()
        .OfType<T>()
        .Single(it => it.Name == uniqueName)
    ) { }

    public static LazyChild<T> ByName(string uniqueName) {
        return new LazyChild<T>(parent => parent.EnumerateChildren()
            .OfType<T>()
            .Single(it => it.Name == uniqueName)
        );
    }

    public T Get(Node parent) {
        return _child ??= truancyOfficer(parent);
    }
}