using Avalonia;
using Avalonia.Controls;
using AvaloniaBarebonesV1.ViewModels.Interfaces;

namespace AvaloniaBarebonesV1.Views;

public class WindowWithLifetimeNotification : Window
{
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataContext is IViewModelLifetime lifetime)
        {
            lifetime.Loaded(this);
        }
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is IViewModelLifetime lifetime)
        {
            lifetime.Unloaded(this);
        }
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DataContextProperty)
        {
            if (change.OldValue is IViewModelLifetime oldVm)
            {
                oldVm.Unloaded(this);
            }
            if (change.NewValue is IViewModelLifetime newVm)
            {
                newVm.Loaded(this);
            }
        }
        base.OnPropertyChanged(change);
    }
}