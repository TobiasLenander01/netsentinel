using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;
using NetSentinel.Interfaces;
using NetSentinel.ViewModels;
using NetSentinel.Views;

namespace NetSentinel;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 1. Setup Dependency Injection
        var collection = new ServiceCollection();

        // Register Services (The Singletons)
        collection.AddSingleton<IAppNavigator, NavigationService>();

        // Register ViewModels
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<EmployeesViewModel>();

        // 2. Build the Service Provider
        Services = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            // 3. Resolve the ViewModel for the MainWindow and set it as the DataContext
            var mainWindowViewModel = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    // Helper method included in the default Avalonia project templates to prevent duplicate or conflicting data validation.
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // 1. Find the specific plugin that handles Data Annotations
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // 2. Remove it from Avalonia's list of active validators
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}