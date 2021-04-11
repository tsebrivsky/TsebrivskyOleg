using System;
using System.Linq;

namespace Graph
{
    public class Graph : ICloneable
    {
        public int VerticesCount { get; set; }
        public int EdgesCount { get; set; }
        public Edge[] Edges { get; set; }
        public Node[] Nodes { get; set; }

        public object Clone()
        {
            return new Graph
            {
                VerticesCount = this.VerticesCount,
                EdgesCount = this.EdgesCount,
                Edges = this.Edges.Select(x => (Edge)x.Clone()).ToArray(),
                Nodes = this.Nodes.Select(x => (Node)x.Clone()).ToArray()
            };
        }
    }
}