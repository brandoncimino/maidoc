using Godot;

namespace maidoc.Scenes.Navigation;

/// <summary>
/// Delegates <a href="https://docs.godotengine.org/en/stable/tutorials/ui/gui_navigation.html">focus-based navigation</a>-related methods and properties to <paramref name="FocusableControl"/>.
/// </summary>
/// <remarks>
/// This is intended to be a "duck interface" for a subset of <see cref="Control"/> - i.e. it has the exact same member signatures.
/// <p/>
/// That should make it easy to swap between <see cref="Control"/> and <see cref="FocusWrapper"/> without refactoring anything.
/// <br/>
/// <br/>
/// 📎 This type should be treated like a <c>ref struct</c>, but cannot actually be one due to Godot 4.5 not supporting <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters#allows-ref-struct"><c>allows ref struct</c></a>.
/// </remarks>
public readonly record struct FocusWrapper {
    public FocusWrapper(Control focusableControl) => FocusableControl = focusableControl;

    private Control FocusableControl { get; }

    public NodePath FocusNeighborLeft {
        get => FocusableControl.FocusNeighborLeft;
        set => FocusableControl.FocusNeighborLeft = value;
    }

    public NodePath FocusNeighborRight {
        get => FocusableControl.FocusNeighborRight;
        set => FocusableControl.FocusNeighborRight = value;
    }

    public NodePath FocusNeighborTop {
        get => FocusableControl.FocusNeighborTop;
        set => FocusableControl.FocusNeighborTop = value;
    }

    public NodePath FocusNeighborBottom {
        get => FocusableControl.FocusNeighborBottom;
        set => FocusableControl.FocusNeighborBottom = value;
    }

    public NodePath FocusNext { get => FocusableControl.FocusNext; set => FocusableControl.FocusNext = value; }

    public NodePath FocusPrevious {
        get => FocusableControl.FocusPrevious;
        set => FocusableControl.FocusPrevious = value;
    }

    public NodePath GetPath() => FocusableControl.GetPath();

    public void GrabFocus()    => FocusableControl.GrabFocus();
    public void ReleaseFocus() => FocusableControl.ReleaseFocus();

    public NodePath GetFocusNeighbor(Side side)                    => FocusableControl.GetFocusNeighbor(side);
    public void     SetFocusNeighbor(Side side, NodePath neighbor) => FocusableControl.SetFocusNeighbor(side, neighbor);

    // public static implicit operator FocusWrapper(Control  control)      =>  new(control);
    // public static implicit operator NodePath(FocusWrapper focusWrapper) => focusWrapper.GetPath();

    public override string ToString() {
        return this.DescribeFocus();
    }
}

/// <summary>
/// Delegates <a href="https://docs.godotengine.org/en/stable/tutorials/ui/gui_navigation.html">focus-based navigation</a>-related methods and properties to <see cref="FocusableControl"/>.
/// </summary>
/// <seealso cref="FocusWrapper"/>
public interface IFocusDelegated<in TSelf> where TSelf : IFocusDelegated<TSelf> {
    protected Control FocusableControl { get; }

    public sealed FocusWrapper FocusWrapper => new(FocusableControl);
}