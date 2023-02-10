namespace BadTree.BehaviorTree.Composites {
    public class Sequence : Composite {
        public Sequence(IBtNode parent, string name = nameof(Sequence)) : base(parent, name) { }

        public override void Init() {
            foreach (IBtNode child in Children) {
                child.Init();
            }
        }

        public override BtResult Tick() {
            foreach (IBtNode child in Children) {
                Raise(BtResult.Tick, child);
                switch (child.Tick()) {
                    case BtResult.Failed:
                        Raise(BtResult.Failed, child);
                        return BtResult.Failed;
                    case BtResult.Running:
                        Raise(BtResult.Running, child);
                        return BtResult.Running;
                    case BtResult.Success:
                        Raise(BtResult.Success, child);
                        continue;
                    default:
                        continue;
                }
            }

            Raise(BtResult.Success, this);
            return BtResult.Success;
        }
    }
}