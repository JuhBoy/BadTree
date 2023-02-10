using System.Collections.Generic;

namespace BadTree.BehaviorTree.Composites {
    public interface IExecutionDispatcher {
        ICollection<IDispatcherNode> Executors { get; }
    }

    public class ExecutionDispatcher : BehaviorTreeNode, IBtNode, IExecutionDispatcher {
        private readonly IIdentifiable selectedState;
        private readonly IDictionary<int, IDispatcherNode> executors;

        public bool IsRoot => Parent == null;
        public int ChildrenLength => executors.Count;
        public ICollection<IDispatcherNode> Executors => executors.Values;

        /// <summary>
        /// SelectedState is updated by a previous manager, in order to decide which action to tick on.
        /// </summary>
        public ExecutionDispatcher(IBtNode parent, IIdentifiable selectedState) : base(parent, nameof(ExecutionDispatcher)) {
            this.selectedState = selectedState;
            executors = new Dictionary<int, IDispatcherNode>();
        }

        public void RegisterExecutor(int id, IDispatcherNode identifiableExecutor) { executors.Add(id, identifiableExecutor); }

        public IDispatcherNode RemoveExecutor(int id) {
            IDispatcherNode identifiableExecutor = executors[id];
            executors.Remove(id);
            return identifiableExecutor;
        }

        public void Init() {
            foreach (IDispatcherNode child in executors.Values) {
                child.Init();
            }
        }

        public BtResult Tick() {
            Raise(BtResult.Tick, this);

            IDispatcherNode executor = executors[selectedState.Id];

            Raise(BtResult.Tick, executor);
            return executor.Tick();
        }
    }
}