namespace GraphEditor.Models;

public record SimpleEdge(int StartNode, int EndNode);

public record ResidualEdge(int StartNode, int EndNode, int ResidualValue) : SimpleEdge(StartNode, EndNode);

public record FlowEdge(int StartNode, int EndNode, int Flow, int Capacity) : SimpleEdge(StartNode, EndNode);