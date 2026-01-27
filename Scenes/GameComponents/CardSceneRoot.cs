using System;
using Godot;
using maidoc.Core.Cards;
using maidoc.Core.NormalCreatures;

namespace maidoc.Scenes.GameComponents;

[Obsolete(message: $"Use {nameof(CardScene3)} instead. This is kept for reference.")]
[Tool]
public partial class CardSceneRoot : InteractiveSceneRoot2D<CardSceneRoot, CardSceneRoot.Input>,
    ISceneRoot<CardSceneRoot, CardSceneRoot.Input> {
    private readonly Disenfranchised<PaperCard> _myCard = new();
    public           PaperCard                  MyCard => _myCard.Value;

    private readonly LazyChild<Control>          _cardFace           = new("Face");
    private readonly LazyChild<Sprite2D>         _cardFrame          = new("Frame");
    private readonly LazyChild<CollisionShape2D> _clickBox           = new("ClickBox");
    private readonly LazyChild<RichTextLabel>    _text               = new("Text");
    private readonly LazyChild<TextureRect>      _cardFaceBackground = new("Background");
    private readonly LazyChild<Sprite2D>         _cardBack           = new("Back");

    private const float OneMeterPlayerCardHeight = 1;
    private const float OneMeterPlayingCardWidth = 1 * CardAspectRatio.Standard;

    [Export]
    public Vector2 CardSizeInMeters = new(OneMeterPlayingCardWidth, OneMeterPlayerCardHeight);

    [Export]
    public float MaximumFontSizeInMeters = .1f;

    private bool _faceDown;

    public bool FaceDown {
        get => _faceDown;
        set {
            _faceDown                   = value;
            _cardBack.Get(this).Visible = value;
        }
    }

    public override void _Ready() { }

    [ExportToolButton(nameof(NormalizeSizes))]
    public Callable NormalizeSizesTool => Callable.From(NormalizeSizes);

    [ExportToolButton("Describe Face")]
    public Callable DescribeFaceTool => Callable.From(() => {
            var faceText = _text.Get(this);
            faceText.blog(
                faceText.Text,
                faceText.GetMinimumSize(),
                faceText.GetSize(),
                faceText.GetRect(),
                faceText.AnchorLeft,
                faceText.AnchorRight
            );
        }
    );

    private void NormalizeSizes() {
        var desiredRectInMeters = GodotHelpers.Rect2ByCenter(
            Position / GodotHelpers.GodotUnitsPerMeter,
            CardSizeInMeters
        );

        desiredRectInMeters.blog();

        _cardFace.Get(this).AdjustSizeAndPosition(desiredRectInMeters);
        _cardFrame.Get(this).AdjustSizeAndPosition(desiredRectInMeters);
        _clickBox.Get(this).AdjustSizeAndPosition(desiredRectInMeters);
        _cardFaceBackground.Get(this).BindToParent();

        _text.Get(this).BindToParent(
            new() {
                Horizontal = new() {
                    Meters = .05f
                },
                Top = new() {
                    Meters = .1f
                },
                Bottom = new() {
                    Meters = .05f
                }
            }
        );
    }

    public CardSceneRoot InitializeSelf(Input input) {
        if (input is { MyCard: NormalCreatureCard normalCreatureCard }) { }

        return this;
    }

    public record Input {
        public required PaperCard MyCard  { get; init; }
        public          Action?   OnClick { get; init; }
    }

    private static PackedScene? _packedScene;

    private static PackedScene PackedScene =>
        _packedScene ??= ResourceLoader.Load<PackedScene>("res://Scenes/GameComponents/Card.tscn");

    public static CardSceneRoot InstantiateRawScene() {
        return PackedScene.Instantiate<CardSceneRoot>();
    }

    public void GrabFocus() {
        throw new NotImplementedException();
    }
}