using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BadTree.BehaviorTree.Editor {
    public static class NodeCreator {
        public static GraphNode CreateFreshMulti(string name, UnityEngine.Rect? position = null) {
            var portBuilder = new PortBuilder {
                Name = "Child",
                Type = typeof(GraphNode),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Multi
            };
            var inPortBuilder = new PortBuilder {
                Name = "In",
                Type = typeof(GraphNode),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single
            };
            var nodeBuilder = new NodeBuilder {
                Title = name,
                EntryPoint = false,
                Position = position ?? new UnityEngine.Rect(0, 0, 200, 200),
                AddOutputButton = false,
                LabelCounts = 2,
            };
            GraphNode node = nodeBuilder.Build();
            portBuilder.Build(node, true);
            inPortBuilder.Build(node, true);

            AddStyleSheet(node);

            return node;
        }

        public static GraphNode CreateFreshSingle(string name, bool entry = false, UnityEngine.Rect? position = null) {
            var portBuilder = new PortBuilder {
                Name = "Child",
                Type = typeof(GraphNode),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Single
            };
            var inPortBuilder = new PortBuilder {
                Name = "In",
                Type = typeof(GraphNode),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single
            };
            var nodeBuilder = new NodeBuilder {
                Title = name,
                EntryPoint = entry,
                Position = position ?? new UnityEngine.Rect(0, 0, 200, 200),
                AddOutputButton = false,
                LabelCounts = 2,
            };
            GraphNode node = nodeBuilder.Build();
            portBuilder.Build(node, true);
            inPortBuilder.Build(node, true);

            AddStyleSheet(node);

            return node;
        }

        public static GraphNode CreateFreshInOnly(string name, UnityEngine.Rect? position = null) {
            var inPortBuilder = new PortBuilder {
                Name = "In",
                Type = typeof(GraphNode),
                Direction = Direction.Input,
                Capacity = Port.Capacity.Single
            };
            var nodeBuilder = new NodeBuilder {
                Title = name,
                EntryPoint = false,
                Position = position ?? new UnityEngine.Rect(0, 0, 200, 200),
                AddOutputButton = false,
                LabelCounts = 2,
            };
            GraphNode node = nodeBuilder.Build();
            inPortBuilder.Build(node, true);

            AddStyleSheet(node);

            return node;
        }
        
        public static GraphNode CreateFreshOutOnly(string name, bool entry = false) {
            var portBuilder = new PortBuilder {
                Name = "Child",
                Type = typeof(GraphNode),
                Direction = Direction.Output,
                Capacity = Port.Capacity.Single
            };
            var nodeBuilder = new NodeBuilder {
                Title = name,
                EntryPoint = entry,
                Position = new UnityEngine.Rect(0, 0, 200, 200),
                AddOutputButton = false,
                LabelCounts = 2,
            };
            GraphNode node = nodeBuilder.Build();
            portBuilder.Build(node, true);

            AddStyleSheet(node);

            return node;
        }

        private static void AddStyleSheet(GraphNode node) {
            node.AddToClassList("GraphNode");
            node.titleContainer.AddToClassList("not-running");
            node.styleSheets.Add(Resources.Load<StyleSheet>("BTGraphNodeStyle"));
            node.extensionContainer.AddToClassList("extension");
            node.extensionContainer.Add(new TextElement() {text = "Active"});
        }
    }
}