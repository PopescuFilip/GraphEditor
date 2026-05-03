using GraphEditor.Algorithms.Models;
using GraphEditor.Models;
using System.Collections.Immutable;

namespace GraphEditor.Algorithms.Tagging;

public static class TaggingAlgorithm
{
    public static IReadOnlyDictionary<int, int> GetDistanceTags(this GraphState<ResidualEdge> graphState, int startNode)
    {
        var tags = new Dictionary<int, int>
        {
            [startNode] = 0
        };

        var seenNodes = new List<int>() { startNode };
        var analysedNodes = new List<int>();
        var nodesToAnalyse = new Queue<int>();
        nodesToAnalyse.Enqueue(startNode);

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