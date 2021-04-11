using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.Salesman_problem
{
    public class BnB
    {
        // public static Graph _graph;
        public static List<Edge> _edges;

        public static int minCost;

        public static int _cost;

        public BnB()
        {
            _edges = new List<Edge>();
            minCost = int.MaxValue;
            _cost = default;
        }

        public static Edge[] BranchAndBound(Graph graph)
        {
            bool noValues = true;

            for (int i = 0; i < graph.Edges.Length; i++)
            {
                var minForRow = FindMinWeightInRow(graph.Edges, i);
                if (minForRow != int.MaxValue)
                {
                    graph.Edges = SubstractFromRow(graph.Edges, i, minForRow);
                    _cost += minForRow;
                    noValues = false;
                }
            }
            for (int i = 0; i < graph.Edges.Length; i++)
            {
                var minForCol = FindMinWeightInCol(graph.Edges, i);
                if (minForCol != int.MaxValue)
                {
                    graph.Edges = SubstractFromCol(graph.Edges, i, minForCol);
                    _cost += minForCol;
                    noValues = false;
                }
            }

            if (!noValues)
            {
                graph.Edges = RemoveMaxCoef(graph.Edges);
            }
            else
            {
                _cost = graph.Edges.Sum(x => x.Weight);
                return graph.Edges;
            }

            if (_cost < minCost)
                BranchAndBound(graph);

            return graph.Edges;
        }

        private static Edge[] RemoveMaxCoef(Edge[] edges)
        {
            int maxCoefForRow = default;
            int maxCoefForCol = default;
            int maxCoef = default;

            foreach (var item in edges)
            {
                int coef = default;
                if (item.Weight == 0)
                {
                    coef = CalculateCoeficient(edges, item.Source, item.Destination);
                    if (coef >= maxCoef)
                    {
                        maxCoefForRow = item.Source;
                        maxCoefForCol = item.Destination;
                        maxCoef = coef;
                    }
                }
            }
            foreach (var item in edges)
            {
                if (item.Source == maxCoefForRow)
                    item.Weight = -1;
            }
            foreach (var item in edges)
            {
                if (item.Destination == maxCoefForCol)
                    item.Weight = -1;
            }
            _edges.Add(new Edge()
            {
                Source = maxCoefForRow,
                Destination = maxCoefForCol,
                Weight = edges.First(x => x.Source == maxCoefForRow && x.Destination == maxCoefForCol).Weight
            });


            return edges;
        }

        private static int CalculateCoeficient(Edge[] edges, int source, int destination)
        {
            int minForRow = int.MaxValue;
            int minForCol = int.MaxValue;

            foreach (var item in edges)
            {
                if (item.Source != source && item.Weight != 0 && item.Weight < minForCol && item.Weight != -1)
                {
                    minForCol = item.Weight;
                }
            }
            foreach (var item in edges)
            {
                if (item.Destination != destination && item.Weight != 0 && item.Weight < minForRow && item.Weight != -1)
                {
                    minForRow = item.Weight;
                }
            }

            if (minForCol == int.MaxValue)
            {
                minForCol = 0;
            }
            if (minForRow == int.MaxValue)
            {
                minForRow = 0;
            }

            return minForCol + minForRow;
        }

        private static Edge[] SubstractFromRow(Edge[] edges, int row, int minForRow)
        {
            foreach (var item in edges)
            {
                if (item.Source == row)
                    item.Weight -= minForRow;
            }
            return edges;
        }
        private static Edge[] SubstractFromCol(Edge[] edges, int col, int minForCol)
        {
            foreach (var item in edges)
            {
                if (item.Destination == col)
                    item.Weight -= minForCol;
            }
            return edges;
        }

        private static int FindMinWeightInRow(Edge[] edges, int row)
        {
            var min = int.MaxValue;
            foreach (var item in edges)
            {
                if (item.Source == row)
                {
                    if (item.Weight < min)
                        min = item.Weight;
                }
            }
            return min;
        }
        private static int FindMinWeightInCol(Edge[] edges, int col)
        {
            var min = int.MaxValue;
            foreach (var item in edges)
            {
                if (item.Destination == col)
                {
                    if (item.Weight < min)
                        min = item.Weight;
                }
            }
            return min;
        }
    }
}