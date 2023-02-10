using System.Collections.Generic;

namespace BadTree.BehaviorTree.Composites {
    public class TimeBreakExecutor : Sequence {
        private readonly int defaultCacheSize;

        private Queue<TimeBreak> registeredTimeBreaks;

        public TimeBreakExecutor(IBtNode parent, int defaultCacheSize) : base(parent, nameof(TimeBreakExecutor)) { this.defaultCacheSize = defaultCacheSize; }

        public override void Init() {
            registeredTimeBreaks = new Queue<TimeBreak>(defaultCacheSize);
            Events.OnTimeBreakRegistered += OnTimeBreakRaised;
        }

        public override BtResult Tick() {
            while (registeredTimeBreaks.Count > 0) {
                TimeBreak timeBreak = registeredTimeBreaks.Peek();
                Raise(BtResult.Tick, timeBreak);
                BtResult result = timeBreak.Tick();
                Raise(result, timeBreak);

                if (result == BtResult.Running) {
                    return BtResult.Running;
                } else {
                    registeredTimeBreaks.Dequeue();
                }
            }
            
            return registeredTimeBreaks.Count > 0 ? BtResult.Running : base.Tick();
        }
        
        private void OnTimeBreakRaised(TimeBreak timeBreak) {
            registeredTimeBreaks.Enqueue(timeBreak);
        }
    }
}