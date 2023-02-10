using System;

namespace BadTree.BehaviorTree.TreeUpdaters {
    public static class TreeUpdatersFactory {
        public static IUpdateHandler Create(UpdateMethod method, UpdateInterval interval, float intervalInSeconds = 0.0F) {
            switch (interval) {
                case UpdateInterval.EngineFrame:
                    return new EngineFrameUpdater();
                case UpdateInterval.ScaledTimeInSeconds:
                    return new ScaledTimeInSecondsUpdater(method, intervalInSeconds);
                case UpdateInterval.UnScaledTimeInSeconds:
                    return new UnScaledTimeInSecondsUpdater(method, intervalInSeconds);
                case UpdateInterval.Once:
                    return new OnceUpdater();
                case UpdateInterval.External:
                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }
        }
    }
}