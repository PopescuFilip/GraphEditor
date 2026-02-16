using GraphEditor.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphEditor.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var vm = (DataContext as MainViewModel)!;
        var clickPosition = e.GetPosition(sender as UIElement);
        vm.AddNodeCommand.Execute(clickPosition);
        vm.UpdateCanvas(PointsCanvas);
    }
}