namespace BadTree.BehaviorTree.TreeUpdaters {
    public class EngineFrameUpdater : IUpdateHandler {
        public BtResult TryTick(Entry entry, out bool ticked) {
            ticked = true;
            return entry.Tick();
        }
    }
}