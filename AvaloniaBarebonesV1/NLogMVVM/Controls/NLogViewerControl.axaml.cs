using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaBarebonesV1.NLogMVVM.Interfaces;
using AvaloniaBarebonesV1.NLogMVVM.ViewModels;

namespace AvaloniaBarebonesV1.NLogMVVM.Controls;

public partial class NLogViewerControl : UserControl
{
    public NLogViewerControl()
    {
        InitializeComponent();
    }
   
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == DataContextProperty)
        {
            if (change.OldValue is NLogViewerControlViewModel oldVm)
            {
                 oldVm.LogEntries.CollectionChanged -= OnCollectionChanged;               
            }

            if (change.NewValue is NLogViewerControlViewModel vm)
            {
                vm.LogEntries.CollectionChanged += OnCollectionChanged;
            }
        }
        base.OnPropertyChanged(change);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if ((AutoScrollToggle.IsChecked ?? false) 
            && sender is IList<StyledLogEntry> logEntries)
        {
            var last = logEntries.LastOrDefault();
            if (last != default)
            {
                Dispatcher.UIThread.Post(() => // see: https://github.com/AvaloniaUI/Avalonia/issues/14415#issuecomment-1920147423
                {
                    MyDataGrid.ScrollIntoView(last, null);
                });           
            }
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is NLogViewerControlViewModel vm)
        {
            vm.LogEntries.CollectionChanged -= OnCollectionChanged;
        }
        
        base.OnDetachedFromVisualTree(e);
    }
}

public struct LogEntryStyle
{
    public FontWeight FontWeight { get; set; }
}

public static class MyConverters 
{
    /// <summary>
    /// Gets a Converter that takes a number as input and converts it into a text representation
    /// </summary>
    public static FuncMultiValueConverter<object, ISolidColorBrush> MyConverter { get; } = 
        new FuncMultiValueConverter<object, ISolidColorBrush> (num =>
        {
            if (num is IList<object> { Count: 2 } objs)
            {
                var logEntry = objs[0] as NLogEntry;
                var vm = objs[1] as NLogViewerControlViewModel;

                return Brushes.Gray; // vm?.LogLevelColors.GetValueOrDefault(logEntry?.Level, Brushes.Black) ?? Brushes.Black;
            }
            
            return Brushes.Green;
        });
}

public class LogEntryStyleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 2 
            && values[0] is NLogEntry logEntry
            && values[1] is NLogViewerControlViewModel vm
            && parameter is string key)
        {

        }

        return new LogEntryStyle()
        {
            FontWeight = FontWeight.Light
        };
    }
}
public class LogLevelToForegroundConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count == 2 
            && values[0] is NLogEntry logEntry
            && values[1] is NLogViewerControlViewModel vm
            && parameter is string key)
        {
           var brushes =
                vm?.LogLevelColors.GetValueOrDefault(logEntry?.Level, NLogViewerControlViewModel.DefaultBrushes) ??
                NLogViewerControlViewModel.DefaultBrushes;

            return key switch
            {
                "foreground" => brushes.Foreground,
                "background" => brushes.Background,
                _ => Brushes.Black
            }; 
        }
                    
        return Brushes.Black;
    }
}