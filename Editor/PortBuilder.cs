using System;
using UnityEditor.Experimental.GraphView;

namespace BadTree.BehaviorTree.Editor {
    public class PortBuilder {
        public string Name { get; set; } = "Out";
        public Orientation Orientation { get; set; } = Orientation.Horizontal;
        public Port.Capacity Capacity { get; set; } = Port.Capacity.Single;
        public Type Type { get; set; } = typeof(float);
        public Direction Direction { get; set; } = Direction.Input;

        public Port Build(Node node, bool includeInNode) {
            Port port = node.InstantiatePort(Orientation, Direction, Capacity, Type);
            port.portName = Name;
            if (includeInNode) {
                IncludeInNode(node, port);
            }
            return port;
        }

        public void IncludeInNode(Node node, Port port) {
            if (Direction == Direction.Output) {
                node.outputContainer.Add(port);    
            } else {
                node.inputContainer.Add(port);
            }
            node.RefreshPorts();
            node.RefreshExpandedState();
        }
        
        public static Edge LinkNodes(GraphNode nodeA, GraphNode nodeB, int bIndex) {
            Edge edge = ((Port) nodeA.outputContainer[bIndex]).ConnectTo((Port)nodeB.inputContainer[0]);
            nodeA.RefreshPorts();
            nodeA.RefreshExpandedState();
            return edge;
        }
    }
}