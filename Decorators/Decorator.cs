namespace BadTree.BehaviorTree.Decorators {
    public abstract class Decorator : BehaviorTreeNode {
        public bool IsRoot => Parent == null;
        public IBtNode Child { get; set; }
        protected Decorator(IBtNode parent, string name) : base(parent, name) { }
    }
}