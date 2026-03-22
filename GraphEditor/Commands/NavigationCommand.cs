using GraphEditor.ViewModels;

namespace GraphEditor.Commands;

public class NavigationCommand<T>(MainViewModel _vm): CommandBase where T : ViewModelBase
{
    public override void Execute(object? parameter) => _vm.Navigate<T>();

    public void Execute(Action<T>? configure = null) => _vm.Navigate(configure);
}