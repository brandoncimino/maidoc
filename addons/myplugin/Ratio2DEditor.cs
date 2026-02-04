#if TOOLS
using System;
using Godot;
using maidoc.Scenes;

[Tool]
public partial class Ratio2DEditor : EditorProperty {
    private static EditorSpinSlider CreateNumberEntry(
        string label,
        string suffix
    ) {
        return new() {
            Suffix     = suffix,
            Step       = 0,
            MinValue   = double.NegativeInfinity,
            MaxValue   = double.PositiveInfinity,
            Label      = label,
            HideSlider = true,
        };
    }

    private readonly EditorSpinSlider _xEntry;
    private readonly EditorSpinSlider _yEntry;
    private readonly Func<Vector2>    _referenceValue;

    public Ratio2DEditor(
        string        unitSuffix,
        Func<Vector2> referenceValue
    ) {
        _referenceValue = referenceValue;

        var vBox = new VBoxContainer()
            .AsChildOf(this);

        _xEntry = CreateNumberEntry("x", unitSuffix);
        _yEntry = CreateNumberEntry("y", unitSuffix);

        vBox.AddChild(_xEntry);
        vBox.AddChild(_yEntry);

        AddFocusable(_xEntry);
        AddFocusable(_yEntry);

        _xEntry.ValueChanged += _ => { _OnValueChanged(); };
        _yEntry.ValueChanged += _ => { _OnValueChanged(); };
    }

    private void _OnValueChanged() {
        var meters = new Vector2((float)_xEntry.Value, (float)_yEntry.Value);
        var pixels = Ratio2D.CalculateActual(meters, _referenceValue(), RectMarker.ReferenceAxis.Corresponding);
        EmitChanged(GetEditedProperty(), pixels);
    }

    public override void _UpdateProperty() {
        var pixels = GetEditedObject().Get(GetEditedProperty()).AsVector2();
        var ratio  = Ratio2D.CalculateRatio(pixels, _referenceValue(), RectMarker.ReferenceAxis.Corresponding);

        _xEntry.SetValueNoSignal(ratio.X);
        _yEntry.SetValueNoSignal(ratio.Y);
    }
}

#endif