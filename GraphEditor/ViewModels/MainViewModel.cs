using GraphEditor.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace GraphEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ViewModelBase CurrentViewModel { get; }

    public MainViewModel(IServiceProvider service)
    {
        CurrentViewModel = service.GetRequiredService<GraphViewModel>();
    }
}