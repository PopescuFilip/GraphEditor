using System.Collections.ObjectModel;

namespace GraphEditor.ViewModels;

public class EditEdgesViewModel(GraphViewModel _graphViewModel) : ViewModelBase
{
    public ObservableCollection<EdgeViewModel> Edges { get; private set; } = _graphViewModel.Edges;
}