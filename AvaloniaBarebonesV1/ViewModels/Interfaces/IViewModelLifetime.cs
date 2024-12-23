using Avalonia;
using Avalonia.Controls;

namespace AvaloniaBarebonesV1.ViewModels.Interfaces;

public interface IViewModelLifetime
{
    void Loaded(Visual view);
    void Unloaded(Visual view);
}