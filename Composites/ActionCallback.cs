using System;

namespace BadTree.BehaviorTree.Composites {
    public class ActionCallback<T> : ActionLeaf<T> {
        private readonly Action<T> callbackAction;

        public ActionCallback(IBtNode parent, string name, Action<T> callbackAction) : base(parent, $"{nameof(ActionCallback<T>)}::{name}") {
            this.callbackAction = callbackAction;
        }

        public override void Init() { }

        public override BtResult Tick() {
            callbackAction(State);
            return BtResult.Success;
        }
    }
}