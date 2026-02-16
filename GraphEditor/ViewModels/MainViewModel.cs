namespace GraphEditor.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string MyString => "valuee";

    public ViewModelBase CurrentViewModel => this;

    public MainViewModel()
    {
    }
}