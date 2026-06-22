namespace MiniGcs.Domain.ValueObjects;

/// <summary>
/// Высота (метры)
/// </summary>
public record Altitude
{
    /// <summary>Высота в метрах</summary>
    public double Meters { get; }

    /// <summary>Высота в футах</summary>
    public double Feet => Meters * 3.28084;

    public Altitude(double meters)
    {
        if (meters < 0)
            throw new ArgumentOutOfRangeException(nameof(meters),
                "Высота не может быть отрицательной");

        Meters = meters;
    }

    /// <summary>Создать из футов</summary>
    public static Altitude FromFeet(double feet) => new(feet / 3.28084);

    /// <summary>Нулевая высота</summary>
    public static Altitude Zero => new(0);
}