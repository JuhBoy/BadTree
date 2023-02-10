using System;

namespace BadTree.BehaviorTree.Decorators {
    public class Inverter : Decorator, IBtNode {
        public Inverter(IBtNode parent) : base(parent, "Inverter") { }

        public void Init() {
            if (Child == null) {
                throw new NullReferenceException($"{nameof(Inverter)} Child is null");
            }
            Child.Init();
        }

        public BtResult Tick() {
            Raise(BtResult.Tick, this);
            BtResult childResult = Child.Tick();
            Raise(BtResult.Tick, Child);

            switch (childResult) {
                case BtResult.Success:
                    Raise(BtResult.Success, Child);
                    break;
                case BtResult.Failed:
                    Raise(BtResult.Failed, Child);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return childResult == BtResult.Failed ? BtResult.Success : BtResult.Failed;
        }
    }
}