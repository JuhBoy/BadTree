using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BadTree.BehaviorTree.Editor {
    public class BehaviorGraphView : GraphView {
        public BehaviorGraphView() {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        public void AddNode(GraphNode node) { AddElement(node); }

        // TODO: This must be handled by a filter class
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
            var compatiblePorts = new List<Port>();

            foreach (Port port in ports) {
                if (port != startPort && startPort.node != port.node) {
                    compatiblePorts.Add(port);
                }
            }

            return compatiblePorts;
        }
    }
}