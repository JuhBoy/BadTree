namespace BadTree.BehaviorTree {
    public enum BtResult {
        Tick,
        Success,
        Failed,
        Running,
        Caching,
        TargetLost,
    }

    public interface IBtNode {
        string Name { get; }
        IBtNode Parent { get; }
        void Init();
        BtResult Tick();
        bool IsRoot { get; }
    }
}