using System;
using System.Linq;
using Godot;

namespace maidoc.Scenes;

public static class LazyChild {

}

public sealed class LazyChild<T>  where T : Node {
    private T?            _child;
    private readonly Func<Node, T> _truancyOfficer;

    public LazyChild(Func<Node, T> truancyOfficer) {
        _truancyOfficer = truancyOfficer;
    }

    public LazyChild(string uniqueName) : this(parent => parent.EnumerateChildren()
        .OfType<T>()
        .Single(it => it.Name == uniqueName)
    ) {

    }

    public static LazyChild<T> ByName(string uniqueName) {
        return new LazyChild<T>(parent => parent.EnumerateChildren()
            .OfType<T>()
            .Single(it => it.Name == uniqueName));
    }

    public T Get(Node parent) {
        return _child ??= _truancyOfficer(parent);
    }
}