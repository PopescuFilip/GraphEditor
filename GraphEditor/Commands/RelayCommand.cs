namespace GraphEditor.Commands;

public class RelayCommand(Action _execute) : CommandBase
{
    public override void Execute(object? _) => _execute();
}

public class RelayCommand<T>(Action<T> _execute, T _argument) : CommandBase
{
    public override void Execute(object? _) => _execute(_argument);

    public override bool CanExecute(object? parameter) => true;
}