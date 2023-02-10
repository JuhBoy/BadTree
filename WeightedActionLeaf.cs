namespace BadTree.BehaviorTree {
    public abstract class WeightedActionLeaf<T> : ActionLeaf<T>, IWeightedNode {
        protected WeightedActionLeaf(IBtNode parent, string name) : base(parent, name) { }
        public abstract float GetWeight();
    }
}