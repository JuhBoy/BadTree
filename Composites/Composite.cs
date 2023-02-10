namespace BadTree.BehaviorTree.Composites {
    public interface IComposite {
        IBtNode[] Children { get; }
        int ChildrenLength { get; }
    }
    
    public abstract class Composite : BehaviorTreeNode, IComposite, IBtNode {
        private IBtNode[] children;
        private int childrenLength;

        public IBtNode[] Children {
            get => children;
            set {
                children = value;
                childrenLength = children.Length;
            }
        }
        public bool IsRoot => Parent == null;
        public int ChildrenLength => childrenLength;

        protected Composite(IBtNode parent, string name) : base(parent, name) {
            Children = new IBtNode[0];
        }

        public abstract void Init();
        public abstract BtResult Tick();
    }
}