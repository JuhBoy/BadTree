namespace BadTree.BehaviorTree.Decorators {
    public class Failure : Decorator, IBtNode {
        public Failure(IBtNode parent) : base(parent, $"Decorator::{nameof(Failure)}") { }

        public void Init() { Child.Init(); }

        public BtResult Tick() {
            Raise(BtResult.Tick, Child);
            Child.Tick();
            return BtResult.Failed;
        }
    }
}