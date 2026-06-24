using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using MiniGcs.App.ViewModels;

namespace MiniGcs.App.Views;

/// <summary>
/// Кастомный контрол карты на основе Canvas
/// Рисует тайлы OSM, маркер дрона, waypoints
/// </summary>
public class MapCanvas : Control
{
    private static readonly HttpClient HttpClient = new();
    private readonly ConcurrentDictionary<string, Bitmap?> _tileCache = new();
    
    private MapViewModel? _viewModel;
    private Point _dragStart;
    private (double lat, double lon) _dragStartCenter;
    private bool _isDragging;
    
    // Размеры тайлов OSM
    private const int TileSize = 256;
    
    public MapCanvas()
    {
        // Включаем обработку событий мыши
        ClipToBounds = true;
    }
    
    /// <summary>
    /// Привязать ViewModel
    /// </summary>
    public void BindViewModel(MapViewModel viewModel)
    {
        _viewModel = viewModel;
        
        // Подписываемся на изменения свойств
        _viewModel.PropertyChanged += (s, e) =>
        {
            Dispatcher.UIThread.Post(InvalidateVisual);
        };
    }
    
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        
        if (_viewModel == null) return;
        
        // Рисуем фон
        context.FillRectangle(
            new SolidColorBrush(Color.FromRgb(200, 200, 200)),
            new Rect(0, 0, Bounds.Width, Bounds.Height));
        
        // Рисуем тайлы OSM
        DrawTiles(context);
        
        // Рисуем маркер дрона
        DrawDroneMarker(context);
        
        // Рисуем waypoints (если есть)
        DrawWaypoints(context);
        
        // Рисуем перекрестие в центре
        DrawCrosshair(context);
    }
    
    /// <summary>
    /// Рисует тайлы OpenStreetMap
    /// </summary>
    private void DrawTiles(DrawingContext context)
    {
        if (_viewModel == null) return;
        
        var zoom = _viewModel.ZoomLevel;
        var centerTileX = LonToTileX(_viewModel.CenterLon, zoom);
        var centerTileY = LatToTileY(_viewModel.CenterLat, zoom);
        
        // Сколько тайлов нужно по сторонам от центра
        var tilesX = (int)Math.Ceiling(Bounds.Width / (2 * TileSize)) + 1;
        var tilesY = (int)Math.Ceiling(Bounds.Height / (2 * TileSize)) + 1;
        
        // Центр экрана
        var screenCenterX = Bounds.Width / 2;
        var screenCenterY = Bounds.Height / 2;
        
        // Смещение в пикселях внутри центрального тайла
        var offsetX = (centerTileX - Math.Floor(centerTileX)) * TileSize;
        var offsetY = (centerTileY - Math.Floor(centerTileY)) * TileSize;
        
        for (int dx = -tilesX; dx <= tilesX; dx++)
        {
            for (int dy = -tilesY; dy <= tilesY; dy++)
            {
                int tileX = (int)Math.Floor(centerTileX) + dx;
                int tileY = (int)Math.Floor(centerTileY) + dy;
                
                // Позиция на экране
                var screenX = screenCenterX + dx * TileSize - offsetX;
                var screenY = screenCenterY + dy * TileSize - offsetY;
                
                // Загружаем тайл
                var tileUrl = $"https://a.basemaps.cartocdn.com/rastertiles/voyager/{zoom}/{tileX}/{tileY}.png";
                var bitmap = GetOrLoadTile(tileUrl);
                
                if (bitmap != null)
                {
                    context.DrawImage(bitmap, new Rect(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                        new Rect(screenX, screenY, TileSize, TileSize));
                }
                else
                {
                    // Рисуем placeholder
                    context.DrawRectangle(
                        new Pen(Brushes.Gray, 1),
                        new Rect(screenX, screenY, TileSize, TileSize));
                }
            }
        }
    }
    
    /// <summary>
    /// Рисует маркер дрона
    /// </summary>
    private void DrawDroneMarker(DrawingContext context)
    {
        if (_viewModel == null) return;
    
        var pos = LatLonToScreen(_viewModel.DroneLat, _viewModel.DroneLon);
    
        // Рисуем треугольник дрона
        var triangle = new PathGeometry();
        using (var ctx = triangle.Open())
        {
            ctx.BeginFigure(new Point(0, -12), true);  // ← добавили isFilled: true
            ctx.LineTo(new Point(10, 10));
            ctx.LineTo(new Point(0, 5));
            ctx.LineTo(new Point(-10, 10));
            ctx.EndFigure(true);
        }
    
        // Поворот по курсу
        var transform = new TransformGroup();
        transform.Children.Add(new RotateTransform(_viewModel.DroneHeading));
        transform.Children.Add(new TranslateTransform(pos.X, pos.Y));
    
        using (context.PushTransform(transform.Value))
        {
            context.DrawGeometry(
                new SolidColorBrush(Color.FromRgb(80, 250, 123)),  // Зелёный
                new Pen(Brushes.White, 2),
                triangle);
        }
    
        // Подпись "DRONE" — используем TextLayout вместо FormattedText
        var droneText = new TextLayout(
            text: "DRONE",
            typeface: new Typeface("Arial"),
            fontSize: 10,
            foreground: Brushes.White);
    
        droneText.Draw(context, new Point(pos.X - droneText.Width / 2, pos.Y + 15));
    }
    
    /// <summary>
    /// Рисует waypoints
    /// </summary>
    private void DrawWaypoints(DrawingContext context)
    {
        if (_viewModel == null) return;
    
        for (int i = 0; i < _viewModel.Waypoints.Count; i++)
        {
            var wp = _viewModel.Waypoints[i];
            var pos = LatLonToScreen(wp.Latitude, wp.Longitude);
        
            // Круг
            context.DrawEllipse(
                new SolidColorBrush(Colors.Orange),
                new Pen(Brushes.White, 2),
                new Rect(pos.X - 8, pos.Y - 8, 16, 16));
        
            // Номер — используем TextLayout
            var numberText = new TextLayout(
                text: (i + 1).ToString(),
                typeface: new Typeface("Arial"),
                fontSize: 12,
                foreground: Brushes.White);
        
            numberText.Draw(context, new Point(pos.X - numberText.Width / 2, pos.Y - numberText.Height / 2));
        }
    
        // Линия маршрута
        if (_viewModel.Waypoints.Count > 1)
        {
            var points = new List<Point>();
            foreach (var wp in _viewModel.Waypoints)
            {
                points.Add(LatLonToScreen(wp.Latitude, wp.Longitude));
            }
        
            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(points[0], false);  // ← добавили isFilled: false (линия не заполняется)
                for (int i = 1; i < points.Count; i++)
                {
                    ctx.LineTo(points[i]);
                }
            }
        
            context.DrawGeometry(
                null,
                new Pen(new SolidColorBrush(Color.FromRgb(255, 165, 0)), 2) { DashStyle = DashStyle.Dash },
                geometry);
        }
    }
    
    /// <summary>
    /// Рисует перекрестие в центре
    /// </summary>
    private void DrawCrosshair(DrawingContext context)
    {
        var cx = Bounds.Width / 2;
        var cy = Bounds.Height / 2;
        
        context.DrawLine(new Pen(Brushes.Red, 1), new Point(cx - 10, cy), new Point(cx + 10, cy));
        context.DrawLine(new Pen(Brushes.Red, 1), new Point(cx, cy - 10), new Point(cx, cy + 10));
    }
    
    /// <summary>
    /// Конвертирует GPS координаты в экранные
    /// </summary>
    private Point LatLonToScreen(double lat, double lon)
    {
        if (_viewModel == null) return new Point(0, 0);
        
        var zoom = _viewModel.ZoomLevel;
        var centerTileX = LonToTileX(_viewModel.CenterLon, zoom);
        var centerTileY = LatToTileY(_viewModel.CenterLat, zoom);
        var pointTileX = LonToTileX(lon, zoom);
        var pointTileY = LatToTileY(lat, zoom);
        
        var dx = (pointTileX - centerTileX) * TileSize;
        var dy = (pointTileY - centerTileY) * TileSize;
        
        return new Point(Bounds.Width / 2 + dx, Bounds.Height / 2 + dy);
    }
    
    /// <summary>
    /// Конвертирует экранные координаты в GPS
    /// </summary>
    public (double lat, double lon) ScreenToLatLon(Point screen)
    {
        if (_viewModel == null) return (0, 0);
        
        var zoom = _viewModel.ZoomLevel;
        var centerTileX = LonToTileX(_viewModel.CenterLon, zoom);
        var centerTileY = LatToTileY(_viewModel.CenterLat, zoom);
        
        var dx = (screen.X - Bounds.Width / 2) / TileSize;
        var dy = (screen.Y - Bounds.Height / 2) / TileSize;
        
        var tileX = centerTileX + dx;
        var tileY = centerTileY + dy;
        
        var lon = TileXToLon(tileX, zoom);
        var lat = TileYToLat(tileY, zoom);
        
        return (lat, lon);
    }
    
    // Формулы конвертации GPS <-> тайлы OSM
    private static double LonToTileX(double lon, int zoom) => (lon + 180.0) / 360.0 * Math.Pow(2, zoom);
    
    private static double LatToTileY(double lat, int zoom)
    {
        var latRad = lat * Math.PI / 180.0;
        return (1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * Math.Pow(2, zoom);
    }
    
    private static double TileXToLon(double tileX, int zoom) => tileX / Math.Pow(2, zoom) * 360.0 - 180.0;
    
    private static double TileYToLat(double tileY, int zoom)
    {
        var n = Math.PI - 2.0 * Math.PI * tileY / Math.Pow(2, zoom);
        return 180.0 / Math.PI * Math.Atan(Math.Sinh(n));
    }
    
    /// <summary>
    /// Загружает тайл из кэша или с сервера
    /// </summary>
    private Bitmap? GetOrLoadTile(string url)
    {
        if (_tileCache.TryGetValue(url, out var cached))
            return cached;
        
        // Асинхронная загрузка
        _ = LoadTileAsync(url);
        return null;
    }
    
    private async System.Threading.Tasks.Task LoadTileAsync(string url)
    {
        try
        {
            var data = await HttpClient.GetByteArrayAsync(url);
            using var ms = new System.IO.MemoryStream(data);
            var bitmap = new Bitmap(ms);
            _tileCache[url] = bitmap;
            
            Dispatcher.UIThread.Post(InvalidateVisual);
        }
        catch
        {
            // Игнорируем ошибки загрузки
        }
    }
    
    // Обработка событий мыши
    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_viewModel == null) return;
        
        _dragStart = e.GetPosition(this);
        _dragStartCenter = (_viewModel.CenterLat, _viewModel.CenterLon);
        _isDragging = true;
        
        e.Pointer.Capture(this);
        base.OnPointerPressed(e);
    }
    
    protected override void OnPointerMoved(Avalonia.Input.PointerEventArgs e)
    {
        if (!_isDragging || _viewModel == null) return;
        
        var current = e.GetPosition(this);
        var dx = (current.X - _dragStart.X) / TileSize;
        var dy = (current.Y - _dragStart.Y) / TileSize;
        
        var zoom = _viewModel.ZoomLevel;
        var startTileX = LonToTileX(_dragStartCenter.lon, zoom);
        var startTileY = LatToTileY(_dragStartCenter.lat, zoom);
        
        _viewModel.CenterLon = TileXToLon(startTileX - dx, zoom);
        _viewModel.CenterLat = TileYToLat(startTileY - dy, zoom);
        
        base.OnPointerMoved(e);
    }
    
    protected override void OnPointerReleased(Avalonia.Input.PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
        base.OnPointerReleased(e);
    }
    
    protected override void OnPointerWheelChanged(Avalonia.Input.PointerWheelEventArgs e)
    {
        if (_viewModel == null) return;
        
        if (e.Delta.Y > 0)
            _viewModel.ZoomIn();
        else
            _viewModel.ZoomOut();
        
        base.OnPointerWheelChanged(e);
    }
}