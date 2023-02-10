using System.Collections.Generic;

namespace BadTree.BehaviorTree.Composites {
    public class CacheResolverSequence : Sequence {
        private Stack<IBtNode> cachedActions;

        public CacheResolverSequence(IBtNode parent) : base(parent, nameof(CacheResolverSequence)) { }

        private bool HasCachedActions => cachedActions.Count > 0;

        public override void Init() {
            cachedActions = new Stack<IBtNode>();
            Events.OnCachingLeafAction += OnCachingNodeRequest;
            Events.OnLoosingTarget += OnLosingTarget;
            base.Init(); // Init sequence children
        }

        public override BtResult Tick() {
            TickCachedActions();
            return HasCachedActions ? BtResult.Running : base.Tick();
        }

        private void TickCachedActions() {
            while (cachedActions.Count > 0) {
                IBtNode cachedAction = cachedActions.Peek();
                Raise(BtResult.Tick, cachedAction);
                BtResult result = cachedAction.Tick();
                Raise(result, cachedAction);

                if (result == BtResult.Running) {
                    return;
                } else {
                    cachedActions.Pop();
                }
            }
        }

        #region Events

        private void OnCachingNodeRequest(IBtNode node) => cachedActions.Push(node);
        private void OnLosingTarget() => cachedActions.Clear();

        #endregion
    }
}