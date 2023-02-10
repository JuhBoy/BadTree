namespace BadTree.BehaviorTree.TreeUpdaters {
    public interface IUpdateHandler {
        BtResult TryTick(Entry entry, out bool ticked);
    }
}