namespace MiniGcs.Domain.Enums;

/// <summary>
/// Действие, которое нужно выполнить в waypoint
/// </summary>
public enum WaypointAction
{
    /// <summary>Нет действия, просто пролететь</summary>
    None = 0,

    /// <summary>Сделать фото</summary>
    TakePhoto = 1,

    /// <summary>Зависнуть на N секунд</summary>
    Hover = 2,

    /// <summary>Посадка</summary>
    Land = 3,

    /// <summary>Начать видеозапись</summary>
    StartVideo = 4,

    /// <summary>Остановить видеозапись</summary>
    StopVideo = 5
}