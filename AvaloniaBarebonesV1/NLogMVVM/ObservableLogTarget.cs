using System.Collections.Concurrent;
using System.Linq;
using Autofac;
using AvaloniaBarebonesV1.NLogMVVM.Interfaces;
using NLog;
using NLog.Targets;

namespace AvaloniaBarebonesV1.NLogMVVM;

// https://github.com/NLog/NLog/wiki/How-to-write-a-custom-target
[Target("UiLog")]
public class ObservableLogTarget : TargetWithLayout
{
    private readonly ConcurrentBag<ILogTargetObserver> _observers;

    public ObservableLogTarget()
    {
        Name = "UiLogTarget";
        _observers = new();
        // Layout = NLog.Layouts.Layout.FromString("{time}");
    }

    public void Register(ILogTargetObserver observer)
    {
        _observers.Add(observer);
    }

    protected override void Write(LogEventInfo logEvent)
    {
        base.Write(logEvent);

        var message = RenderLogEvent(this.Layout, logEvent);
        foreach (var observer in _observers)
        {
            observer.NewMessage(new NLogEntry()
            {
                Level = logEvent.Level,
                Message = message
            });
        }
    }

    public static void RegisterConfiguredTarget(ContainerBuilder builder)
    {
        if (LogManager.Configuration.AllTargets.FirstOrDefault(t => t is ObservableLogTarget) is ObservableLogTarget uiLogTarget)
        {
            builder.RegisterInstance<ObservableLogTarget>(uiLogTarget);
        }
    }
}