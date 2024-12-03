using AvaloniaBarebonesV1.Services.Interfaces;

namespace AvaloniaBarebonesV1.Services;

public class HelloWorldService : IHelloWorldService
{
    public string? HelloWorld => "Hello, World! by Avalonia + Autofac";
}