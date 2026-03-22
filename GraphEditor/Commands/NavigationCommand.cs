using GraphEditor.ViewModels;

namespace GraphEditor.Commands;

public class NavigationCommand<T>(MainViewModel _vm, Action<T>? configure = null) : CommandBase where T : ViewModelBase
{
    public Action<T>? Configure { get; set; } = configure;

    public override void Execute(object? parameter) => _vm.Navigate(Configure);
}