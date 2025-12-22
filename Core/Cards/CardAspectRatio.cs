namespace maidoc.Core.Cards;

public readonly record struct CardAspectRatio {
    public const double MillimetersPerInch  = 25.4;
    public const double InchesPerMillimeter = 1 / MillimetersPerInch;

    public const double StandardWidth        = 2.5 * MillimetersPerInch;
    public const double StandardHeight       = 3.5 * MillimetersPerInch;
    public const double StandardCornerRadius = .125 * MillimetersPerInch;

    public const float Standard = 2.5f  / 3.5f;
    public const double YuGiOh   = 59.0 / 86.0;
    public const double Tarot    = 70.0 / 120.0;
}