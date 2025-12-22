using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using BSharp.Core;

namespace maidoc.Core;

public record Ruleset(
    int StartingHandSize = 4,
    int Columns          = 4,
    int RowsPerPlayer    = 2
) {
    public Ruleset Validate() {
        using var validator = new Validator();

        validator.Require(StartingHandSize, StartingHandSize > 0);
        validator.Require(Columns,          Columns          > 0);
        validator.Require(RowsPerPlayer,    RowsPerPlayer    > 0);

        return this;
    }
}

public readonly ref struct Validator() {
    private readonly List<string> _violations = [];

    public Validator Require<T>(
        T    value,
        bool condition,
        [CallerArgumentExpression(nameof(value))]
        string _value = "",
        [CallerArgumentExpression(nameof(condition))]
        string _condition = ""
    ) {
        if (!condition) {
            _violations.Add($"""
                            `{_value}` ({value}) must satisfy:
                                {_condition}
                            """);
        }

        return this;
    }

    public void Dispose() {
        switch (_violations) {
            case []:                    return;
            case [var singleViolation]: throw new InvalidOperationException(singleViolation);
            default:
                throw new InvalidOperationException(
                    string.Join("\n • ", _violations)
                    // _violations.JoinString(separator: "\n • ")
                );
        }
    }
}