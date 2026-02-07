using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using maidoc.Core;
using maidoc.Core.Cards;

namespace maidoc.Scenes.GameComponents;

[Tool]
public partial class CardScene3 : Node2D, ISceneRoot<CardScene3, CardScene3.SpawnInput>, ICardSceneRoot {
    private readonly LazyChild<Container>     _faceContainer = new("FaceContainer");
    private readonly LazyChild<RichTextLabel> _faceText      = new("FaceText");
    private readonly LazyChild<Control>       _cardBack      = new("CardBack");

    public Control FocusableControl => _faceContainer.Get(this);

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/CardScene3.tscn");

    public static CardScene3 InstantiateRawScene() {
        return PackedScene.Instantiate<CardScene3>();
    }

    Tween? ICardSceneRoot.CurrentPositionTween { get; set; }

    public const float      StandardWidthInMeters = CardAspectRatio.Standard;
    public       Distance2D UnscaledSize { get; } = new Vector2(StandardWidthInMeters, 1).Meters;

    public ICardData CardData { get; set; } = CreatureData.JunkRhino;

    public Vector2 SizeInMeters { get => Scale * UnscaledSize.Meters; set => Scale = value / UnscaledSize.Meters; }

    public Tween? CurrentPositionTween { get; set; }

    public Node2D AsNode2D => this;

    [Export]
    public bool FaceDown { get; set; }

    public readonly record struct SpawnInput {
        public required ICardData CardData { get; init; }
    }

    [ExportToolButton(nameof(NormalizeSizes))]
    public Callable ExportSizesButton => Callable.From(NormalizeSizes);

    private void NormalizeSizes() {
        _faceContainer.Get(this)
                      .AdjustSizeAndPosition(new Rect2(Vector2.Zero, UnscaledSize.Meters));
    }

    [ExportToolButton(nameof(RefreshFace))]
    public Callable RefreshPlaceholderTextButton => Callable.From(RefreshFace);

    private void RefreshFace() {
        _cardBack.Get(this).Visible = FaceDown;
        _faceText.Get(this).Text    = "";
        _faceText.Get(this).AppendCardFaceText(CardData);
    }

    public CardScene3 InitializeSelf(SpawnInput input) {
        CardData = input.CardData;
        RefreshFace();
        return this;
    }

    public void GrabFocus() => _faceContainer.Get(this).GrabFocus();

    public void ReleaseFocus() {
        _focusedByMouse = false;
        _faceContainer.Get(this).ReleaseFocus();
    }

    public bool IsFocused => _faceContainer.Get(this).IsFocused();

    private HandView? MyHand => this.EnumerateAncestors().OfType<HandView>().FirstOrDefault();

    private bool IsInHand([MaybeNullWhen(false)] out HandView hand) {
        return (hand = MyHand) is not null;
    }

    private bool _focusedByMouse;

    private void _OnMouseEntered() {
        _focusedByMouse = true;
        GrabFocus();
    }

    private void _OnMouseExited() {
        GD.Print(
            $"Mouse exited {this.Name}; {nameof(_focusedByMouse)}: {_focusedByMouse}; {(_focusedByMouse ? "" : "NOT ")}releasing focus"
        );
        if (_focusedByMouse) {
            ReleaseFocus();
        }
    }

    public override void _Ready() {
        var face = _faceContainer.Get(this);
        face.Name += this.Name;

        face.MouseEntered += _OnMouseEntered;
        face.MouseExited  += _OnMouseExited;

        face.FocusEntered += () => {
            if (IsInHand(out var hand)) {
                hand.RequestReorganizing($"{Name} focused");
            }
        };

        face.FocusExited += () => {
            if (IsInHand(out var hand)) {
                hand.RequestReorganizing($"{Name} focus lost");
            }
        };

        face.FocusMode   = Control.FocusModeEnum.All;
        face.MouseFilter = Control.MouseFilterEnum.Stop;
        face.EnumerateChildren()
            .OfType<Control>()
            .ForEach(it => {
                    it.FocusMode   = Control.FocusModeEnum.None;
                    it.MouseFilter = Control.MouseFilterEnum.Ignore;
                }
            );
    }
}