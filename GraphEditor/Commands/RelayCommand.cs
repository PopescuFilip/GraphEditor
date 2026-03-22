namespace GraphEditor.Commands;

public class RelayCommand(Action _execute, Func<bool>? _canExecute = null) : CommandBase
{
    public override void Execute(object? _) => _execute();

    public override bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
}

public class RelayCommand<T>(Action<T> _execute) : CommandBase<T>
{
    public override void Execute(T parameter) => _execute(parameter);
}