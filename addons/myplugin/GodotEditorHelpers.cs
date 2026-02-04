#if TOOLS
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;

[Obsolete("This was an experiment. Also, what happened to that annotation?")]
public static class GodotEditorHelpers {
    public static void AddRatio2DPropertyEditor(
        this EditorInspectorPlugin editorInspectorPlugin,
        string                     backingPropertyName,
        string                     unitPlural,
        string                     unitSymbol,
        Func<Vector2>              referenceValue,
        bool                       addToEnd = false,
        string?                    label    = null
    ) {
        editorInspectorPlugin.AddPropertyEditor(
            backingPropertyName,
            new Ratio2DEditor(
                unitSymbol,
                referenceValue
            ),
            addToEnd,
            label ?? $"{backingPropertyName} in {unitPlural}"
        );
    }

    public static Dictionary CreatePropertyDictionary(
        string       name,
        Variant.Type variantType,
        string       className  = "",
        PropertyHint hint       = PropertyHint.None,
        string       hintString = ""
    ) {
        return new GodotProperty() {
            Name       = name,
            Type       = variantType,
            ClassName  = className,
            Hint       = hint,
            HintString = hintString
        }.ToGodotDictionary();
    }

    /// <summary>
    /// Creates a <see cref="GodotProperty"/> that describes <paramref name="backer"/>.
    /// </summary>
    /// <param name="backer">The actual C# property that you are delegating to.</param>
    /// <param name="nameInEditor">The name that will be used for the property in the Godot UI.<br/>Defaults to the expression used for <paramref name="backer"/>.</param>
    /// <param name="variantType">The <see cref="Variant.Type"/> of the property.
    /// <br/>Defaults to <see cref="T"/>.</param>
    /// <param name="hint">See <see cref="ExportAttribute.Hint"/></param>
    /// <param name="hintString">See <see cref="ExportAttribute.HintString"/></param>
    /// <param name="usage">Seems to control <i>functional behavior</i> of the property, e.g. <see cref="PropertyUsageFlags.ReadOnly"/>.
    /// <br/>📎 This defaults to <see cref="PropertyUsageFlags.Editor"/> rather than <see cref="PropertyUsageFlags.Default"/> because if you wanted the actual default behavior, you'd be using <see cref="ExportAttribute"/>.</param>
    /// <param name="className">🤷‍♀️, but it's used by <see cref="GodotObject.GetPropertyList"/></param>
    /// <typeparam name="T">The underlying <see cref="Variant"/> type</typeparam>
    /// <returns></returns>
    public static GodotProperty CreateGodotProperty<[MustBeVariant] T>(
        this T backer,
        string hintString = "",
        [CallerArgumentExpression(nameof(backer))]
        string nameInEditor = "",
        Variant.Type?      variantType = null,
        PropertyHint       hint        = PropertyHint.None,
        PropertyUsageFlags usage       = PropertyUsageFlags.Editor | PropertyUsageFlags.NoInstanceState,
        string             className   = ""
    ) {
        return new GodotProperty() {
            Name       = nameInEditor,
            Type       = variantType ?? Variant.From(backer).VariantType,
            ClassName  = className,
            Hint       = hint,
            HintString = hintString,
            Usage      = usage
        };
    }
}

/// <summary>
/// Represents a single entry in <see cref="GodotObject.GetPropertyList"/>/<see cref="GodotObject._GetPropertyList"/>, which also corresponds to the parameters in <see cref="EditorInspectorPlugin._ParseProperty"/>:
/// <br/>
/// <inheritdoc cref="GodotObject.GetPropertyList"/>
/// </summary>
public readonly record struct GodotProperty() {
    public required string             Name       { get; init; }
    public required Variant.Type       Type       { get; init; }
    public          StringName         ClassName  { get; init; } = "";
    public          PropertyHint       Hint       { get; init; }
    public          string             HintString { get; init; } = "";
    public          PropertyUsageFlags Usage      { get; init; } = PropertyUsageFlags.Default;

    public Dictionary ToGodotDictionary() {
        return new Dictionary() {
            { "name", Name },
            { "type", (int)Type },
            { "class_name", ClassName },
            { "hint", (int)Hint },
            { "hint_string", HintString },
            { "usage", (int)Usage }
        };
    }

    public enum HeaderType {
        Group,
        Category,
        Subgroup
    }

    public static GodotProperty Header(
        string     name,
        HeaderType headerType = HeaderType.Group
    ) {
        return new GodotProperty() {
            Name = name,
            Type = 0,
            Usage = headerType switch {
                HeaderType.Group    => PropertyUsageFlags.Group,
                HeaderType.Category => PropertyUsageFlags.Category,
                HeaderType.Subgroup => PropertyUsageFlags.Subgroup,
                _                   => throw new ArgumentOutOfRangeException(nameof(headerType), headerType, null)
            }
        };
    }

    [Pure]
    public static GodotProperty FromDictionary(Dictionary godotDictionary) {
        return new GodotProperty() {
            Name       = godotDictionary["name"].AsString(),
            ClassName  = godotDictionary["class_name"].AsStringName(),
            Type       = (Variant.Type)godotDictionary["type"].AsInt32(),
            Usage      = (PropertyUsageFlags)godotDictionary["usage"].AsInt32(),
            Hint       = (PropertyHint)godotDictionary["hint"].AsInt32(),
            HintString = godotDictionary["hint_string"].AsString()
        };
    }
}
#endif