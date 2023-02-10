namespace BadTree.BehaviorTree {
    public interface IActionLeaf { string Name { get; } }
    public abstract class ActionLeaf<T> : BehaviorTreeNode, IActionLeaf, IBtNode {
        public T State { get; set; }
        public bool IsRoot => false;

        public ActionLeaf(IBtNode parent, string name) : base(parent, name) { }

        public abstract void Init();
        public abstract BtResult Tick();
    }
}