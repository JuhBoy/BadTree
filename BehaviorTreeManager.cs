using System;
using BadTree.BehaviorTree.TreeUpdaters;
using UnityEngine;

namespace BadTree.BehaviorTree {
    public enum UpdateMethod {
        Update,
        FixedUpdate,
        Custom,
    }

    public enum UpdateInterval {
        EngineFrame,
        ScaledTimeInSeconds,
        UnScaledTimeInSeconds,
        External, // The user will call Tick by himself
        Once,
    }

    public abstract class BehaviorTreeManager : MonoBehaviour {
        [NonSerialized] protected IBtNode root;

        [Header("Behavior Base"), SerializeField]
        protected bool active;

        [SerializeField] protected UpdateMethod updateMethod;
        [SerializeField] protected UpdateInterval updateInterval;
        [SerializeField] protected float intervalInSeconds;
        [SerializeField] protected ICustomUpdater customUpdater;
        protected BtResult result;

        protected IUpdateHandler Updater { get; set; }

        public IBtNode Root => root;

        public BtResult Result {
            get => result;
            set {
                result = value;
                TriggerEvents(result);
            }
        }

        [field: NonSerialized] public BtEvents Events { get; set; } = new();

        private bool Active {
            get => active;
            set {
                active = value;
                if (active) {
                    customUpdater?.SetActive();
                } else {
                    customUpdater?.SetInactive();
                }
            }
        }

        public void SetCustomUpdater(ICustomUpdater iCustomUpdater) {
            updateMethod = UpdateMethod.Custom;
            customUpdater = iCustomUpdater;
        }

        protected virtual void Build() {
            if (Root == null && Active) {
                Result = BtResult.Failed;
                Debug.LogError($"Activated behavior tree without root");
            }

            Root?.Init();
            Updater = TreeUpdatersFactory.Create(updateMethod, updateInterval, intervalInSeconds);

            if (updateMethod == UpdateMethod.Custom) {
                customUpdater.SetRoot(root);
                customUpdater.SetActive();
                customUpdater.OnTickResult += OnCustomTickUpdate;
            }
        }

        protected virtual void Update() {
            if (updateMethod != UpdateMethod.Update || !Active) {
                return;
            }

            BtResult btResult = Updater.TryTick((Entry)Root, out bool ticked);
            if (ticked) {
                Result = btResult;
            }
        }

        protected virtual void FixedUpdate() {
            if (updateMethod != UpdateMethod.FixedUpdate || !Active) {
                return;
            }

            BtResult btResult = Updater.TryTick((Entry)Root, out bool ticked);
            if (ticked) {
                Result = btResult;
            }
        }

        #region Events

        private void OnCustomTickUpdate(BtResult cResult) {
            if (!active) {
                Result = BtResult.Failed;
                throw new ApplicationException($"Custom updater has not been deactivated");
            }

            Result = cResult;
        }

        private void TriggerEvents(BtResult btResult) { Events.Raise(btResult, root); }

        #endregion
    }
}