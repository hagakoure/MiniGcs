# 🚁 Mini GCS - Ground Control Station

**Наземная станция управления беспилотником** — пет-проект для демонстрации навыков работы с Avalonia UI.

![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Avalonia](https://img.shields.io/badge/Avalonia-11.x-green)
![License](https://img.shields.io/badge/license-MIT-orange)

---

##  Описание проекта

Mini GCS (Ground Control Station) — это десктопное приложение для управления беспилотными аппаратами (дронами, роботами, морскими аппаратами). Приложение получает телеметрию в реальном времени и позволяет отправлять команды управления.

### Ключевые возможности:

-  **Подключение к устройству** по UDP с автоматическим переподключением
-  **Телеметрия в реальном времени** — координаты, высота, скорость, курс, батарея
-  **Управление устройством** — взлёт, посадка, возврат домой, зависание, аварийная остановка
-  **2D визуализация** позиции устройства (в планах — реальная карта с Mapsui)
-  **Журнал событий** — логирование всех действий и событий
-  **Симулятор устройства** — для тестирования без реального дрона

---

##  Архитектура

Проект построен по принципам **Clean Architecture** с разделением на 4 слоя:

### Требования

- .NET 8.0 SDK или выше
- Windows / Linux / macOS
---
### Установка

```bash
# Клонируем репозиторий
git clone https://github.com/your-username/MiniGcs.git
cd MiniGcs

# Восстанавливаем пакеты
dotnet restore

# Собираем проект
dotnet build
```
### Запуск
```bash
# Запустите симулятор дрона (в первом терминале):
cd tools/DroneSimulator.Console
dotnet run
# Запустите приложение MiniGcs (во втором терминале)
cd src/MiniGcs.App
dotnet run
```