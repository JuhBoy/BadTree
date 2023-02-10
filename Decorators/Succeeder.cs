using System;

namespace BadTree.BehaviorTree.Decorators {
    public class Succeeder : Decorator, IBtNode {
        public Succeeder(IBtNode parent) : base(parent, "Succeeder") { }

        public void Init() {
            if (Child == null) {
                throw new NullReferenceException($"{nameof(Succeeder)} Child is null");
            }
            Child.Init();
        }

        public BtResult Tick() {
            Raise(BtResult.Tick, Child);
            Child.Tick();
            return BtResult.Success;
        }
    }
}