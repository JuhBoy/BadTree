using UnityEngine;

namespace BadTree.BehaviorTree.TreeUpdaters {
    public class ScaledTimeInSecondsUpdater : IUpdateHandler {
        protected readonly UpdateMethod Interval;
        protected readonly float Seconds;

        protected float TimeElapsedSinceLastCall;

        public ScaledTimeInSecondsUpdater(UpdateMethod interval, float seconds) {
            Interval = interval;
            Seconds = seconds;
        }

        public BtResult TryTick(Entry entry, out bool ticked) {
            TimeElapsedSinceLastCall += GetTime();
            if (TimeElapsedSinceLastCall >= Seconds) {
                TimeElapsedSinceLastCall = 0;
                ticked = true;
                return entry.Tick();
            }

            ticked = false;
            return BtResult.Success;
        }

        protected virtual float GetTime() { return (Interval == UpdateMethod.FixedUpdate) ? Time.fixedDeltaTime : Time.deltaTime; }
    }
}