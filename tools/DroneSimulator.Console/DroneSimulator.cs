using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using MiniGcs.Domain.Enums;

namespace DroneSimulator.Console;

/// <summary>
/// Эмулятор дрона для тестирования GCS
/// Отправляет телеметрию и принимает команды
/// </summary>
public class DroneSimulatorApp
{
    private readonly int _listenPort;
    private readonly string _gcsHost;
    private readonly int _gcsPort;
    
    // Состояние дрона
    private double _latitude = 55.7558;   // Москва
    private double _longitude = 37.6173;
    private double _altitude;
    private double _speed;
    private double _heading;
    private double _battery = 100;
    private DeviceState _state = DeviceState.Idle;
    
    // Целевая точка (для GoToWaypoint)
    private double? _targetLat;
    private double? _targetLon;
    
    private readonly CancellationTokenSource _cts = new();
    
    public DroneSimulatorApp(string gcsHost, int gcsPort, int listenPort)
    {
        _gcsHost = gcsHost;
        _gcsPort = gcsPort;
        _listenPort = listenPort;
    }
    
    public async Task RunAsync()
    {
        System.Console.WriteLine("Drone Simulator запущен");
        System.Console.WriteLine($"Отправка телеметрии на {_gcsHost}:{_gcsPort}");
        System.Console.WriteLine($"Ожидание команд на порту {_listenPort}");
        System.Console.WriteLine("─────────────────────────────────────");
        
        // Запускаем параллельно: отправку телеметрии и приём команд
        var telemetryTask = SendTelemetryLoop(_cts.Token);
        var commandTask = ReceiveCommandsLoop(_cts.Token);
        
        await Task.WhenAny(
            telemetryTask,
            commandTask,
            Task.Run(() => System.Console.ReadLine(), _cts.Token)
        );
        
        _cts.Cancel();
        System.Console.WriteLine("\nСимулятор остановлен");
    }
    
    /// <summary>
    /// Отправка телеметрии на GCS
    /// </summary>
    private async Task SendTelemetryLoop(CancellationToken ct)
    {
        using var udpClient = new UdpClient();
        
        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Обновляем состояние дрона
                UpdateDroneState();
                
                // Формируем бинарный пакет телеметрии
                var data = BuildTelemetryPacket();
                
                // Отправляем на GCS
                await udpClient.SendAsync(data, new IPEndPoint(IPAddress.Parse(_gcsHost), _gcsPort));
                
                // Выводим в консоль
                PrintStatus();
                
                // 10 раз в секунду
                await Task.Delay(100, ct);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nОшибка отправки: {ex.Message}");
                await Task.Delay(1000, ct);
            }
        }
    }
    
    /// <summary>
    /// Приём команд от GCS
    /// </summary>
    private async Task ReceiveCommandsLoop(CancellationToken ct)
    {
        using var listener = new UdpClient(_listenPort);
        
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await listener.ReceiveAsync(ct);
                var json = System.Text.Encoding.UTF8.GetString(result.Buffer);
                
                var command = JsonSerializer.Deserialize<CommandDto>(json);
                if (command != null)
                {
                    ProcessCommand(command);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\nОшибка приёма команды: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Обновление состояния дрона (симуляция физики)
    /// </summary>
    private void UpdateDroneState()
    {
        switch (_state)
        {
            case DeviceState.Flying:
                // Движение к целевой точке или по курсу
                if (_targetLat.HasValue && _targetLon.HasValue)
                {
                    MoveTowardsTarget();
                }
                else
                {
                    // Движение по курсу
                    _latitude += 0.0001 * Math.Cos(_heading * Math.PI / 180);
                    _longitude += 0.0001 * Math.Sin(_heading * Math.PI / 180);
                    _speed = 5.0;
                }
                
                // Разрядка батареи
                _battery = Math.Max(0, _battery - 0.05);
                
                if (_battery < 10)
                {
                    _state = DeviceState.Emergency;
                    System.Console.WriteLine("\nLOW BATTERY!");
                }
                break;
                
            case DeviceState.Landing:
                _altitude = Math.Max(0, _altitude - 0.5);
                _speed = Math.Max(0, _speed - 0.5);
                if (_altitude == 0)
                {
                    _state = DeviceState.Idle;
                    System.Console.WriteLine("\nПосадка завершена");
                }
                break;
                
            case DeviceState.ReturningHome:
                // Возврат к стартовой точке
                MoveTowardsPoint(55.7558, 37.6173);
                if (Math.Abs(_latitude - 55.7558) < 0.0001 && 
                    Math.Abs(_longitude - 37.6173) < 0.0001)
                {
                    _state = DeviceState.Landing;
                    System.Console.WriteLine("\nДома, начинаем посадку");
                }
                break;
                
            case DeviceState.HoldingPosition:
                _speed = 0;
                break;
        }
    }
    
    private void MoveTowardsTarget()
    {
        if (!_targetLat.HasValue || !_targetLon.HasValue) return;
        MoveTowardsPoint(_targetLat.Value, _targetLon.Value);
    }
    
    private void MoveTowardsPoint(double targetLat, double targetLon)
    {
        var dLat = targetLat - _latitude;
        var dLon = targetLon - _longitude;
        var distance = Math.Sqrt(dLat * dLat + dLon * dLon);
        
        if (distance < 0.0001)
        {
            _speed = 0;
            _state = DeviceState.HoldingPosition;
            _targetLat = null;
            _targetLon = null;
            System.Console.WriteLine("\nДостигли цели");
            return;
        }
        
        // Нормализуем направление
        var step = 0.0001;
        _latitude += dLat / distance * step;
        _longitude += dLon / distance * step;
        
        // Вычисляем курс
        _heading = Math.Atan2(dLon, dLat) * 180 / Math.PI;
        if (_heading < 0) _heading += 360;
        
        _speed = 5.0;
    }
    
    /// <summary>
    /// Формирование бинарного пакета телеметрии
    /// Формат: [lat:8][lon:8][alt:8][speed:8][heading:8][battery:8][state:4] = 52 байта
    /// </summary>
    private byte[] BuildTelemetryPacket()
    {
        var data = new byte[52];
        var span = data.AsSpan();
        
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(0), _latitude);
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(8), _longitude);
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(16), _altitude);
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(24), _speed);
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(32), _heading);
        BinaryPrimitives.WriteDoubleLittleEndian(span.Slice(40), _battery);
        BinaryPrimitives.WriteInt32LittleEndian(span.Slice(48), (int)_state);
        
        return data;
    }
    
    /// <summary>
    /// Обработка команды от GCS
    /// </summary>
    private void ProcessCommand(CommandDto command)
    {
        System.Console.WriteLine($"\nПолучена команда: {command.Type}");
        
        var commandType = Enum.Parse<CommandType>(command.Type);
        
        switch (commandType)
        {
            case CommandType.Arm:
                _state = DeviceState.Armed;
                System.Console.WriteLine("Устройство взведено");
                break;
                
            case CommandType.Disarm:
                _state = DeviceState.Idle;
                System.Console.WriteLine("Устройство снято с взвода");
                break;
                
            case CommandType.Takeoff:
                _state = DeviceState.Flying;
                _altitude = 10;
                System.Console.WriteLine("Взлёт!");
                break;
                
            case CommandType.Land:
                _state = DeviceState.Landing;
                System.Console.WriteLine("Посадка...");
                break;
                
            case CommandType.ReturnToHome:
                _state = DeviceState.ReturningHome;
                System.Console.WriteLine("Возврат домой...");
                break;
                
            case CommandType.HoldPosition:
                _state = DeviceState.HoldingPosition;
                System.Console.WriteLine("Зависание в точке");
                break;
                
            case CommandType.EmergencyStop:
                _state = DeviceState.Emergency;
                _speed = 0;
                System.Console.WriteLine("АВАРИЙНАЯ ОСТАНОВКА!");
                break;
                
            default:
                System.Console.WriteLine($"Неизвестная команда: {commandType}");
                break;
        }
    }
    
    /// <summary>
    /// Вывод статуса в консоль
    /// </summary>
    private void PrintStatus()
    {
        System.Console.Write(
            $"\r[Drone] Lat={_latitude:F6} Lon={_longitude:F6} " +
            $"Alt={_altitude:F1}m Speed={_speed:F1}m/s " +
            $"Heading={_heading:F0}° Bat={_battery:F0}% State={_state,-15}");
    }
    
    private record CommandDto(string CommandId, string Type, DateTime Timestamp);
}