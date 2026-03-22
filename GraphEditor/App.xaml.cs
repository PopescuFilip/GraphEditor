using GraphEditor.Store;
using GraphEditor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Navigation;

namespace GraphEditor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceProvider = CreateServiceProvider();

        MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();

        base.OnStartup(e);
    }

    private IServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddSingleton(s => new MainWindow()
            {
                DataContext = s.GetRequiredService<MainViewModel>()
            })
            .AddSingleton<MainViewModel>()
            .AddSingleton<GraphViewModel>()
            .AddSingleton<ReadOnlyGraphViewModel>()
            .AddSingleton<GraphHistoryViewModel>()
            .AddSingleton<ResidualGraphStore>()
            .BuildServiceProvider();
    }
}