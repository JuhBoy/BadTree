using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace BadTree.BehaviorTree.Editor {
    public class NodePositioner {
        private IBtNode Root { get; }
        private Dictionary<IBtNode, GraphNode> NodeRelation { get; }

        private readonly Vector2 nodeSize = new Vector2(200, 200);

        private readonly float verticalStep = 150;
        private readonly float horizontalStep = 250;

        public NodePositioner(IBtNode root, Dictionary<IBtNode, GraphNode> nodeRelation) {
            Root = root;
            NodeRelation = nodeRelation;
        }

        #region Positionning

        public void Distribute() {
            List<List<IBtNode>> nodesByLevels = PrepareLevelPlacement();
            var takenPositionPerLevel = new Dictionary<int, Vector2>();
            var toRealignNode = new Stack<(int i, int j)>();

            var childrenHorPos = 0.0F;
            var childrenVerPos = 0.0F;

            NodeRelation[nodesByLevels[0][0]].SetPosition(new UnityEngine.Rect(new Vector2(childrenHorPos, childrenVerPos), nodeSize));
            var nodesPosition = new Dictionary<IBtNode, Vector2> { [nodesByLevels[0][0]] = new Vector2(childrenHorPos, childrenVerPos) };

            for (var i = 0; i < nodesByLevels.Count - 1; i++) {
                childrenHorPos += horizontalStep;

                for (var j = 0; j < nodesByLevels[i].Count; j++) {
                    IBtNode node = nodesByLevels[i][j];

                    childrenVerPos = nodesPosition[node].y;

                    if (NodeUtils.HasChildren(node)) {
                        IBtNode[] children = NodeUtils.GetChildren(node).ToArray();

                        foreach (IBtNode child in children) {
                            var childPosition = new Vector2(childrenHorPos, childrenVerPos);

                            if (takenPositionPerLevel.ContainsKey(i) && takenPositionPerLevel[i].y >= childPosition.y) {
                                childPosition = new Vector2(childrenHorPos, takenPositionPerLevel[i].y + verticalStep);
                            }

                            TryAdd(takenPositionPerLevel, i, childPosition);
                            nodesPosition[child] = childPosition;

                            NodeRelation[child].SetPosition(new UnityEngine.Rect(childPosition, nodeSize));

                            float step = NodeUtils.HasChildren(child) ? NodeUtils.GetChildren(child).Count() * verticalStep : verticalStep;

                            childrenVerPos = childPosition.y + step;
                        }

                        toRealignNode.Push((i, j));
                    }

                    NodeRelation[node].RefreshExpandedState();
                    NodeRelation[node].RefreshPorts();
                }
            }

            // Realign parent from right to left with their children.
            // Move to bottom nodes that overlaps.
            while (toRealignNode.Count > 0) {
                (int i, int j) = toRealignNode.Pop();
                IBtNode current = nodesByLevels[i][j];
                IBtNode[] children = NodeUtils.GetChildren(current).ToArray();

                var updatedPosition = new Vector2(nodesPosition[current].x, GetMiddlePoint(children[0], children[^1], nodesPosition));
                float lowerVertical = updatedPosition.y;

                for (int r = j + 1; r < nodesByLevels[i].Count; r++) {
                    IBtNode next = nodesByLevels[i][r];
                    if (Mathf.Abs(nodesPosition[next].y - lowerVertical) <= nodeSize.y || nodesPosition[next].y < lowerVertical) {
                        var newPosition = new Vector2(nodesPosition[next].x, lowerVertical + verticalStep);
                        nodesPosition[next] = newPosition;
                        lowerVertical = lowerVertical + verticalStep;
                        NodeRelation[next].SetPosition(new UnityEngine.Rect(newPosition, nodeSize));
                    }
                }

                NodeRelation[current].SetPosition(new UnityEngine.Rect(updatedPosition, nodeSize));
                nodesPosition[current] = updatedPosition;
            }
        }

        private static void TryAdd(IDictionary<int, Vector2> dictionary, int i, Vector2 value) {
            if (!dictionary.ContainsKey(i)) {
                dictionary.Add(i, value);
            } else {
                dictionary[i] = value;
            }
        }

        private static float GetMiddlePoint(IBtNode a, IBtNode b, Dictionary<IBtNode, Vector2> nodePositions) {
            Vector2 aPosition = nodePositions[a];
            Vector2 bPosition = nodePositions[b];
            return aPosition.y + (bPosition.y - aPosition.y) * .5F;
        }

        private static float GetOffset(IReadOnlyList<List<IBtNode>> nodesByLevels, int levelIndex, IBtNode parent) {
            if (levelIndex == 0 || parent == null) {
                return 0.0f;
            }

            IBtNode parentParent = parent.Parent;

            var acc = .0F;

            for (var i = 0; i < nodesByLevels[levelIndex].Count; i++) {
                if (nodesByLevels[levelIndex][i] == parent) {
                    break;
                }

                if (nodesByLevels[levelIndex][i].Parent != parentParent) {
                    acc += GetOffset(nodesByLevels, levelIndex - 1, nodesByLevels[levelIndex][i].Parent);
                }
            }

            return acc;
        }

        #endregion

        private List<List<IBtNode>> PrepareLevelPlacement() {
            var nodesByLevelOrder = new List<List<IBtNode>>();
            int level = 0;
            var queue = new Queue<IBtNode>();
            var temp = new List<IBtNode>(20);

            queue.Enqueue(Root);
            nodesByLevelOrder.Add(new List<IBtNode>() { Root });

            while (queue.Count > 0) {
                IBtNode current = queue.Dequeue();
                IEnumerable<IBtNode> children = NodeUtils.GetChildren(current);
                temp.AddRange(children);

                if (queue.Count == 0) { // if queue is empty, an entire level has been processed, let's flush children.
                    level++;
                    nodesByLevelOrder.Add(temp);
                    temp = new List<IBtNode>(20);

                    foreach (IBtNode node in nodesByLevelOrder[level]) { // Now swap children as parents for next level children.
                        queue.Enqueue(node);
                    }
                }
            }

            return nodesByLevelOrder;
        }
    }
}