namespace MiniGcs.Domain.ValueObjects;

/// <summary>
/// Географическая позиция (координаты)
/// </summary>
public record GeoPosition
{
    /// <summary>Широта (-90 до 90)</summary>
    public double Latitude { get; }

    /// <summary>Долгота (-180 до 180)</summary>
    public double Longitude { get; }

    public GeoPosition(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude),
                "Широта должна быть от -90 до 90");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude),
                "Долгота должна быть от -180 до 180");

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Вычисляет расстояние до другой точки (формула Haversine)
    /// </summary>
    public double DistanceTo(GeoPosition other)
    {
        const double r = 6371000; // Радиус Земли в метрах

        var lat1 = Latitude * Math.PI / 180;
        var lat2 = other.Latitude * Math.PI / 180;
        var deltaLat = (other.Latitude - Latitude) * Math.PI / 180;
        var deltaLon = (other.Longitude - Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return r * c; // Расстояние в метрах
    }

    /// <summary>Нулевая позиция (для инициализации)</summary>
    public static GeoPosition Zero => new(0, 0);

    /// <summary>test</summary>
    public static GeoPosition Moscow => new(55.7558, 37.6173);
}