using GraphEditor.Models;
using GraphEditor.ViewModels;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphEditor.Views;

/// <summary>
/// Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : UserControl
{
    private GraphViewModel _vm = null!;

    public MainView()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        _vm = (DataContext as GraphViewModel)!;
    }

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var clickPosition = e.GetPosition(sender as UIElement);
        _vm.AddNodeCommand.Execute(clickPosition);
    }
}