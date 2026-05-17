using GraphEditor.Algorithms.Models;
using GraphEditor.Models;

namespace GraphEditor.Algorithms.Tagging;

public class FIFOPrefluxAlgorithm : IAlgorithm
{
    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, Action<Graph<ResidualEdge>> onNewResidualGraph) =>
        Run(graph, graph.Nodes.Min(x => x.Number), graph.Nodes.Max(x => x.Number), 0, onNewResidualGraph);

    public (int MaxFlow, Graph ResultingGraph) Run(Graph graph, int startNode, int endNode, int initialFlow,
        Action<Graph<ResidualEdge>> onNewResidualGraph)
    {
        var graphState = GraphState.CreateRange(graph.Edges).ToExcess();
        var residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        var tags = residualGraph.GetDistanceTags(endNode);
        tags[startNode] = graph.Nodes.Length;
        graphState = SetInitialFlow(graphState, startNode);
        residualGraph = graphState.ToResidual();
        onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

        var activeNodes = GetInitialQueue(graphState, endNode);
        while (activeNodes.Count != 0)
        {
            var currentNode = activeNodes.Dequeue();
            var admissibleNodes = residualGraph.GetAdmissibleNodes(currentNode, tags).Shuffled();
            while (graphState.Excess[currentNode] > 0 && admissibleNodes.Count != 0)
            {
                var admissibleNode = admissibleNodes[0];
                var currentKey = (currentNode, admissibleNode);
                var residualEdge = residualGraph.Edges[currentKey];

                var flowToAdd = Math.Min(graphState.Excess.TryGetValueOrDefault(currentNode, 0), residualEdge.ResidualValue);
                graphState = graphState.AddFlow(currentKey, flowToAdd);

                if (!activeNodes.Contains(admissibleNode) &&
                    admissibleNode != startNode &&
                    admissibleNode != endNode)
                    activeNodes.Enqueue(admissibleNode);

                residualGraph = graphState.ToResidual();
                onNewResidualGraph(new Graph<ResidualEdge>(graph.Nodes, [.. residualGraph.Edges.Values]));

                admissibleNodes = residualGraph.GetAdmissibleNodes(currentNode, tags).Shuffled();
            }

            if (graphState.Excess[currentNode] > 0)
            {
                var minDistance = residualGraph.AdjacencyList[currentNode].Select(node => tags[node]).Min();
                tags[currentNode] = minDistance + 1;
                activeNodes.Enqueue(currentNode);
            }
        }

        var maxFlow = graphState.Excess[endNode];
        return (maxFlow, graph with { Edges = [.. graphState.FixFlow().GetEdges()] });
    }

    private ExcessGraphState SetInitialFlow(ExcessGraphState graphState, int startNode)
    {
        var newGraphState = graphState with { };

        foreach (var adjacentNode in graphState.AdjacencyList[startNode])
        {
            var edgeKey = (startNode, adjacentNode);
            var edge = graphState.Edges[edgeKey];
            newGraphState = newGraphState.AddFlow(edge, edge.Capacity);
        }

        return newGraphState;
    }

    private Queue<int> GetInitialQueue(ExcessGraphState graphState, int endNode)
    {
        var activeNodes = graphState.GetActiveNodes(endNode).ToList();
        activeNodes.Sort();

        var queue = new Queue<int>();
        foreach (var node in activeNodes)
            queue.Enqueue(node);

        return queue;
    }
}