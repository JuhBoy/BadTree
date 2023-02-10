using System;
using BadTree.BehaviorTree.Composites;

namespace BadTree.BehaviorTree {
    public class BtEvents {
        public event Action<IBtNode> OnTick;
        public event Action<IBtNode> OnSuccess;
        public event Action<IBtNode> OnFailed;
        public event Action<IBtNode> OnRunning;
        public event Action<TimeBreak> OnTimeBreakRegistered;

        public event Action<IBtNode> OnCachingLeafAction;
        public event Action OnLoosingTarget;

        public void Raise(BtResult result, IBtNode node) {
            switch (result) {
                case BtResult.Tick:
                    OnTick?.Invoke(node);
                    break;
                case BtResult.Success:
                    OnSuccess?.Invoke(node);
                    break;
                case BtResult.Failed:
                    OnFailed?.Invoke(node);
                    break;
                case BtResult.Running:
                    OnRunning?.Invoke(node);
                    break;
                case BtResult.Caching:
                    OnCachingLeafAction?.Invoke(node);
                    break;
                case BtResult.TargetLost:
                    OnLoosingTarget?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public void RaiseTimeBreak(TimeBreak timeBreak) { OnTimeBreakRegistered?.Invoke(timeBreak); }
    }
}