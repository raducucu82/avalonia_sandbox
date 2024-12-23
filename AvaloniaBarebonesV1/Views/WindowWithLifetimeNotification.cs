using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using AvaloniaBarebonesV1.ViewModels.Interfaces;

namespace AvaloniaBarebonesV1.Views;

public class UserControlWithLifetimeNotification: UserControl 
{
     protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
     {
         NotifyLoadedUnloaded.NotifyOnDetachedFromVisualTree(this);
         base.OnDetachedFromVisualTree(e);
     }
 
     protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
     {
         NotifyLoadedUnloaded.NotifyOnDataContextChanged(this, change);
         base.OnPropertyChanged(change);
     }   
     
     // protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
     // {
     //     NotifyLoadedUnloaded.NotifyOnAttachedToVisualTree(this);
     //     base.OnAttachedToVisualTree(e);
     // }

}

public class WindowWithLifetimeNotification : Window
{
    // protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    // {
    //     base.OnAttachedToVisualTree(e);
    //     if (DataContext is IViewModelLifetime lifetime)
    //     {
    //         lifetime.Loaded(this);
    //     }
    // }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        NotifyLoadedUnloaded.NotifyOnDetachedFromVisualTree(this);
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        NotifyLoadedUnloaded.NotifyOnDataContextChanged(this, change);
        base.OnPropertyChanged(change);
    }
}

internal static class NotifyLoadedUnloaded
{
    public static void NotifyOnDetachedFromVisualTree(Visual visual)
    {
          if (visual.DataContext is IViewModelLifetime lifetime)
          {
              lifetime.Unloaded(visual);
          }       
    }

    public static void NotifyOnAttachedToVisualTree(Visual visual)
    {
        if (visual.DataContext is IViewModelLifetime lifetime)
        {
            lifetime.Loaded(visual);
        }
    }

    public static void NotifyOnDataContextChanged(Visual visual, AvaloniaPropertyChangedEventArgs change)
    {
          if (change.Property == StyledElement.DataContextProperty && visual.IsAttachedToVisualTree())
          {
              if (change.OldValue is IViewModelLifetime oldVm)
              {
                  oldVm.Unloaded(visual);
              }
              if (change.NewValue is IViewModelLifetime newVm)
              {
                  newVm.Loaded(visual);
              }
          }       
    }
}
