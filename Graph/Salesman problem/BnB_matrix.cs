using System;
using System.Collections.Generic;
using System.Linq;

namespace Graph.Salesman_problem
{
    public class BnB_matrix
    {
        // public static Graph _graph;
        public static List<Edge> _edges;
        public static int[,] _matrix;
        public static int[,] StaticMatrix;
        public static int costWithEdge;
        public static int costWithoutEdge;
        public static int _cost;
        public static bool noValues;
        public static int lowestCost;
        public static int notChangedCost;

        List<(int[,] matrix, int pathCost)> matrices;


        public BnB_matrix()
        {
            _edges = new List<Edge>();
            costWithEdge = int.MaxValue;
            _cost = default;
            lowestCost = default;
            notChangedCost = default;
            noValues = true;

            matrices = new List<(int[,] matrix, int pathCost)>();
        }

        public Edge[] BranchAndBound(int[,] matrix)
        {
            if (StaticMatrix == null)
            {
                StaticMatrix = (int[,])matrix.Clone();
            }

            noValues = true;
            _matrix = (int[,])matrix.Clone();

            matrix = Substracting(matrix);//Знайти та відняти мінімальні значення для кожного рядка, потім для кожного стовпця

            if (lowestCost == 0)
            {
                lowestCost = _cost;
                notChangedCost = _cost;
            }

            matrices.Add((StaticMatrix, notChangedCost));
            int r = 0;
            int c = 0;
            int maxCoef = 0;

            if (!noValues)
            {
                matrix = RemoveMaxCoef(matrix, out r, out c, out maxCoef);//Порахувати штрафи за невикористання кожного ребра(утворені нулі) та обрати максимальний

                costWithoutEdge = maxCoef + lowestCost;//Загальна вартість шляху = вартість початковго гамільтонового шляху + штраф

                matrices.Add((matrix, costWithoutEdge));

                matrix[c, r] = -1;//Ребро, що є протилежним не може входити в маршрут у випадку використання ребра із максимальним штрафосм

                for (int i = 0; i < matrix.GetLength(0); i++)//У випадку використання цьгого ребра видалити відповідний рядок
                {
                    matrix[r, i] = -1;
                }
                for (int i = 0; i < matrix.GetLength(1); i++)//та ствопець
                {
                    matrix[i, c] = -1;
                }
                var newcost = CalculateSomeCostWithNewEdge(matrix);//вартість гамільтонового шляху у скороченій матриці = попередня вартість + сума констант приведеної матриці
                costWithEdge = lowestCost + newcost;
                matrices.Add((matrix, costWithEdge));
            }
            else
            {
                _cost = _edges.Sum(x => x.Weight);
                return _edges.ToArray();
            }

            if (costWithoutEdge < costWithEdge)//Якщо раптом невикористання ребра вигідніше, то повертаємось на крок коли було норм, і комірку коли було норм не використовуємо
            {
                var rollbackMatrix = matrices.First(x => x.pathCost < costWithEdge).matrix;
                int a = 0; int b = 0; int m = 0;
                rollbackMatrix = RemoveMaxCoef(rollbackMatrix, out a, out b, out m);
                rollbackMatrix[a, b] = -1;
                BranchAndBound(rollbackMatrix);
            }
            else
            {
                _edges.Add(new Edge()//Включене ребро у шлях добавляю у шлях
                {
                    Source = r,
                    Destination = c,
                    // Weight = StaticMatrix[r, c] != 0 ? StaticMatrix[r, c] : costWithEdge - lowestCost
                    Weight = StaticMatrix[r, c]
                });
                if (_edges.Count >= 2 && _edges.SkipLast(1).Last().Source == _edges.Last().Destination)//якщо початок попереднього ребра і кінець наступного ребра однакові
                    matrix[_edges.SkipLast(1).Last().Destination, _edges.Last().Source] = -1;//запобігання підциклів, видаляю таке ребро
                lowestCost = costWithEdge;
                BranchAndBound(matrix);//рекурсія суцільна
            }
            var s = _edges.Sum(x => x.Weight);
            return _edges.ToArray();
        }


        private static int CalculateSomeCostWithNewEdge(int[,] matrix)
        {
            int cost = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var minForRow = FindMinWeightInRow(matrix, i);
                if (minForRow != int.MaxValue)
                {
                    cost += minForRow;
                }
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                var minForCol = FindMinWeightInCol(matrix, i);
                if (minForCol != int.MaxValue)
                {
                    cost += minForCol;
                }
            }
            return cost;
        }


        private static bool IsEmpty(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (matrix[i, j] > 0)
                        return false;
                }
            }
            return true;
        }
        private static int[,] Substracting(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var minForRow = FindMinWeightInRow(matrix, i);
                if (minForRow != int.MaxValue)
                {
                    matrix = SubstractFromRow(matrix, i, minForRow);
                    _cost += minForRow;
                    noValues = false;
                }
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                var minForCol = FindMinWeightInCol(matrix, i);
                if (minForCol != int.MaxValue)
                {
                    matrix = SubstractFromCol(matrix, i, minForCol);
                    _cost += minForCol;
                    noValues = false;
                }
            }
            return matrix;
        }
        private static int[,] RemoveMaxCoef(int[,] matrix, out int a, out int b, out int c)
        {
            // int maxCoefForRow = default;
            // int maxCoefForCol = default;
            // int maxCoef = int.MinValue;
            int maxCoefForRow = default;
            int maxCoefForCol = default;
            int maxCoef = int.MinValue;
            List<(int row, int col, int maxCof)> Fine = new List<(int row, int col, int maxCof)>();
            //Вираховується штраф за невикористання кожного нульового елементу приведеної мтариці
            //Штарф за невикористання індексу (i,j) в матриці означає, що це ребро не включається в маршрут, а знач мінімальна вартість
            //"невикористання" цього ребра рівна сумі мінімальних елментів в стріці i & column j
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    int coef = default;
                    if (matrix[i, j] == 0)//шукаю всі нульові елементи в новій приведеній матриці
                    {
                        coef = CalculateCoeficient(matrix, i, j);//для кожного нуля рахую штраф за невикористання
                        if (coef > maxCoef)//обираю останній(?) найбільший штраф
                        {
                            maxCoefForRow = i;
                            maxCoefForCol = j;
                            maxCoef = coef;
                            if (Fine.Exists(x => x.row == maxCoefForRow && x.col == maxCoefForCol))
                            {
                                var tuple = Fine.First(x => x.row == maxCoefForRow && x.col == maxCoefForCol);
                                tuple.maxCof = coef;
                                var index = Fine.FindIndex(x => x.row == maxCoefForRow && x.col == maxCoefForCol);
                                Fine[index] = tuple;
                            }
                            else
                                Fine.Add((i, j, coef));
                        }
                    }
                }

            }
            a = maxCoefForRow;
            b = maxCoefForCol;
            c = maxCoef;
            return matrix;
        }

        private static int CalculateCoeficient(int[,] matrix, int source = -1, int destination = -1)
        {
            int minForRow = int.MaxValue;
            int minForCol = int.MaxValue;

            if (destination != -1)
            {
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    if (i != source && matrix[i, destination] != -1 && matrix[i, destination] < minForCol)
                    {
                        minForCol = matrix[i, destination];
                    }
                }
            }

            if (source != -1)
            {
                for (int i = 0; i < matrix.GetLength(1); i++)
                {
                    if (i != destination && matrix[source, i] != -1 && matrix[source, i] < minForRow)
                    {
                        minForRow = matrix[source, i];
                    }
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

        private static int[,] SubstractFromRow(int[,] matrix, int row, int minForRow)
        {
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (matrix[row, i] != -1)
                    matrix[row, i] -= minForRow;
            }
            return matrix;
        }
        private static int[,] SubstractFromCol(int[,] matrix, int col, int minForCol)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, col] != -1)
                    matrix[i, col] -= minForCol;
            }
            return matrix;
        }

        private static int FindMinWeightInRow(int[,] matrix, int row)
        {
            var min = int.MaxValue;
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (matrix[row, i] != -1 && matrix[row, i] < min)
                    min = matrix[row, i];
            }
            return min;
        }
        private static int FindMinWeightInCol(int[,] matrix, int col)
        {
            var min = int.MaxValue;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (matrix[i, col] != -1 && matrix[i, col] < min)
                    min = matrix[i, col];
            }
            return min;
        }
    }
}