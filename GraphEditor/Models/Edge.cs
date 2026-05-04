namespace GraphEditor.Models;

public record SimpleEdge(int StartNode, int EndNode);

public record ResidualEdge(int StartNode, int EndNode, int ResidualValue) : SimpleEdge(StartNode, EndNode);

public record FlowEdge(int StartNode, int EndNode, int Flow, int Capacity) : SimpleEdge(StartNode, EndNode);

public record ExcessEdge(int StartNode, int EndNode, int Flow, int Capacity, int Excess) : FlowEdge(StartNode, EndNode, Flow, Capacity)
{
    public ExcessEdge(FlowEdge flowEdge) : this(flowEdge.StartNode, flowEdge.EndNode, flowEdge.Flow, flowEdge.Capacity, 0)
    {}
}