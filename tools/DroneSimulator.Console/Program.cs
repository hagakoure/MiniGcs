using DroneSimulator.Console;

Console.WriteLine("╔═══════════════════════════════════════╗");
Console.WriteLine("║         DRONE SIMULATOR v1.0          ║");
Console.WriteLine("║  Эмулятор устройства для тестирования ║");
Console.WriteLine("╚═══════════════════════════════════════╝");
Console.WriteLine();

// Параметры подключения к GCS
const string gcsHost = "127.0.0.1";
const int gcsPort = 9001;      // Порт, на котором GCS слушает телеметрию
const int listenPort = 9000;   // Порт, на котором симулятор слушает команды

var simulator = new DroneSimulatorApp(gcsHost, gcsPort, listenPort);

try
{
    await simulator.RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"\nКритическая ошибка: {ex.Message}");
}