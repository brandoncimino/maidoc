using Godot;
using maidoc.Scenes.Navigation;

namespace maidoc.Scenes.GameComponents;

public interface ICardSceneRoot : IFocusDelegated<ICardSceneRoot> {
    public Node2D AsNode2D { get; }

    public bool FaceDown { get; set; }

    public Vector2 SizeInMeters     { get; set; }
    public Vector2 PositionInMeters { get; set; }

    protected Tween? CurrentPositionTween { get; set; }

    protected Tween CreateTween();

    public void AnimatePosition(Vector2 destinationInMeters, double durationInSeconds = .1) {
        CurrentPositionTween?.Kill();
        CurrentPositionTween = CreateTween();
        CurrentPositionTween.TweenMethod(
                                Callable.From<Vector2>(pos => PositionInMeters = pos),
                                PositionInMeters,
                                destinationInMeters,
                                durationInSeconds
                            )
                            .SetTrans(Tween.TransitionType.Cubic)
                            .SetEase(Tween.EaseType.Out);
    }

    public bool IsFocused { get; }
}