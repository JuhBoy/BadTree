namespace BadTree.BehaviorTree.Composites {
    public class Fallback : Composite {
        public Fallback(IBtNode parent) : base(parent, "Fallback") { }

        public override void Init() {
            foreach (IBtNode child in Children) {
                child.Init();
            }
        }

        public override BtResult Tick() {
            foreach (IBtNode child in Children) {
                Raise(BtResult.Tick, child);
                switch (child.Tick()) {
                    case BtResult.Running:
                        Raise(BtResult.Running, child);
                        return BtResult.Running;
                    case BtResult.Success:
                        Raise(BtResult.Success, child);
                        return BtResult.Success;
                    case BtResult.Failed:
                        Raise(BtResult.Failed, child);
                        continue;
                }
            }

            return BtResult.Failed;
        }
    }
}