using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace maidoc.Scenes;

[StackTraceHidden]
public sealed class Disenfranchised<T> {
    [MemberNotNullWhen(true, nameof(_value))]
    private bool Enfranchised { get; set; }

    private T? _value;

    public void Enfranchise(
        Func<T> valueFactory
    ) {
        if (Enfranchised) {
            throw new InvalidOperationException($"Cannot enfranchise because I am already: {_value}");
        }

        _value = valueFactory();
        GD.Print($"Enfranchised to: {_value}");
        Enfranchised = true;
    }

    public void Enfranchise(T value) {
        Enfranchise(() => value);
    }

    public T Value => Enfranchised
        ? _value
        : throw new InvalidOperationException($"I have not been enfranchised with a {typeof(T)} value yet!");
}