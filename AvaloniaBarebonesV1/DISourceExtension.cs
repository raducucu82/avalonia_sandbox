using System;
using Autofac;
using Avalonia.Markup.Xaml;

namespace AvaloniaBarebonesV1;

// See: https://docs.avaloniaui.net/docs/concepts/markupextensions
// Code base: https://community.devexpress.com/blogs/wpf/archive/2022/02/07/dependency-injection-in-a-wpf-mvvm-application.aspx 
// Also check: https://docs.avaloniaui.net/docs/basics/data/data-binding/data-binding-syntax
// Avalonia DI: https://docs.avaloniaui.net/docs/guides/implementation-guides/how-to-implement-dependency-injection
// Other links:
// https://reference.avaloniaui.net/api/Avalonia.Markup.Xaml/MarkupExtension/
// https://github.com/AvaloniaUI/Avalonia/issues/2121
// https://www.reactiveui.net/docs/handbook/dependency-inversion/custom-dependency-inversion#set-the-locator.current-to-your-implementation
// https://github.com/AvaloniaUI/Avalonia/issues/2554

public class DISourceExtension : MarkupExtension
{
    public static IContainer? Container { private get; set; }
    public Type? Type { get; set; }

    public override object ProvideValue(IServiceProvider _)
    {
        using var scope = Container?.BeginLifetimeScope();
        if (Type == null)
        {
            return default!;
        }

        return scope?.Resolve(Type) ?? default!;
    }
}