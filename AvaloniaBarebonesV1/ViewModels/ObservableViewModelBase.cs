using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace AvaloniaBarebonesV1.ViewModels;

public class ObservableViewModelBase : ViewModelBase
{
    private readonly Dictionary<string, CancellationTokenSource> _propertyNamesToCts;

    protected ObservableViewModelBase()
    {
        _propertyNamesToCts = new();
    }

    protected IObservable<CancellationToken> ObserveOnPropertyChangedWithCancel(string propertyName, TimeSpan throttle)
    {
        return ObservePropertyChangedEvent()
            .Where(arg => arg.PropertyName == propertyName)
            .Throttle(throttle)
            .Select(arg => WithCancellation(arg.PropertyName!).ct);
    }

    protected IObservable<CancellationToken> DoOnPropertyChangedEvent(string propertyName, TimeSpan throttle,
        Func<CancellationToken, Task> action, ILogger logger)
    {
        var observable = 
            ObserveOnPropertyChangedWithCancel(propertyName, throttle) 
            .SelectMany(async ct =>
            {
                try
                {
                    await action(ct);
                }
                catch (OperationCanceledException _)
                {
                    logger.Debug("Canceled");
                }

                return ct;
            })
            .Replay();
        observable.Connect();

        return observable;
    }

    protected (string name, CancellationToken ct) WithCancellation(string propertyName)
    {
        if (_propertyNamesToCts.TryGetValue(propertyName, out var cts))
        {
            cts.Cancel();
        }
        _propertyNamesToCts[propertyName] = new CancellationTokenSource();

        return (propertyName, _propertyNamesToCts[propertyName].Token);
    }

    protected IObservable<PropertyChangedEventArgs> ObservePropertyChangedEvent()
    {
        return
            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    addHandler: h => this.PropertyChanged += h,
                    removeHandler: h =>
                    {
                        this.PropertyChanged -= h;
                    })
                .Select(ep => ep.EventArgs)
                .Publish()
                .RefCount();
    }   
}