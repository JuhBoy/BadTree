namespace BadTree.BehaviorTree.TreeUpdaters {
    public class OnceUpdater : IUpdateHandler {
        private bool updated;
        
        public BtResult TryTick(Entry entry, out bool ticked) {
            if (updated) {
                ticked = false;
                return BtResult.Success;
            }

            ticked = true;
            return entry.Tick();
        }

        public void Reset() {
            updated = false;
        }
    }
}