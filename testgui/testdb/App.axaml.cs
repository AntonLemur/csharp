using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using testdb.ViewModels;
using testdb.Views;
using Microsoft.Extensions.DependencyInjection;
using testdb.Data;
using testdb.Services;

namespace testdb;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>();
        services.AddScoped<NavigationService>();
        services.AddScoped<EmployeeService>();
        services.AddScoped<SpecificationService>();
        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<EmployeesViewModel>();
        services.AddTransient<SpecificationsViewModel>();

        var provider = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = provider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}