namespace maidoc.Core.Cards;

/// <summary>
/// A physical card that can be shuffled, drawn, etc.
/// </summary>
/// <remarks>
/// <h1>⚠️ WARNING ⚠️</h1>
/// It is crucial that this type is <b><i>NOT</i></b> a <c>record</c> - it MUST use <see cref="object.ReferenceEquals"/>!
/// </remarks>
public abstract class PaperCard : ISelectable {
    public required PlayerId  OwnerId     { get; init; }
}