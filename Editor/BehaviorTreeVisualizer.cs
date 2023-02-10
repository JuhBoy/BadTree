using System;
using System.Collections.Generic;
using BadTree.BehaviorTree.Composites;
using BadTree.BehaviorTree.Decorators;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BadTree.BehaviorTree.Editor {
    public class BehaviorTreeVisualizer : IDisposable {
        private BehaviorGraphView Window { get; }
        private BehaviorTreeManager Manager { get; }
        private Dictionary<IBtNode, GraphNode> btNodeToGraphNode = new Dictionary<IBtNode, GraphNode>();
        private List<Edge> edgeContainer = new List<Edge>();
        private Dictionary<IBtNode, long> tickCountByNode = new Dictionary<IBtNode, long>();

        public BehaviorTreeVisualizer(BehaviorTreeManager manager, BehaviorGraphView window) {
            Window = window;
            Manager = manager;
            Manager.Events.OnTick += OnTickEvent;
            Manager.Events.OnFailed += OnFailedEvent;
            Manager.Events.OnSuccess += OnSuccessEvent;
            Manager.Events.OnRunning += OnRunningEvent;
        }

        private void OnTickEvent(IBtNode node) {
            if (node.Parent == null || node is Entry) {
                foreach (KeyValuePair<IBtNode, GraphNode> pair in btNodeToGraphNode) {
                    pair.Value.titleContainer.RemoveFromClassList("running");
                    pair.Value.extensionContainer[0].Q<TextElement>().RemoveFromClassList("extension-failed");
                    pair.Value.extensionContainer[0].Q<TextElement>().RemoveFromClassList("extension-running");
                    pair.Value.extensionContainer[0].Q<TextElement>().RemoveFromClassList("extension-success");
                    pair.Value.titleContainer.AddToClassList("not-running");
                }
            }

            btNodeToGraphNode[node].contentContainer[2].Q<Label>().text = $"Ticks: {tickCountByNode[node]++}";
            btNodeToGraphNode[node].titleContainer.RemoveFromClassList("not-running");
            btNodeToGraphNode[node].titleContainer.AddToClassList("running");
        }

        private void OnRunningEvent(IBtNode node) {
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).text = "Running";
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).AddToClassList("extension-running");
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).RemoveFromClassList("extension-failed");
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).RemoveFromClassList("extension-success");
        }

        private void OnSuccessEvent(IBtNode node) {
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).text = "Success";
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).AddToClassList("extension-success");
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).RemoveFromClassList("extension-failed");
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).RemoveFromClassList("extension-running");
        }

        private void OnFailedEvent(IBtNode node) {
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).text = "Failed";
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).AddToClassList("extension-failed");
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).RemoveFromClassList("extension-success");
            (btNodeToGraphNode[node].extensionContainer[0].Q<TextElement>()).RemoveFromClassList("extension-running");
        }

        public void Init() {
            CreateAndStoreNode();
            var positioner = new NodePositioner(Manager.Root, btNodeToGraphNode);
            positioner.Distribute();
        }

        private void CreateAndStoreNode() {
            var queue = new Queue<IBtNode>();
            queue.Enqueue(Manager.Root);

            while (queue.Count > 0) {
                IBtNode current = queue.Dequeue();
                CheckEventsAssignation(current);

                if (!btNodeToGraphNode.ContainsKey(current)) {
                    GraphNode node = NodeUtils.BuildNode(current, Window);
                    btNodeToGraphNode.Add(current, node);
                }

                IEnumerable<IBtNode> children = CreateAndLinkChildren(current);
                foreach (IBtNode child in children) {
                    if (child == null) {
                        continue;
                    }

                    queue.Enqueue(child);
                }
            }

            foreach (KeyValuePair<IBtNode, GraphNode> el in btNodeToGraphNode) {
                tickCountByNode.Add(el.Key, 0);
            }
        }

        private void CheckEventsAssignation(IBtNode current) {
            if (current is BehaviorTreeNode && ((BehaviorTreeNode)current).Events != null) {
                return;
            }

            Debug.LogWarning($"{current.Name} node has not events registered");
        }

        public void Dispose() {
            Manager.Events.OnTick -= OnTickEvent;
            Manager.Events.OnFailed -= OnFailedEvent;
            Manager.Events.OnSuccess -= OnSuccessEvent;
            Manager.Events.OnRunning -= OnRunningEvent;

            foreach (KeyValuePair<IBtNode, GraphNode> pair in btNodeToGraphNode) {
                pair.Value.parent.Remove(pair.Value);
            }

            foreach (Edge edge in edgeContainer) {
                edge.parent.Remove(edge);
            }
        }

        private IEnumerable<IBtNode> CreateAndLinkChildren(IBtNode current) {
            void AddChild(int nodeIndex, IBtNode node, ICollection<IBtNode> cacheChildren) {
                if (nodeIndex > 0) {
                    new PortBuilder {
                        Orientation = Orientation.Horizontal,
                        Capacity = Port.Capacity.Single,
                        Direction = Direction.Output,
                        Type = typeof(IBtNode)
                    }.Build(btNodeToGraphNode[current], true);
                }

                GraphNode gNode = NodeUtils.BuildNode(node, Window);
                btNodeToGraphNode.TryAdd(node, gNode);
                cacheChildren.Add(node);

                Edge seqEdge = PortBuilder.LinkNodes(btNodeToGraphNode[current], gNode, nodeIndex);
                edgeContainer.Add(seqEdge);
                Window.AddElement(seqEdge);
            }

            var children = new List<IBtNode>();

            switch (current) {
                case Entry entry:
                    GraphNode entryChildNode = NodeUtils.BuildNode(entry.Child, Window);
                    btNodeToGraphNode.Add(entry.Child, entryChildNode);
                    Edge entryEdge = PortBuilder.LinkNodes(btNodeToGraphNode[current], entryChildNode, 0);
                    edgeContainer.Add(entryEdge);
                    Window.AddElement(entryEdge);
                    children.Add(entry.Child);
                    break;
                case IComposite composite:
                    for (var i = 0; i < composite.Children.Length; i++) {
                        AddChild(i, composite.Children[i], children);
                    }

                    break;
                case WeightedSequence weightedSequence:
                    for (var i = 0; i < weightedSequence.Children.Length; i++) {
                        AddChild(i, weightedSequence.Children[i], children);
                    }

                    break;
                case Decorator decorator:
                    AddChild(0, decorator.Child, children);
                    break;
                case IExecutionDispatcher executionDispatcher:
                    var exeIndex = 0;
                    foreach (IDispatcherNode dispatcher in executionDispatcher.Executors) {
                        AddChild(exeIndex, dispatcher, children);
                        exeIndex++;
                    }

                    break;
                case WeightedActionLeaf<IBehaviorTreeData> _:
                case ActionLeaf<IBehaviorTreeData> _:
                    break;
            }

            return children;
        }

        public void Update() { }
    }
}