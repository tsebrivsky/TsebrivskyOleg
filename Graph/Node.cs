using System;

namespace Graph
{
    public class Node : ICloneable
    {
        public int Id { get; set; }
        public int Rank { get; set; }

        public object Clone()
        {
            return new Node{
                Id = this.Id,
                Rank = this.Rank
            };
        }
    }
}