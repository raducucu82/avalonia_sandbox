using Avalonia.Controls;

namespace AvaloniaBarebonesV1.ViewModels.Interfaces;

public interface IViewModelLifetime
{
    void Loaded(Control view);
    void Unloaded(Control view);
}