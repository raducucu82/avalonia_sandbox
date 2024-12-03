using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace AvaloniaBarebonesV1.ViewModels;

public class ObservableViewModelBase : ViewModelBase
{
    public IObservable<PropertyChangedEventArgs> ObservePropertyChangedEvent()
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