namespace Graph.Flow
{
    public class FlowModel
    {
        public Edge Edge { get; set; }
        public bool Visited { get; set; }
        public bool Full { get; set; }
        private int _flow;
        public int Flow
        {
            get { return _flow; }
            set
            {
                _flow = value;
                if (_flow == Edge.Weight)
                {
                    Full = true;
                }
            }
        }
    }
}