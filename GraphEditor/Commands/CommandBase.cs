using System.Windows.Input;

namespace GraphEditor.Commands;

public interface ICommand<T> : ICommand
{
    void Execute(T parameter);
}

public abstract class CommandBase : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public virtual bool CanExecute(object? parameter) => true;

    public abstract void Execute(object? parameter);

    public void Execute() => Execute(null);

    public void OnCanExecutedChanged()
    {
        CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}

public abstract class CommandBase<T> : CommandBase, ICommand<T>
{
    public abstract void Execute(T parameter);

    public override void Execute(object? parameter) => Execute((T)parameter!);
}