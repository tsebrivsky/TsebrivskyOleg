using System;

namespace Graph
{
    public class Edge : ICloneable
    {
        public int Source { get; set; }
        public int Destination { get; set; }
        public int Weight { get; set; }

        public object Clone()
        {
            return new Edge{
                Source = this.Source,
                Destination = this.Destination,
                Weight = this.Weight
            };
        }
    }
}