using System;

namespace BadTree.BehaviorTree {
    public class Entry : BehaviorTreeNode, IBtNode {
        public bool IsRoot => true;
        public IBtNode Child { get; set; }

        public Entry(string name = "Entry") : base(null, name) { }

        public void Init() {
            Child?.Init();
        }

        public BtResult Tick() {
            Raise(BtResult.Tick, this);
            Raise(BtResult.Tick, Child);
            BtResult result = Child.Tick();

            switch (result) {
                case BtResult.Success:
                    Raise(BtResult.Success, Child);
                    Raise(BtResult.Success, this);
                    break;
                case BtResult.Failed:
                    Raise(BtResult.Failed, Child);
                    Raise(BtResult.Failed, this);
                    break;
                case BtResult.Running:
                    Raise(BtResult.Running, Child);
                    Raise(BtResult.Running, this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return result;
        }
    }
}