using System;
using System.Collections.Generic;
using System.Linq;
using BadTree.BehaviorTree.Composites;
using BadTree.BehaviorTree.Decorators;

namespace BadTree.BehaviorTree.Editor {
    public static class NodeUtils {
        public static IEnumerable<IBtNode> GetChildren(IBtNode current) {
            var children = new List<IBtNode>();

            switch (current) {
                case Entry entry:
                    children.Add(entry.Child);
                    break;
                case IComposite composite:
                    children.AddRange(composite.Children);
                    break;
                case Decorator decorator:
                    children.Add(decorator.Child);
                    break;
                case IExecutionDispatcher executionDispatcher:
                    children.AddRange(executionDispatcher.Executors);
                    break;
                case WeightedSequence weightedSequence:
                    children.AddRange(weightedSequence.Children);
                    break;
                case ActionLeaf<IBehaviorTreeData> _:
                    break;
            }

            return children;
        }

        public static bool HasChildren(IBtNode node) {
            return node switch {
                Entry entry => entry.Child != null,
                IComposite seq => seq.Children.Length > 0,
                WeightedSequence wSeq => wSeq.Children.Length > 0,
                Decorator dec => dec.Child != null,
                IExecutionDispatcher exe => exe.Executors.Count > 0,
                _ => false
            };
        }

        public static GraphNode BuildNode(IBtNode current, BehaviorGraphView window) {
            GraphNode node;
            switch (current) {
                case Entry _:
                    node = NodeCreator.CreateFreshOutOnly(current.Name, true);
                    break;
                case IComposite _:
                    node = NodeCreator.CreateFreshMulti(current.Name);
                    break;
                case WeightedSequence _:
                    node = NodeCreator.CreateFreshMulti(current.Name);
                    break;
                case Decorator _:
                    node = NodeCreator.CreateFreshMulti(current.Name);
                    break;
                case IExecutionDispatcher _:
                    node = NodeCreator.CreateFreshMulti(current.Name);
                    break;
                case WeightedActionLeaf<IBehaviorTreeData> _:
                case ActionLeaf<IBehaviorTreeData> _:
                    node = NodeCreator.CreateFreshInOnly(current.Name);
                    break;
                default:
                    if (current.GetType().GetInterfaces().Any(a => a == typeof(IActionLeaf) ||
                                                                   a == typeof(IWeightedNode) ||
                                                                   a == typeof(IDispatcherNode))) {
                        node = NodeCreator.CreateFreshInOnly(current.Name);
                        break;
                    }

                    throw new ApplicationException("Type not handled");
            }

            window.AddNode(node);

            return node;
        }
    }
}