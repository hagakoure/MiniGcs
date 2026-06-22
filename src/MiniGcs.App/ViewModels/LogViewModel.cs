using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MiniGcs.App.ViewModels;

public partial class LogViewModel : ObservableObject
{
    public ObservableCollection<string> Messages { get; } = new();
    
    private const int MaxMessages = 200;
    
    public void AddMessage(string message)
    {
        var timestamped = $"[{DateTime.Now:HH:mm:ss}] {message}";
        
        // Добавляем в начало
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Messages.Insert(0, timestamped);
            
            // Ограничиваем количество сообщений
            while (Messages.Count > MaxMessages)
            {
                Messages.RemoveAt(Messages.Count - 1);
            }
        });
    }
    
    public void Clear()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            Messages.Clear();
        });
    }
}