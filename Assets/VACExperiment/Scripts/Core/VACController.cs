using UnityEngine;

namespace VACExperiment
{
    public class VACController : MonoBehaviour
    {
        [SerializeField] private ExperimentConfig config;
        [SerializeField] private Transform viewer;
        [SerializeField] private Transform contentRoot;

        private Vector3 referenceScale = Vector3.one;
        private bool cachedScale;

        public VACCondition CurrentCondition { get; private set; }
        public float CurrentDistanceMeters { get; private set; }

        public void Configure(ExperimentConfig experimentConfig, Transform viewerTransform, Transform root)
        {
            config = experimentConfig;
            viewer = viewerTransform;
            contentRoot = root;
            CacheScale();
        }

        private void Awake()
        {
            CacheScale();
        }

        private void CacheScale()
        {
            if (contentRoot != null && !cachedScale)
            {
                referenceScale = contentRoot.localScale;
                cachedScale = true;
            }
        }

        public void ApplyCondition(VACCondition condition)
        {
            if (config == null || viewer == null || contentRoot == null)
            {
                Debug.LogError("VACController is missing config, viewer, or contentRoot.");
                return;
            }

            CacheScale();
            CurrentCondition = condition;
            CurrentDistanceMeters = config.GetDistance(condition);

            Vector3 forward = Vector3.ProjectOnPlane(viewer.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude < 0.0001f)
                forward = viewer.forward.normalized;

            contentRoot.position = viewer.position + forward * CurrentDistanceMeters;
            contentRoot.rotation = Quaternion.LookRotation(forward, Vector3.up);

            float scaleFactor = CurrentDistanceMeters / Mathf.Max(0.001f, config.referenceDistanceMeters);
            contentRoot.localScale = referenceScale * scaleFactor;
        }
    }
}