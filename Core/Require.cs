using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace maidoc.Core;

public static class Require {
    public static T Argument<T>(
        T    argumentValue,
        [DoesNotReturnIf(false)]
        bool condition,
        [CallerArgumentExpression(nameof(argumentValue))]
        string parameterName = "",
        [CallerArgumentExpression(nameof(condition))]
        string _condition = ""
    ) {
        if (condition) {
            return argumentValue;
        }

        throw new ArgumentOutOfRangeException(
            parameterName,
            argumentValue,
            $"Failed condition: {_condition}"
        );
    }

    public static T NotNull<T>(
        T? argumentValue,
        [CallerArgumentExpression(nameof(argumentValue))]
        string parameterName = ""
    ) where T : notnull {
        ArgumentNullException.ThrowIfNull(argumentValue, parameterName);
        return argumentValue;
    }

    public static void State(
        bool condition,
        [CallerArgumentExpression(nameof(condition))]
        string _condition = "",
        [CallerMemberName] string _caller = ""
    ) {
        if (!condition) {
            throw new InvalidOperationException($"Cannot invoke {_caller} because of a failed condition: {_condition}");
        }
    }
}

public static class RequireExtensions {
    public static T Require<T>(this T argumentValue, Func<T, bool> condition,
        [CallerArgumentExpression(nameof(argumentValue))]
        string parameterName = "",
        [CallerArgumentExpression(nameof(condition))]
        string _condition = "") {
        return Core.Require.Argument(argumentValue, condition(argumentValue), parameterName, _condition);
    }
}