using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.ChinesePostman
{
    public class ChinesePostman
    {
        public static Graph graph = new Graph();
        public static bool IsEvenDegree(Node[] nodes)
        {
            foreach (var node in nodes)
            {
                if (node.Rank % 2 != 0)
                {
                    return false;
                }
            }
            return true;
        }
        private static List<(Node, Node)> GetOddNodesCombinations(List<Node> oddNodes)
        {
            Node firstNode = oddNodes.First();
            int minWeight = int.MaxValue;
            List<(Node, Node)> bestPairs = new List<(Node, Node)>();
            (Node, Node) currentPair;
            if (oddNodes.Count == 2)
            {
                return new List<(Node, Node)>() { (firstNode, oddNodes.Last()) };
            }
            foreach (var node in oddNodes.Skip(1))
            {
                currentPair = (firstNode, node);
                List<(Node, Node)> newPair = GetOddNodesCombinations(oddNodes.Except(new List<Node>() { node, firstNode }).ToList());
                int weight =
                    CalculateWeightOfPairs(newPair);
                if (weight < minWeight)
                {
                    minWeight = weight;
                    bestPairs = new List<(Node, Node)>() { currentPair };
                    bestPairs.AddRange(newPair);
                }
            }

            return bestPairs;
        }
        private static int CalculateWeightOfPairs(List<(Node, Node)> pairs)
        {
            int sum = 0;
            foreach (var pair in pairs)
            {
                sum += ChinesePostman.graph.Edges.FirstOrDefault(x => x.Source == pair.Item1.Id && x.Destination == pair.Item2.Id
                                                                    || x.Destination == pair.Item1.Id && x.Source == pair.Item2.Id).Weight;
            }

            return sum;
        }

        private static List<Edge> CreateEdgeBetweenOddVertices(List<(Node, Node)> pairsOfOddNodes)
        {
            var edges = ChinesePostman.graph.Edges.ToList();
            foreach (var pair in pairsOfOddNodes)
            {
                edges.Add(new Edge()
                {
                    Source = pair.Item1.Id,
                    Destination = pair.Item2.Id,
                    Weight = ChinesePostman.graph.Edges.FirstOrDefault(x => x.Source == pair.Item1.Id && x.Destination == pair.Item2.Id
                                                                        || x.Destination == pair.Item1.Id && x.Source == pair.Item2.Id).Weight
                });
                ChinesePostman.graph.Nodes.First(x => x.Id == pair.Item1.Id).Rank++;
                ChinesePostman.graph.Nodes.First(x => x.Id == pair.Item2.Id).Rank++;
            }
            return edges;
        }
        public static Graph PairingOddVertices(Graph graph, Node[] oddNode)
        {
            ChinesePostman.graph = (Graph)graph.Clone();
            var pairs = GetOddNodesCombinations(oddNode.ToList());
            var newEdges = CreateEdgeBetweenOddVertices(pairs);
            ChinesePostman.graph.Edges = newEdges.ToArray();
            ChinesePostman.graph.EdgesCount = ChinesePostman.graph.Edges.Length;
            return ChinesePostman.graph;
        }

        public static List<Node> FindEulerianPath(Graph graph)
        {
            Stack<Node> nodesStack = new Stack<Node>();
            List<Node> result = new List<Node>();
            nodesStack.Push(graph.Nodes.First());
            var edgesToCompile = graph.Edges.ToList();
            var nodesToCompile = graph.Nodes.ToList();
            while (nodesStack.Count != 0)
            {
                var vNode = nodesStack.Peek();
                if (edgesToCompile.Any(x => x.Source == vNode.Id || x.Destination == vNode.Id))
                {
                     Edge edgeToRemove = new Edge();
                    Node vNodeConnected = new Node();
                    if (edgesToCompile.Any(x => x.Source == vNode.Id))
                    {
                        vNodeConnected = nodesToCompile.First(x => x.Id == edgesToCompile.First(x => x.Source == vNode.Id).Destination);
                        nodesStack.Push(vNodeConnected);
                        edgeToRemove = edgesToCompile.First(x => x.Destination == vNodeConnected.Id && x.Source == vNode.Id);
                    }
                    else if (edgesToCompile.Any(x => x.Destination == vNode.Id))
                    {
                        vNodeConnected = nodesToCompile.First(x => x.Id == edgesToCompile.First(x => x.Destination == vNode.Id).Source);
                        nodesStack.Push(vNodeConnected);
                        edgeToRemove = edgesToCompile.First(x => x.Source == vNodeConnected.Id && x.Destination == vNode.Id);

                    }
                    if (edgeToRemove != null) edgesToCompile.Remove(edgeToRemove);
                }
                else
                {
                    nodesStack.Pop();
                    result.Add(vNode);
                }

            }

            return result;
        }
    }

}