using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Tagging;

public static class TaggingHelper
{
    public static IEnumerable<int> GetAdmissibleEdges(this GraphState<ResidualEdge> graphState, int fromNode, IReadOnlyDictionary<int, int> tags) =>
        graphState.AdjacencyList.TryGetValue(fromNode, out var adjacencyList)
        ? adjacencyList.Where(toNode => IsAdmissible((fromNode, toNode), tags))
        : [];

    public static bool IsAdmissible(this (int, int) edge, IReadOnlyDictionary<int, int> tags) =>
        tags[edge.Item1] == tags[edge.Item2] + 1;

    public static Dictionary<int, int> GetDistanceTags(this GraphState<ResidualEdge> graphState, int endNode)
    {
        var tags = new Dictionary<int, int>
        {
            [endNode] = 0
        };

        var seenNodes = new List<int>() { endNode };
        var analysedNodes = new List<int>();
        var nodesToAnalyse = new Queue<int>();
        nodesToAnalyse.Enqueue(endNode);

        while (analysedNodes.Count != 0)
        {
            var currentNode = nodesToAnalyse.Dequeue();
            analysedNodes.Add(currentNode);

            if (!graphState.AdjacencyList.TryGetValue(currentNode, out var adjacentNodes))
                continue;

            var undiscoveredNodes = adjacentNodes
                .Where(x => !analysedNodes.Contains(x) && !seenNodes.Contains(x))
                .ToImmutableArray();
            if (undiscoveredNodes.Length != 0)
            {
                undiscoveredNodes.Sort();

                var currentTag = tags[currentNode];
                foreach (var undiscoveredNode in undiscoveredNodes)
                {
                    tags[undiscoveredNode] = currentTag + 1;
                    seenNodes.Add(currentTag);
                    nodesToAnalyse.Enqueue(undiscoveredNode);
                }
            }
        }

        return tags;
    }
}