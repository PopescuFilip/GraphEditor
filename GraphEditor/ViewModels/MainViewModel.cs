using GraphEditor.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace GraphEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider _service;

    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

    public MainViewModel(IServiceProvider service)
    {
        _service = service;
        CurrentViewModel = _service.GetRequiredService<GraphViewModel>();
    }

    public void Navigate<T>() where T : ViewModelBase =>
        CurrentViewModel = _service.GetRequiredService<T>();
}