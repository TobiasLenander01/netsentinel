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
        collection.AddSingleton<IAppNavigator, AppNavigator>();

        // Register ViewModels
        // We use Transient for the Main Window so it gets a fresh instance on startup
        collection.AddTransient<MainWindowViewModel>();

        // 2. Build the Service Provider
        Services = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            // 3. Resolve the ViewModel
            // Instead of 'new MainWindowViewModel()', we ask the container for it.
            // The container sees the constructor needs IAppNavigator, finds it, and injects it.
            var mainViewModel = Services.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
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