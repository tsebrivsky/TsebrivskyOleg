using System;
using System.Threading.Tasks;

namespace Graph
{
    public class KruskalAlgorithm
    {
        private static int Find(Subset[] subsets, int i)
        {
            if (subsets[i].Parent != i)
                subsets[i].Parent = Find(subsets, subsets[i].Parent);
            return subsets[i].Parent;
        }

        private static void Union(Subset[] subsets, int x, int y)
        {
            int xroot = Find(subsets, x);
            int yroot = Find(subsets, y);

            if (subsets[xroot].Rank < subsets[yroot].Rank)
                subsets[xroot].Parent = yroot;
            else if (subsets[xroot].Rank > subsets[yroot].Rank)
                subsets[yroot].Parent = xroot;
            else
            {
                subsets[yroot].Parent = xroot;
                subsets[xroot].Rank++;
            }
        }

        public static Graph KruskalSolve(Graph graph)
        {
            int verticesCount = graph.VerticesCount;
            Edge[] edgesArray = new Edge[verticesCount];

            Array.Sort(graph.Edges, (Edge a, Edge b) =>
            {
                return a.Weight.CompareTo(b.Weight);
            });

            Subset[] subsets = new Subset[verticesCount];

            for (int v = 0; v < verticesCount; v++)
                subsets[v] = new Subset() { Parent = v, Rank = 0 };

            int e = default;
            int i = default;
            while (e < verticesCount - 1)
            {
                Edge nextEdge = graph.Edges[i++];
                int x = Find(subsets, nextEdge.Source);
                int y = Find(subsets, nextEdge.Destination);

                if (x != y)
                {
                    if (nextEdge != null)
                        edgesArray[e++] = nextEdge;
                    Union(subsets, x, y);
                }
            }

            return new Graph
            {
                VerticesCount = verticesCount,
                Edges = edgesArray,
                EdgesCount = e,

            };
        }
    }
}