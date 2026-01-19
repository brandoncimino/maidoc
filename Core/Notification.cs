namespace maidoc.Core;

public readonly record struct Notification {
    public required string           Message { get; init; }
    public          NotificationTone Tone    { get; init; }

    public enum NotificationTone {
        Neutral,
        Positive,
        Negative
    }
}