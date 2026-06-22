using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using MiniGcs.Domain.Enums;
using MiniGcs.Domain.Events;
using MiniGcs.Domain.Interfaces;
using MiniGcs.Domain.ValueObjects;

namespace MiniGcs.Infrastructure.Network;

/// <summary>
/// Реализация подключения к устройству через UDP
/// </summary>
public class UdpDeviceConnection(ILogger<UdpDeviceConnection> logger) : IDeviceConnection, IDisposable
{
    private UdpClient? _udpClient;
    private CancellationTokenSource? _cts;
    private Task? _receiveTask;
    
    private string _host = string.Empty;
    private int _port;
    private string _deviceId = "drone-001";
    
    public bool IsConnected { get; private set; }
    
    public event Action<TelemetryReceivedEvent>? TelemetryReceived;
    public event Action<bool>? ConnectionStateChanged;

    /// <summary>
    /// Подключиться к устройству
    /// </summary>
    public async Task ConnectAsync(string host, int port, CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            logger.LogWarning("Уже подключено к устройству");
            return;
        }
        
        try
        {
            _host = host;
            _port = port;
            
            logger.LogInformation("Подключение к {Host}:{Port}...", host, port);
            
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            // Создаём UDP клиент и привязываем к локальному порту для приёма данных
            _udpClient = new UdpClient(port);
            
            IsConnected = true;
            ConnectionStateChanged?.Invoke(true);
            
            logger.LogInformation("Успешно подключено к {Host}:{Port}", host, port);
            
            // Запускаем приём телеметрии в фоне
            _receiveTask = Task.Run(() => ReceiveLoop(_cts.Token), _cts.Token);
            
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка подключения к {Host}:{Port}", host, port);
            await DisconnectAsync(cancellationToken);
            throw;
        }
    }
    
    /// <summary>
    /// Отключиться от устройства
    /// </summary>
    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Отключение от устройства...");
        
        _cts?.Cancel();
        
        try
        {
            _receiveTask?.Wait(TimeSpan.FromSeconds(2));
        }
        catch
        {
            // Игнорируем ошибки при ожидании
        }
        
        _udpClient?.Dispose();
        _udpClient = null;
        
        _cts?.Dispose();
        _cts = null;
        
        IsConnected = false;
        ConnectionStateChanged?.Invoke(false);
        
        logger.LogInformation("Отключено от устройства");
        
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Отправить команду устройству
    /// </summary>
    public async Task SendCommandAsync(CommandType command, CancellationToken cancellationToken = default)
    {
        if (_udpClient == null || !IsConnected)
        {
            throw new InvalidOperationException("Нет подключения к устройству");
        }
        
        try
        {
            // Формируем JSON команду
            var commandDto = new
            {
                CommandId = Guid.NewGuid().ToString(),
                Type = command.ToString(),
                Timestamp = DateTime.UtcNow
            };
            
            var json = JsonSerializer.Serialize(commandDto);
            var data = System.Text.Encoding.UTF8.GetBytes(json);
            
            // Отправляем на хост:порт симулятора (9000)
            var endPoint = new IPEndPoint(IPAddress.Parse(_host), 9000);
            await _udpClient.SendAsync(data, endPoint, cancellationToken);
            
            logger.LogInformation("Отправлена команда: {Command}", command);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка отправки команды {Command}", command);
            throw;
        }
    }
    
    /// <summary>
    /// Цикл приёма телеметрии
    /// </summary>
    private async Task ReceiveLoop(CancellationToken ct)
    {
        logger.LogInformation("Запущен цикл приёма телеметрии");
        
        while (!ct.IsCancellationRequested && IsConnected && _udpClient != null)
        {
            try
            {
                var result = await _udpClient.ReceiveAsync(ct);
                
                // Парсим бинарные данные телеметрии
                var telemetry = ParseTelemetry(result.Buffer);
                
                // Уведомляем подписчиков
                TelemetryReceived?.Invoke(telemetry);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (SocketException ex)
            {
                logger.LogWarning(ex, "Ошибка сокета при приёме данных");
                await Task.Delay(100, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Неожиданная ошибка при приёме телеметрии");
                await Task.Delay(500, ct);
            }
        }
        
        logger.LogInformation("Цикл приёма телеметрии остановлен");
    }
    
    /// <summary>
    /// Парсинг бинарных данных телеметрии
    /// Формат: [lat:8][lon:8][alt:8][speed:8][heading:8][battery:8][state:4] = 52 байта
    /// </summary>
    private TelemetryReceivedEvent ParseTelemetry(byte[] data)
    {
        if (data.Length < 52)
        {
            throw new InvalidOperationException($"Недостаточно данных: получено {data.Length}, нужно 52");
        }
        
        var span = data.AsSpan();
        
        var latitude = BinaryPrimitives.ReadDoubleLittleEndian(span.Slice(0, 8));
        var longitude = BinaryPrimitives.ReadDoubleLittleEndian(span.Slice(8, 8));
        var altitude = BinaryPrimitives.ReadDoubleLittleEndian(span.Slice(16, 8));
        var speed = BinaryPrimitives.ReadDoubleLittleEndian(span.Slice(24, 8));
        var heading = BinaryPrimitives.ReadDoubleLittleEndian(span.Slice(32, 8));
        var battery = BinaryPrimitives.ReadDoubleLittleEndian(span.Slice(40, 8));
        var stateInt = BinaryPrimitives.ReadInt32LittleEndian(span.Slice(48, 4));
        
        var state = (DeviceState)stateInt;
        
        return new TelemetryReceivedEvent(
            deviceId: _deviceId,
            position: new GeoPosition(latitude, longitude),
            altitude: new Altitude(altitude),
            speed: new Speed(speed),
            heading: heading,
            batteryLevel: battery,
            state: state
        );
    }
    
    /// <summary>
    /// Освобождение ресурсов
    /// </summary>
    public void Dispose()
    {
        DisconnectAsync().GetAwaiter().GetResult();
    }
}