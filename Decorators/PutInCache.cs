using BadTree.BehaviorTree.Composites;

namespace BadTree.BehaviorTree.Decorators {
    public class PutInCache : Decorator, IBtNode {
        public PutInCache(IBtNode parent, string name = nameof(PutInCache)) : base(parent, name) { }

        public void Init() { Child.Init(); }

        public BtResult Tick() {
            Raise(BtResult.Tick, Child);
            Raise(BtResult.Caching, Child);
            return Child.Tick();
        }
    }

    public class IdentifiablePutInChace : PutInCache, IIdentifiable {
        public IdentifiablePutInChace(IBtNode parent, int id, string name = nameof(IdentifiablePutInChace)) : base(parent, name) { Id = id; }
        public int Id { get; }
    }

    public class DispatcherPutInCache : IdentifiablePutInChace, IDispatcherNode {
        public DispatcherPutInCache(IBtNode parent, int id, string name = nameof(DispatcherPutInCache)) : base(parent, id, name) { }
    }
}