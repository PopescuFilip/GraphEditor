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
    public const int NodeSize = 30;
    private MainViewModel _vm = null!;

    public MainView()
    {
        InitializeComponent();
    }

    //private void UserControl_Loaded(object sender, RoutedEventArgs e)
    //{
    //    _vm = (DataContext as MainViewModel)!;
    //    _vm.Nodes.CollectionChanged += UpdateCanvas;
    //}

    //private void UpdateCanvas(object? sender, NotifyCollectionChangedEventArgs e)
    //{
    //    switch (e.Action)
    //    {
    //        case NotifyCollectionChangedAction.Add:
    //            var newNode = e.NewItems!.OfType<Node>().Single();
    //            PointsCanvas.Children.Add(new CanvasNodeView(newNode));
    //            break;
    //        default:
    //            break;
    //    }
    //}

    private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var clickPosition = e.GetPosition(sender as UIElement);
        (DataContext as MainViewModel)!.AddNodeCommand.Execute(clickPosition);
    }
}