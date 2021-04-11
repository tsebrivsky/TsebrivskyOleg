using System;
using System.Linq;
using Graph.ChinesePostman;
using Graph.Salesman_problem;
using Graph.Flow;

namespace Graph
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var graph = Initializing.CreateGraph(@"../Graph/_Flow.txt");
            // var matrix = Initializing.CreateMatrix(@"../Graph/_SalesmanProblem.txt");

            FlowCalc flow = new FlowCalc(graph);
            flow.FindMaximumFlow();

        }
    }
}
