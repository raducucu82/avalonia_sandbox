using System;
using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaBarebonesV1.NLogMVVM.ViewModels;
using AvaloniaBarebonesV1.Services.Interfaces;
using AvaloniaBarebonesV1.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;

namespace AvaloniaBarebonesV1.ViewModels;

public partial class MainWindowViewModel : ObservableViewModelBase, IViewModelLifetime
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private string? _greeting;
    private IDisposable? _subscription = null;

    [ObservableProperty] private int _sliderVal;
    
    public NLogViewerControlViewModel? NlogVM { get; }
    
    public MainWindowViewModel() {}

    public MainWindowViewModel(IHelloWorldService helloWorldService, NLogViewerControlViewModel? nlogVM)
    {
        _greeting = helloWorldService.HelloWorld;
        UpdateGreetingCommand = new RelayCommand(UpdateGreeting);
        
        NlogVM = nlogVM;
        SliderVal = 5;
        
        Logger.Info("Greetings!");
    }

    public string? Greeting
    {
        get => _greeting;
        set => SetProperty(ref _greeting, value);
    }

    public IRelayCommand? UpdateGreetingCommand { get; }

    private void UpdateGreeting()
    {
        Greeting = "Button Clicked!";
        
        var logLevels = new List<LogLevel> { LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal };
        var rg = new Random();
                
        Logger.Log(logLevels[rg.Next(logLevels.Count)], "Button Clicked!Button ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton ClickedButton Clicked!!!!!!!!!!!!!!!!!!!!!!!! ");
    }

    public void Loaded(Control view)
    {
        Logger.Info("VM Loaded.");
        _subscription = ObservePropertyChangedEvent()
            .Subscribe(args =>
            {
                // Console.WriteLine($" -> {args.PropertyName}");
                Logger.Debug($"{args.PropertyName} changed.");
            });
    }

    public void Unloaded(Control view)
    {
        Logger.Info("VM Unloaded");
        _subscription?.Dispose();
    }
}