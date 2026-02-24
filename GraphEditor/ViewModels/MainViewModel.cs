using GraphEditor.Commands;
using GraphEditor.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

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

    public ICommand Save { get; }
    public ICommand NewGraph { get; }
    public ICommand Open { get; }
    public ICommand Delete { get; }

    public MainViewModel(IServiceProvider service)
    {
        _service = service;
        CurrentViewModel = _service.GetRequiredService<GraphViewModel>();
        Save = new RelayCommand(SaveGraph);
        Open = new RelayCommand(OpenGraph);
        NewGraph = new RelayCommand(ClearGraph);
        Delete = new RelayCommand(() => _service.GetRequiredService<GraphViewModel>().Delete.Execute(null));
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

        var graphVM = _service.GetRequiredService<GraphViewModel>();
        graphVM.InitializeFrom(graph);
    }

    private void ClearGraph()
    {
        var graphVM = _service.GetRequiredService<GraphViewModel>();
        graphVM.Edges.Clear();
        graphVM.Nodes.Clear();
    }

    private void SaveGraph()
    {
        var graphVM = _service.GetRequiredService<GraphViewModel>();

        var nodes = graphVM.Nodes.Select(n => new Node(n.Number, n.Position));
        var edges = graphVM.Edges.Select(x => new Edge(x.StartNode.Number, x.EndNode.Number, x.Flow, x.Capacity));

        var graph = new Graph([.. nodes], [.. edges]);

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
        using var sw = new FileStream(pathFile, FileMode.OpenOrCreate);
        using var jsonWriter = new Utf8JsonWriter(sw);
        document.WriteTo(jsonWriter);
    }

    public void Navigate<T>() where T : ViewModelBase =>
        CurrentViewModel = _service.GetRequiredService<T>();
}