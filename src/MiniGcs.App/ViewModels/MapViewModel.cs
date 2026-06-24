using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniGcs.App.ViewModels;

public partial class MapViewModel : ObservableObject
{
    // Позиция центра карты (в координатах OSM)
    [ObservableProperty] private double _centerLat = 55.7558;  
    [ObservableProperty] private double _centerLon = 37.6173;
    
    // Уровень зума (1-19)
    [ObservableProperty] private int _zoomLevel = 13;
    
    // Позиция дрона
    [ObservableProperty] private double _droneLat = 55.7558;
    [ObservableProperty] private double _droneLon = 37.6173;
    [ObservableProperty] private double _droneHeading;
    
    // Автоцентрирование
    [ObservableProperty] private bool _autoCenterOnDrone = false;
    
    // Waypoints (для будущего использования)
    public ObservableCollection<WaypointViewModel> Waypoints { get; } = [];
    
    /// <summary>
    /// Обновить позицию дрона
    /// </summary>
    public void UpdatePosition(double lat, double lon, double heading)
    {
        DroneLat = lat;
        DroneLon = lon;
        DroneHeading = heading;
        
        if (AutoCenterOnDrone)
        {
            CenterLat = lat;
            CenterLon = lon;
        }
    }
    
    /// <summary>
    /// Центрировать карту на дроне
    /// </summary>
    public void CenterOnDrone()
    {
        CenterLat = DroneLat;
        CenterLon = DroneLon;
    }
    
    /// <summary>
    /// Приблизить
    /// </summary>
    public void ZoomIn()
    {
        if (ZoomLevel < 19) ZoomLevel++;
    }
    
    /// <summary>
    /// Отдалить
    /// </summary>
    public void ZoomOut()
    {
        if (ZoomLevel > 1) ZoomLevel--;
    }
    
    /// <summary>
    /// Переключить автоцентрирование
    /// </summary>
    public void ToggleAutoCenter()
    {
        AutoCenterOnDrone = !AutoCenterOnDrone;
    }
}

/// <summary>
/// ViewModel для waypoint (для будущего использования)
/// </summary>
public partial class WaypointViewModel : ObservableObject
{
    [ObservableProperty] private double _latitude;
    [ObservableProperty] private double _longitude;
    [ObservableProperty] private string _label = "";
    [ObservableProperty] private bool _isSelected;
}