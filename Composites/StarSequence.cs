using System;

namespace BadTree.BehaviorTree.Composites {
    /// <summary>
    /// Act as a sequence but do not tick already succeeded children.
    /// </summary>
    public class StarSequence : Composite {
        public StarSequence(IBtNode parent, string name = nameof(StarSequence)) : base(parent, name) { }

        private int currentIndex;

        public override void Init() { }

        public override BtResult Tick() {
            if (currentIndex > 0) {
                return TickCachedChild();
            }

            return TickAll();
        }

        private BtResult TickCachedChild() {
            Raise(BtResult.Tick, Children[currentIndex]);
            BtResult result = Children[currentIndex].Tick();

            if (result == BtResult.Running) {
                return result;
            } else if (result == BtResult.Failed) {
                currentIndex = 0;
                return BtResult.Failed;
            } else {
                currentIndex++;
                return TickAll();
            }
        }

        private BtResult TickAll() {
            for (int i = currentIndex; i < ChildrenLength; i++) {
                Raise(BtResult.Tick, Children[i]);
                switch (Children[i].Tick()) {
                    case BtResult.Success:
                        Raise(BtResult.Success, Children[i]);
                        continue;
                    case BtResult.Failed:
                        Raise(BtResult.Failed, Children[i]);
                        currentIndex = 0;
                        return BtResult.Failed;
                    case BtResult.Running:
                        Raise(BtResult.Running, Children[i]);
                        currentIndex = i;
                        return BtResult.Running;
                    case BtResult.Caching:
                    case BtResult.Tick:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            currentIndex = 0;
            return BtResult.Success;
        }
    }
}