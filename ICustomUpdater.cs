using System;

namespace BadTree.BehaviorTree {
    public interface ICustomUpdater {
        void SetRoot(IBtNode root);
        void SetActive();
        void SetInactive();

        event Action<BtResult> OnTickResult;
    }
}