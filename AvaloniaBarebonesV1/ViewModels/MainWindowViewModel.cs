using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using AvaloniaBarebonesV1.NLogMVVM.ViewModels;
using AvaloniaBarebonesV1.Services.Interfaces;
using AvaloniaBarebonesV1.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using NLog.Fluent;

namespace AvaloniaBarebonesV1.ViewModels;

public partial class MainWindowViewModel : ObservableViewModelBase, IViewModelLifetime
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private string? _greeting;
    private IDisposable? _subscription = null;

    [ObservableProperty] private int _sliderVal;
    [ObservableProperty] private int _result = 0;
    
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

    public void Loaded(Visual view)
    {
        Logger.Info("VM Loaded.");
        _subscription = ObservePropertyChangedEvent()
            .Subscribe(args =>
            {
                Logger.Debug($"{args.PropertyName} changed.");
            });

        if (false)
        {
            DoOnPropertyChangedEvent(
                nameof(SliderVal),
                TimeSpan.FromMilliseconds(200), 
                async ct =>
                {
                    Result = await GetResultLongRunning(SliderVal, ct);
                },
                Logger);
        }
        else
        {
            // Pass the result along the chain. Overly complex, for show.
            ObserveOnPropertyChangedWithCancel(
                    nameof(SliderVal),
                    TimeSpan.FromMilliseconds(500)).
                SelectMany(ct =>
                {
                    Logger.Debug("Passed throttle");
                    return Observable.FromAsync(async () =>
                    {
                        try
                        {
                            return (r: await GetResultLongRunning(SliderVal, ct), ct);
                        }
                        catch (OperationCanceledException)
                        {
                        }
 
                        return (r: default, ct);
                    });
                }).
                Where(tuple => (!tuple.ct.IsCancellationRequested) && (tuple.r != default)).
                Select(tuple => tuple.r).
                ObserveOn(AvaloniaScheduler.Instance).
                Subscribe(result =>
                {
                    Result = result;
                });
        }
    }

    public void Unloaded(Visual view)
    {
        Logger.Info("VM Unloaded");
        _subscription?.Dispose();
    }

    private async Task<int> GetResultLongRunning(int val, CancellationToken ct)
    {
        Logger.Info($"Started for {val}");
        try
        {
            var result = val * 2;
            await Task.Delay(3000, ct);
            Logger.Info($"Done for {val}: {result}");

            return result;
        }
        catch (OperationCanceledException e)
        {
            Logger.Info($"Canceled for {val}");
            throw;
        }
    }
}