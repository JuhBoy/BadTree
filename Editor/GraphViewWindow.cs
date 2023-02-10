using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BadTree.BehaviorTree.Editor {
    public class GraphViewWindow : EditorWindow {
        [SerializeField] private BehaviorTreeVisualizer visualizer;
        [SerializeField] private BehaviorGraphView graphView;
        [SerializeField] private ObjectField objectField;
        [SerializeField] private Toolbar toolbar;

        [NonSerialized] private static Object _cachedObjectValue;

        [MenuItem("Graph/IA Behavior Tree")]
        public static void OpenWindow() {
            var window = GetWindow<GraphViewWindow>();
            window.titleContent = new GUIContent("Behavior Tree Graph");
        }

        private void OnEnable() {
            graphView = new BehaviorGraphView() {
                name = "Bt's Graph View"
            };
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            GenerateToolBar();
        }

        private void Update() {
            visualizer?.Update();
            if (objectField.value != null) {
                _cachedObjectValue = objectField.value;
            }
        }
        
        private void Visualize() {
            if (objectField.value == null) {
                return;
            }
            visualizer = new BehaviorTreeVisualizer(objectField.value as BehaviorTreeManager, graphView);
            visualizer.Init();
        }

        private void Stop() {
            objectField.value = null;
            visualizer?.Dispose();
            visualizer = null;
        }

        private void GenerateToolBar() {
            toolbar = new Toolbar {name = "Edit"};
            // TODO: Do it when serialization will be implemented.
            // toolbar.contentContainer.Add(new Button(() => { graphView.AddNode(NodeCreator.CreateFreshMulti("Sequence")); }) {text = "Create Sequence"});
            // toolbar.contentContainer.Add(new Button(() => { graphView.AddNode(NodeCreator.CreateFreshMulti("Fallback")); }) {text = "Create Fallback"});
            // toolbar.contentContainer.Add(new Button(() => { }) {text = "Create ActionLeaf"});
            // toolbar.contentContainer.Add(new Button(() => { }) {text = "Create Succeeder"});
            // toolbar.contentContainer.Add(new Button(() => { }) {text = "Create Inverter"});

            objectField = new ObjectField("") {allowSceneObjects = true, objectType = typeof(BehaviorTreeManager), value = _cachedObjectValue};
            toolbar.contentContainer.Add(objectField);
            toolbar.contentContainer.Add(new Button(Visualize) {text = "Visualize"});
            toolbar.contentContainer.Add(new Button(Stop) {text = "Stop"});

            rootVisualElement.Add(toolbar);
        }

        private void OnDisable() {
            Stop();
            if (graphView != null) {
                graphView?.Clear();
                rootVisualElement.Remove(graphView);
            }
        }
    }
}