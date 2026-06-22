namespace MiniGcs.Domain.Enums;

/// <summary>
/// Типы команд, которые можно отправить устройству
/// </summary>
public enum CommandType
{
    /// <summary>Взвести устройство (подготовить к полёту)</summary>
    Arm = 0,

    /// <summary>Снять с взвода</summary>
    Disarm = 1,

    /// <summary>Взлёт</summary>
    Takeoff = 2,

    /// <summary>Посадка</summary>
    Land = 3,

    /// <summary>Возврат в точку старта</summary>
    ReturnToHome = 4,

    /// <summary>Зависание в текущей позиции</summary>
    HoldPosition = 5,

    /// <summary>Следование к waypoint</summary>
    GoToWaypoint = 6,

    /// <summary>Начать выполнение миссии</summary>
    StartMission = 7,

    /// <summary>Прервать текущую операцию</summary>
    Abort = 8,

    /// <summary>Экстренная остановка</summary>
    EmergencyStop = 9
}