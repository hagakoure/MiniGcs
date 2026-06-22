using CommunityToolkit.Mvvm.ComponentModel;
using MiniGcs.Application.DTOs;

namespace MiniGcs.App.ViewModels;

public partial class TelemetryViewModel : ObservableObject
{
    [ObservableProperty] private double _latitude;
    [ObservableProperty] private double _longitude;
    [ObservableProperty] private double _altitude;
    [ObservableProperty] private double _speed;
    [ObservableProperty] private double _speedKmh;
    [ObservableProperty] private double _heading;
    [ObservableProperty] private double _batteryLevel;
    [ObservableProperty] private string _stateText = "Disconnected";
    [ObservableProperty] private string _lastUpdate = "--:--:--";

    public void UpdateFromDto(TelemetryDto dto)
    {
        Latitude = dto.Latitude;
        Longitude = dto.Longitude;
        Altitude = dto.Altitude;
        Speed = dto.Speed;
        SpeedKmh = dto.SpeedKmh;
        Heading = dto.Heading;
        BatteryLevel = dto.BatteryLevel;
        StateText = dto.State.ToString();
        LastUpdate = dto.Timestamp.ToLocalTime().ToString("HH:mm:ss");
    }
}