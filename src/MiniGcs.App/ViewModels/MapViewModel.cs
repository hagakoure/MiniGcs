using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniGcs.App.ViewModels;

public partial class MapViewModel : ObservableObject
{
    // Позиция дрона на карте (в пикселях)
    [ObservableProperty] private double _droneX = 400;
    [ObservableProperty] private double _droneY = 300;
    [ObservableProperty] private double _droneRotation;
    
    // Центр карты
    [ObservableProperty] private double _centerLat = 55.7558;
    [ObservableProperty] private double _centerLon = 37.6173;
    
    // Масштаб (метры на пиксель)
    private const double MetersPerPixel = 10.0;
    private const double DegreesPerMeterLat = 1.0 / 111320.0;
    
    public void UpdateDronePosition(double lat, double lon, double heading)
    {
        // Вычисляем смещение от центра карты в метрах
        var dLat = lat - CenterLat;
        var dLon = lon - CenterLon;
        
        // Примерный перевод в метры
        var metersLat = dLat / DegreesPerMeterLat;
        var metersLon = dLon / (DegreesPerMeterLat * Math.Cos(CenterLat * Math.PI / 180));
        
        // Переводим в пиксели (400x600 - размер области карты)
        DroneX = 400 + metersLon / MetersPerPixel;
        DroneY = 300 - metersLat / MetersPerPixel;
        DroneRotation = heading;
    }
}