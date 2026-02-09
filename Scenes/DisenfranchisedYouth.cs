using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace maidoc.Scenes;

[StackTraceHidden]
public sealed class Disenfranchised<T> {
    [MemberNotNullWhen(true, nameof(_value), nameof(EnfranchisedBy))]
    private bool Enfranchised { get; set; }

    private string? EnfranchisedBy { get; set; }

    private T? _value;

    public bool TryEnfranchise(Func<T> valueFactory, [CallerMemberName] string _caller = "") {
        if (Enfranchised) {
            return false;
        }

        _value       = valueFactory();
        Enfranchised = true;
        return true;
    }

    public bool TryEnfranchise(T value, [CallerMemberName] string _caller = "") {
        return TryEnfranchise(() => value, _caller);
    }

    public void Enfranchise(
        Func<T>                   valueFactory,
        [CallerMemberName] string _caller = ""
    ) {
        if (TryEnfranchise(valueFactory, _caller) is false) {
            throw new InvalidOperationException(
                $"""
                 Cannot be enfranchised by {_caller} because I am already enfranchised!
                   {nameof(_value)}:         {_value}
                   {nameof(EnfranchisedBy)}: {EnfranchisedBy.OrNullPlaceholder()}
                 """
            );
        }
    }

    public void Enfranchise(T value, [CallerMemberName] string _caller = "") {
        Enfranchise(() => value, _caller);
    }

    public T Value => Enfranchised
        ? _value
        : throw new InvalidOperationException($"I have not been enfranchised with a {typeof(T)} value yet!");

    public static implicit operator T(Disenfranchised<T> disenfranchised) => disenfranchised.Value;
}