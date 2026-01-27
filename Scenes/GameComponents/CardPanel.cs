using System;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

[Obsolete($"Switched back to a {nameof(Node2D)}-based solution, {nameof(CardScene3)}")]
public partial class CardPanel : PanelContainer, ISceneRoot<CardPanel, CardPanel.SpawnInput> {
    [Export]
    public float UnfocusedOffsetInCardHeights { get; set; } = .5f;

    #region Spawning

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/CardUi.tscn");

    public static CardPanel InstantiateRawScene() {
        return PackedScene.Instantiate<CardPanel>();
    }

    public CardPanel InitializeSelf(SpawnInput input) {
        UpdateText(input.CardData);

        return this;
    }

    public readonly record struct SpawnInput {
        public required ICardData CardData { get; init; }
    }

    #endregion

    private readonly LazyChild<TextureRect>   _back     = new("Back");
    private readonly LazyChild<RichTextLabel> _faceText = new();

    public bool FaceDown { get => _back.Get(this).Visible; set => _back.Get(this).Visible = value; }

    // private const double NameFontSizeMultiplier = 1.1;
    private const int NameFontSize = 24;

    public void UpdateText(ICardData cardData) {
        var face = _faceText.Get(this);

        face.AppendCardFaceText(cardData);
    }

    private bool _focusedByMouse;

    public override void _Ready() {
        FocusMode    =  FocusModeEnum.All;
        FocusEntered += () => AnimateFocus(0);
        FocusExited  += LoseFocus;

        MouseEntered += GrabMouseFocus;
        MouseExited  += ReleaseMouseFocus;

        AnimateFocus(UnfocusedOffsetInCardHeights);

        // Ensure that no children can "steal" mouse inputs - we want the entire card itself to be clickable, not anything on it
        this.EnumerateChildren()
            .OfType<Control>()
            .ForEach(it => it.MouseFilter = MouseFilterEnum.Ignore);
    }

    private void GrabMouseFocus() {
        if (FocusMode == FocusModeEnum.None) {
            return;
        }

        _focusedByMouse = true;
        GrabFocus();
    }

    private void ReleaseMouseFocus() {
        if (FocusMode == FocusModeEnum.None) {
            return;
        }

        _focusedByMouse = false;
        ReleaseFocus();
    }

    public void LoseFocus() {
        AnimateFocus(Size.Y * UnfocusedOffsetInCardHeights);
    }

    private Tween? _focusTween;

    private void AnimateFocus(float targetY) { }
}