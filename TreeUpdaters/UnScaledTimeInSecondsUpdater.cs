using UnityEngine;

namespace BadTree.BehaviorTree.TreeUpdaters {
    public class UnScaledTimeInSecondsUpdater : ScaledTimeInSecondsUpdater {
        public UnScaledTimeInSecondsUpdater(UpdateMethod interval, float seconds) : base(interval, seconds) { }

        protected override float GetTime() { return (Interval == UpdateMethod.FixedUpdate) ? Time.fixedUnscaledDeltaTime : Time.unscaledDeltaTime; }
    }
}