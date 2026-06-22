namespace MiniGcs.Domain.ValueObjects;

/// <summary>
/// Скорость (м/с)
/// </summary>
public record Speed
{
    /// <summary>Скорость в метрах в секунду</summary>
    public double MetersPerSecond { get; }

    /// <summary>Скорость в км/ч</summary>
    public double KilometersPerHour => MetersPerSecond * 3.6;

    /// <summary>Скорость в узлах</summary>
    public double Knots => MetersPerSecond * 1.94384;

    public Speed(double metersPerSecond)
    {
        if (metersPerSecond < 0)
            throw new ArgumentOutOfRangeException(nameof(metersPerSecond),
                "Скорость не может быть отрицательной");

        MetersPerSecond = metersPerSecond;
    }

    /// <summary>Создать из км/ч</summary>
    public static Speed FromKilometersPerHour(double kmh) => new(kmh / 3.6);

    /// <summary>Создать из узлов</summary>
    public static Speed FromKnots(double knots) => new(knots / 1.94384);

    /// <summary>Нулевая скорость</summary>
    public static Speed Zero => new(0);
}