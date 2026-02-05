using System;
using Godot;
using maidoc.Scenes.Navigation;

namespace maidoc.Scenes.GameComponents;

public interface ICardSceneRoot : IFocusDelegated<ICardSceneRoot>, IGameNode2D {
    public bool FaceDown { get; set; }

    public Vector2 SizeInMeters { get; set; }

    protected Tween? CurrentPositionTween { get; set; }

    protected Tween CreateTween();

    public void AnimatePosition(
        Distance2D     destination,
        double         durationInSeconds = .1,
        Action<Tween>? moreTweening      = null
    ) {
        GD.Print($"Animating {this} from {LocalPosition} → {destination}");

        CurrentPositionTween?.Kill();
        CurrentPositionTween = CreateTween();
        CurrentPositionTween.TweenMethod(
                                Callable.From<Vector2>(posInMeters => LocalPosition = posInMeters.Meters()),
                                LocalPosition.Meters,
                                destination.Meters,
                                durationInSeconds
                            )
                            .SetTrans(Tween.TransitionType.Cubic)
                            .SetEase(Tween.EaseType.Out);

        moreTweening?.Invoke(CurrentPositionTween);
    }

    public bool IsFocused { get; }
}