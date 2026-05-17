using GraphEditor.Algorithms;
using GraphEditor.Algorithms.Tagging;
using GraphEditor.Commands;
using GraphEditor.Models;
using GraphEditor.Store;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using AhujaSimple = GraphEditor.Algorithms.AhujaOrlinAlgorithm;
using AhujaTagging = GraphEditor.Algorithms.Tagging.AhujaOrlinAlgorithm;

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

    public RelayCommand Save { get; }
    public ICommand NewGraph { get; }
    public ICommand Open { get; }
    public RelayCommand Delete { get; }
    public ICommand ExecuteGenericAlgorithm { get; }
    public ICommand ExecuteFordFulkersonAlgorithm { get; }
    public ICommand ExecuteEdmondsKarpAlgorithm { get; }
    public ICommand ExecuteAhujaOrlinAlgorithm { get; }
    public ICommand ExecuteGabowAlgorithm { get; }
    public ICommand ExecuteTagAhujaOrlin { get; }
    public ICommand ExecuteStratified { get; }
    public ICommand ExecutePrefluxGeneric { get; }
    public ICommand ExecutePrefluxFIFO { get; }
    public ICommand ExecutePrefluxBiggestTag { get; }
    public ICommand ViewHistory { get; }

    public MainViewModel(IServiceProvider service, ResidualGraphStore graphStore)
    {
        _service = service;
        CurrentViewModel = _service.GetRequiredService<GraphViewModel>();
        Save = new RelayCommand(SaveGraph, () => CurrentViewModel is GraphViewModel);
        Open = new RelayCommand(OpenGraph);
        NewGraph = new RelayCommand(ClearGraph);
        Delete = new RelayCommand(
            () => _service.GetRequiredService<GraphViewModel>().Delete.Execute(null),
            () => CurrentViewModel is GraphViewModel);
        ViewHistory = new NavigationCommand<GraphHistoryViewModel>(this, (g) => g.Reset());

        var navigateCommand = new NavigationCommand<ReadOnlyGraphViewModel>(this);
        ExecuteGenericAlgorithm = new AlgorithmCommand(GetGraph, new GenericAlgorithm(), navigateCommand, graphStore);
        ExecuteFordFulkersonAlgorithm = new AlgorithmCommand(GetGraph, new FordFulkersonAlgorithm(), navigateCommand, graphStore);
        ExecuteEdmondsKarpAlgorithm = new AlgorithmCommand(GetGraph, new EdmondsKarpAlgorithm(), navigateCommand, graphStore);
        ExecuteAhujaOrlinAlgorithm = new AlgorithmCommand(GetGraph, new AhujaSimple(), navigateCommand, graphStore);
        ExecuteGabowAlgorithm = new AlgorithmCommand(GetGraph, new GabowAlgorithm(), navigateCommand, graphStore);
        ExecuteTagAhujaOrlin = new AlgorithmCommand(GetGraph, new AhujaTagging(), navigateCommand, graphStore);
        ExecuteStratified = new AlgorithmCommand(GetGraph, new StratifiedAlgorithm(), navigateCommand, graphStore);
        ExecutePrefluxGeneric = new AlgorithmCommand(GetGraph, new GenericPrefluxAlgorithm(), navigateCommand, graphStore);
        ExecutePrefluxFIFO = new AlgorithmCommand(GetGraph, new FIFOPrefluxAlgorithm(), navigateCommand, graphStore);
        //ExecutePrefluxBiggestTag = new AlgorithmCommand(GetGraph, new(), navigateCommand, graphStore);
        PropertyChanged += CurrentViewModelChangedHandler;
    }

    private void OpenGraph()
    {
        var dialog = new OpenFileDialog()
        {
            DefaultDirectory = Environment.ExpandEnvironmentVariables("C:\\Users\\%username%\\Desktop"),
            DefaultExt = "json",
            Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Open Graph from json"
        };
        var answer = dialog.ShowDialog();

        if (answer == false)
            return;

        using var fileStream = new FileStream(dialog.FileName, FileMode.Open);
        var graph = JsonSerializer.Deserialize<Graph>(fileStream);
        if (graph == null)
        {
            MessageBox.Show("Invalid file");
            return;
        }

        Navigate<GraphViewModel>((x) => x.InitializeFrom(graph));
    }

    private void ClearGraph()
    {
        Navigate<GraphViewModel>((x) =>
        {
            x.Edges.Clear();
            x.Nodes.Clear();
        });
    }

    private void SaveGraph()
    {
        var graph = GetGraph();

        var saveFileDialog = new SaveFileDialog()
        {
            DefaultDirectory = Environment.ExpandEnvironmentVariables("C:\\Users\\%username%\\Desktop"),
            DefaultExt = ".json",
            Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
            Title = "Save Graph Data"
        };

        var answer = saveFileDialog.ShowDialog();

        if (answer == false)
            return;

        var pathFile = saveFileDialog.FileName;
        var document = JsonSerializer.SerializeToDocument(graph);
        using var sw = new FileStream(pathFile, FileMode.Create);
        using var jsonWriter = new Utf8JsonWriter(sw);
        document.WriteTo(jsonWriter);
    }

    public void Navigate<T>(Action<T>? configure = null) where T : ViewModelBase
    {
        var newViewModel = _service.GetRequiredService<T>();
        if (configure is not null)
        {
            configure(newViewModel);
        }
        CurrentViewModel = newViewModel;
    }

    private Graph GetGraph()
    {
        var graphVM = _service.GetRequiredService<GraphViewModel>();

        var nodes = graphVM.Nodes.Select(n => new Node(n.Number, n.Position));
        var edges = graphVM.Edges.Select(x => new FlowEdge(x.StartNode.Number, x.EndNode.Number, x.Flow, x.Capacity));
        return new Graph([.. nodes], [.. edges]);
    }

    private void CurrentViewModelChangedHandler(object? _, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CurrentViewModel))
        {
            Save.OnCanExecutedChanged();
            Delete.OnCanExecutedChanged();
        }
    }
}