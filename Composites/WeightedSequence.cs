using System.Collections.Generic;
using UnityEngine;

namespace BadTree.BehaviorTree.Composites {
    /// <summary>
    /// Traverse all children in order to find the best weight (higher).
    /// Only the child with the higher weight will be ticked.
    /// The weights are reevaluated every tick.
    /// </summary>
    public sealed class WeightedSequence : BehaviorTreeNode, IBtNode {
        private readonly int defaultIndex;
        private readonly IList<int> equalsIndexes;

        public WeightedSequence(IBtNode parent, int defaultIndex = 0) : base(parent, nameof(WeightedSequence)) {
            this.defaultIndex = defaultIndex;
            equalsIndexes = new List<int>(5);
        }

        public bool IsRoot => Parent == null;
        public IWeightedNode[] Children { get; set; }
        private int ChildrenLength => Children.Length;

        public void Init() {
            foreach (IWeightedNode child in Children) {
                child.Init();
            }
        }

        public BtResult Tick() {
            if (ChildrenLength <= 0) {
                return BtResult.Success;
            }

            equalsIndexes.Clear();
            var highestWeight = 0.0F;

            for (var i = 0; i < ChildrenLength; i++) {
                float current = Children[i].GetWeight();

                if (current > highestWeight) {
                    equalsIndexes.Clear();
                    equalsIndexes.Add(i);
                    highestWeight = current;
                } else if (Mathf.Abs(current - highestWeight) < 0.01F) {
                    equalsIndexes.Add(i);
                }
            }

            int selectedIndex = GetSelectedIndex();

            Raise(BtResult.Tick, Children[selectedIndex]);
            return Children[selectedIndex].Tick();
        }

        private int GetSelectedIndex() {
            switch (equalsIndexes.Count) {
                case 0:
                    return defaultIndex;
                case 1:
                    return equalsIndexes[0];
                default: {
                    int randomizedIndex = Random.Range(0, equalsIndexes.Count);
                    return equalsIndexes[randomizedIndex];
                }
            }
        }
    }
}