using System.Collections.Generic;

namespace Graph.ChinesePostman
{
    public static class OddFinder
    {
        public static Node[] FindOddNodes(Node[] nodes)
        {
            var nodesToReturn = new List<Node>();
            foreach (var item in nodes)
            {
                if(item.Rank % 2 != 0)
                    nodesToReturn.Add(item);
            }
            return nodesToReturn.ToArray();
        }
    }
}