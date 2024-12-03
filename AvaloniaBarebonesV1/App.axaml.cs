using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaBarebonesV1.NLogMVVM;
using AvaloniaBarebonesV1.NLogMVVM.ViewModels;
using AvaloniaBarebonesV1.Services;
using AvaloniaBarebonesV1.Services.Interfaces;
using AvaloniaBarebonesV1.ViewModels;
using AvaloniaBarebonesV1.Views;
using NLog;

namespace AvaloniaBarebonesV1;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        SetupNLog();

        // Inject Autofac here, see: https://docs.avaloniaui.net/docs/guides/implementation-guides/how-to-implement-dependency-injection
        // https://autofac.readthedocs.io/en/latest/getting-started/index.html#structuring-the-application
        var container = SetupServices();
        // https://community.devexpress.com/blogs/wpf/archive/2022/02/07/dependency-injection-in-a-wpf-mvvm-application.aspx
        DISourceExtension.Container = container;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = new MainWindow
            {
                // Nothing here, Autofac will resolve via MarkupExtension
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void SetupNLog()
    {
         // https://github.com/NLog/NLog/wiki/Register-your-custom-component
         NLog.LogManager.Setup().SetupExtensions(ext => {
             ext.RegisterTarget<ObservableLogTarget>();
         });
                         
         // https://github.com/NLog/NLog/wiki/Configure-from-code
         // https://github.com/NLog/NLog/wiki/Tutorial
         NLog.LogManager.Setup().LoadConfiguration(builder =>
         {
             builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToConsole(
                 layout: "${time} [${level:upperCase=true}] ${message}"
             );
             // builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: "file.txt");
             var uiTarget = new ObservableLogTarget()
             {
                 Layout = "${time} [${level:upperCase=true}] ${message}"
             };
             builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteTo(uiTarget);
         });
         // https://nlog-project.org/2009/09/02/routing-system-diagnostics-trace-and-system-diagnostics-tracesource-logs-through-nlog.html
         System.Diagnostics.Trace.Listeners.Clear();
         System.Diagnostics.Trace.Listeners.Add(new NLog.NLogTraceListener { Name = "NLog" });
    }

    private static IContainer SetupServices()
    {
         var builder = new ContainerBuilder();
         
         // ViewModels
         builder.RegisterType<MainWindowViewModel>().AsSelf();
         builder.RegisterType<NLogViewerControlViewModel>().AsSelf();
         
         // Services
         builder.RegisterType<HelloWorldService>().As<IHelloWorldService>();
         
         // NLog UI
         ObservableLogTarget.RegisterConfiguredTarget(builder);
         
         return builder.Build();
    }
}