using UnityEngine;

namespace BadTree.BehaviorTree.Composites {
    public sealed class TimeBreak : Composite {
        private readonly float timeWindowInSeconds;
        private readonly bool saveInCache;

        private bool counting;
        private float lastTimeFootPrint;

        public TimeBreak(IBtNode parent, float timeWindowInSeconds, bool saveInCache = true) : base(parent, nameof(TimeBreak)) {
            this.timeWindowInSeconds = timeWindowInSeconds;
            this.saveInCache = saveInCache;
        }

        public override void Init() { }

        public override BtResult Tick() {
            if (counting && (Time.time - lastTimeFootPrint >= timeWindowInSeconds) || timeWindowInSeconds <= 0.0f) {
                counting = false;
                return BtResult.Success;
            }

            if (!counting) {
                lastTimeFootPrint = Time.time;
                counting = true;
                TryCacheRegistration();
            }

            return BtResult.Running;
        }

        private void TryCacheRegistration() {
            if (!saveInCache) {
                return;
            }
            Events.RaiseTimeBreak(this);
        }
    }
}