using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.Flow
{
    public class FlowCalc
    {
        public List<FlowModel> Flows;

        public Edge[] finalPath;
        private readonly Graph graph;
        public FlowCalc(Graph graph)
        {
            this.graph = graph;
            Flows = new List<FlowModel>();
            foreach (var item in graph.Edges)
            {
                Flows.Add(new FlowModel { Edge = item, Full = false, Flow = 0 });
            }
        }
        public List<FlowModel> FindMaximumFlow()
        {
            List<Edge> path = new List<Edge>();
            bool exit = false;
            do
            {
                Node currentNode = graph.Nodes.First();
                Node startNode = currentNode;
                Edge previousEdge = new Edge();
                while (currentNode.Id != graph.Nodes.Last().Id)
                {
                    FlowModel flow = new FlowModel();
                    var temp = Flows.Where(x => x.Edge.Source == currentNode.Id && !x.Full && !x.Visited).OrderByDescending(x => x.Edge.Weight).FirstOrDefault(x => x.Edge.Destination == graph.Nodes.Last().Id);
                    if (temp != null)
                        flow = temp;
                    else                            //max loaded edge, don't visited, don't full and equal needed node
                        flow = Flows.OrderByDescending(x => x.Edge.Weight).FirstOrDefault(x => !x.Full && !x.Visited && x.Edge.Source == currentNode.Id);
                    if (flow == null)
                    {
                        if (currentNode.Id == graph.Nodes.First().Id)
                        {
                            exit = true;
                            break;
                        }
                        else
                        {
                            Flows.First(x => x.Edge == previousEdge).Full = true;
                            currentNode = startNode;
                        }
                    }
                    else
                    {
                        var edge = flow.Edge;
                        path.Add(edge);
                        Flows.First(x => x.Edge == edge).Visited = true;
                        currentNode = graph.Nodes.FirstOrDefault(x => x.Id == edge.Destination);
                        previousEdge = edge;
                    }
                }
                if (!exit)
                {
                    CalculateMinCapacity(path);
                    // System.Console.WriteLine(path.Sum(x => x.Weight));
                    finalPath = path.ToArray();
                }
                Flows.ForEach(x => x.Visited = false);
                path.Clear();
            } while (!exit);
            var max = CalculateMaximumFlow(graph.Nodes.Last());
            // Console.WriteLine($"Maximum flow: {max}");
            return Flows;
        }

        private void CalculateMinCapacity(List<Edge> path)
        {
            int minCapacity = path.Min(e => e.Weight - Flows.FirstOrDefault(x => x.Edge == e).Flow);//min residual capacities, min from stock weight minus already filled capacity
            foreach (var item in path)
            {
                Flows.Where(f => f.Edge == item).Select(x => { x.Flow = x.Flow + minCapacity; return x; }).ToList();//encrease flow for current path by min capacity
            }
        }
        private int CalculateMaximumFlow(Node node)
        {
            return Flows.Where(e => e.Edge.Destination == node.Id).Sum(x => x.Flow);
        }
    }
}