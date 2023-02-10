using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BadTree.BehaviorTree.Editor {
    public class NodeBuilder {
        public string Title { get; set; } = "New Node";
        public bool EntryPoint { get; set; }
        public UnityEngine.Rect Position { get; set; } = new UnityEngine.Rect(100, 100, 100, 100);
        public List<Port> Outputs { get; private set; } = new List<Port>();
        public List<Port> Inputs { get; private set; } = new List<Port>();
        public bool AddOutputButton { get; set; }
        public int LabelCounts { get; set; }

        private void BuildOutputBuildButton(GraphNode node) {
            var button = new Button(() => {
                var portBuilder = new PortBuilder {
                    Type = typeof(GraphNode),
                    Direction = Direction.Output,
                    Name = "Out"
                };
                var port = portBuilder.Build(node, false);
                portBuilder.IncludeInNode(node, port);
            }) { text = "New output" };
            node.titleContainer.Add(button);
        }

        private void SetOutputsInputs(GraphNode node) {
            foreach (Port output in Outputs) {
                node.outputContainer.Add(output);
            }

            foreach (Port input in Inputs) {
                node.inputContainer.Add(input);
            }
        }

        public GraphNode Build() {
            var node = new GraphNode { title = Title, DialogueText = Title, GUID = Guid.NewGuid().ToString(), entryPoint = EntryPoint };
            node.SetPosition(Position);
            SetOutputsInputs(node);

            if (AddOutputButton) {
                BuildOutputBuildButton(node);
            }

            if (LabelCounts > 1) {
                for (var i = 1; i < LabelCounts; i++) {
                    node.Add(new Label("Ticks: 0"));
                }
            }

            node.RefreshExpandedState();
            node.RefreshPorts();

            return node;
        }
    }
}