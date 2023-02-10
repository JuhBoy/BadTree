namespace BadTree.BehaviorTree.Composites {
    public abstract class IdentifiableExecutor<T> : ActionLeaf<T>, IDispatcherNode {
        protected IdentifiableExecutor(IBtNode parent, int id, string name) : base(parent, name) { Id = id; }
        public int Id { get; }
    }
}