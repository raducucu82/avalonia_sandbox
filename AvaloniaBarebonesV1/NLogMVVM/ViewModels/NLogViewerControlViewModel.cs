using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaBarebonesV1.NLogMVVM.Interfaces;
using AvaloniaBarebonesV1.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace AvaloniaBarebonesV1.NLogMVVM.ViewModels;

public record LogEntryBrushes(ISolidColorBrush Foreground, ISolidColorBrush Background);

public partial class NLogViewerControlViewModel : ViewModelBase, ILogTargetObserver
{
    public static LogEntryBrushes DefaultBrushes => new(Brushes.Black, Brushes.Transparent);
    
    public ObservableCollection<StyledLogEntry> LogEntries { get; }

    [ObservableProperty] private int _fontSize;

    [ObservableProperty] private bool _showDebug;

    [ObservableProperty] private FontFamily _fontFamily;

    public readonly Dictionary<LogLevel, LogEntryBrushes> LogLevelColors = new()
    {
        { LogLevel.Debug, new(Brushes.Gray, Brushes.Transparent) },
        { LogLevel.Info, new(Brushes.Black, Brushes.Transparent)},
        { LogLevel.Warn, new(Brushes.DarkGray, Brushes.Yellow)},
        { LogLevel.Error, new(Brushes.White, Brushes.DarkRed)},
        { LogLevel.Fatal, new(Brushes.White, Brushes.OrangeRed)}
    };

    public NLogViewerControlViewModel(ObservableLogTarget? uiLogTarget = null)
    {
        FontSize = 11;
        FontFamily = new("Courier New");
        LogEntries = new();
        
        uiLogTarget?.Register(this);
    }

    public void NewMessage(NLogEntry logMessage)
    {
        if ((logMessage.Level <= LogLevel.Debug) && !ShowDebug)
        {
            return;
        }

        var (foreground, background) = GetBrushes(logMessage.Level);
        Dispatcher.UIThread.Post(() =>
        {
            LogEntries.Add(new()
            {
                Message = logMessage.Message,
                FontWeight = FontWeight.Normal,
                Foreground = foreground,
                Background = background,
                IsDebug = logMessage.Level <= LogLevel.Debug
            }) ;
        });
    }

    private LogEntryBrushes GetBrushes(LogLevel? logLevel)
        => LogLevelColors.GetValueOrDefault(logLevel, DefaultBrushes);
}

public class StyledLogEntry
{
    public string Message { get; set; } = string.Empty;
    public FontWeight FontWeight { get; set; }
    public ISolidColorBrush Foreground { get; set; } = Brushes.Black;
    public ISolidColorBrush Background { get; set; } = Brushes.Transparent;
    public bool IsDebug { get; set; }
}


