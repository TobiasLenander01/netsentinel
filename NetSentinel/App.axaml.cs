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
using NetSentinel.Services;

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
        // SETUP DEPENDENCY INJECTION

        // 1. Create a Service Collection and register services and viewmodels
        var collection = new ServiceCollection();
        collection.AddSingleton<IAppNavigator, NavigationService>(sp => new NavigationService(sp));
        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<DashboardViewModel>();
        collection.AddTransient<EmployeesViewModel>();
        collection.AddTransient<SettingsViewModel>();

        // 2. Build the Service Provider
        Services = collection.BuildServiceProvider();

        // 3. Use the service provider to create the main window and set its DataContext
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow { DataContext = Services.GetRequiredService<MainWindowViewModel>() };
            Services.GetRequiredService<IAppNavigator>().GoToDashboard();
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