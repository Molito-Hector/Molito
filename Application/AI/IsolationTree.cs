namespace Application.AI
{
    [Serializable]
    public class IsolationTree
    {
        public int Height { get; set; }
        public IsolationTree Left { get; set; }
        public IsolationTree Right { get; set; }
        public int SplitAttribute { get; set; }
        public double SplitValue { get; set; }
        public bool IsExternalNode { get; set; }
    }
}