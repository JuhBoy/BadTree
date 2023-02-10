namespace BadTree.BehaviorTree {
    public abstract class BehaviorTreeNode {
        public string Name { get; }
        public IBtNode Parent { get; }
        public BtEvents Events { get; set; }

        protected BehaviorTreeNode(IBtNode parent, string name) {
            Parent = parent;
            Name = name;
        }

        public void Raise(BtResult result, IBtNode node) { Events?.Raise(result, node); }
    }
}