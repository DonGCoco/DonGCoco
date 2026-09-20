using System;
using System.Collections.Generic;
using UnityEngine;

namespace VACExperiment
{
    public class DepthJudgmentManager : MonoBehaviour
    {
        [SerializeField] private ExperimentConfig config;
        [SerializeField] private Transform viewer;
        [SerializeField] private DataLogger logger;
        [SerializeField] private Material targetMaterial;

        public event Action TestCompleted;

        private GameObject leftTarget;
        private GameObject rightTarget;
        private readonly List<bool> closerLeftSchedule = new List<bool>();
        private int trialIndex;
        private float stimulusOnsetTime;
        private float referenceDepth;
        private bool training;

        public bool IsRunning { get; private set; }
        public VACCondition CurrentCondition { get; private set; }
        public DepthTestPhase CurrentPhase { get; private set; }

        public void Configure(ExperimentConfig experimentConfig, Transform viewerTransform, DataLogger dataLogger)
        {
            config = experimentConfig;
            viewer = viewerTransform;
            logger = dataLogger;
        }

        public void StartTraining(VACCondition condition = VACCondition.Low)
        {
            int trials = config != null ? config.trainingDepthTrials : 5;
            StartTest(condition, DepthTestPhase.Training, trials, true);
        }

        public void StartFormal(VACCondition condition, DepthTestPhase phase)
        {
            int trials = config != null ? config.formalDepthTrials : 20;
            StartTest(condition, phase, trials, false);
        }

        public void SubmitLeft() => SubmitResponse(true);
        public void SubmitRight() => SubmitResponse(false);

        private void StartTest(VACCondition condition, DepthTestPhase phase, int trialCount, bool isTraining)
        {
            CleanupTargets();
            CurrentCondition = condition;
            CurrentPhase = phase;
            training = isTraining;
            trialIndex = 0;
            referenceDepth = config != null ? config.GetDistance(condition) : 1f;
            BuildBalancedSchedule(Mathf.Max(2, trialCount));
            IsRunning = true;
            logger?.LogEvent("DepthTestStarted",
                phase == DepthTestPhase.Pre ? ExperimentPhase.PreDepth :
                phase == DepthTestPhase.Post ? ExperimentPhase.PostDepth : ExperimentPhase.TrainingDepth,
                condition, $"Phase={phase};Trials={trialCount}");
            PresentTrial();
        }

        private void BuildBalancedSchedule(int trialCount)
        {
            closerLeftSchedule.Clear();
            int leftCount = trialCount / 2;
            int rightCount = trialCount - leftCount;
            for (int i = 0; i < leftCount; i++) closerLeftSchedule.Add(true);
            for (int i = 0; i < rightCount; i++) closerLeftSchedule.Add(false);

            for (int i = closerLeftSchedule.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (closerLeftSchedule[i], closerLeftSchedule[j]) = (closerLeftSchedule[j], closerLeftSchedule[i]);
            }
        }

        private void PresentTrial()
        {
            CleanupTargets();
            if (trialIndex >= closerLeftSchedule.Count)
            {
                FinishTest();
                return;
            }

            bool closerIsLeft = closerLeftSchedule[trialIndex];
            float delta = config != null ? config.depthDifferenceMeters : 0.05f;
            float nearDepth = Mathf.Max(0.1f, referenceDepth - delta * 0.5f);
            float farDepth = referenceDepth + delta * 0.5f;

            float leftDepth = closerIsLeft ? nearDepth : farDepth;
            float rightDepth = closerIsLeft ? farDepth : nearDepth;

            leftTarget = CreateTarget("DepthTarget_Left", true, leftDepth);
            rightTarget = CreateTarget("DepthTarget_Right", false, rightDepth);
            stimulusOnsetTime = Time.realtimeSinceStartup;
        }

        private GameObject CreateTarget(string name, bool left, float depth)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            target.name = name;
            Destroy(target.GetComponent<Collider>());
            if (targetMaterial != null)
                target.GetComponent<Renderer>().sharedMaterial = targetMaterial;

            float horizontalAngle = (config != null ? config.targetHorizontalAngleDegrees : 4f) * Mathf.Deg2Rad;
            float x = Mathf.Tan(horizontalAngle) * depth * (left ? -1f : 1f);

            Vector3 forward = Vector3.ProjectOnPlane(viewer.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude < 0.0001f) forward = viewer.forward.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            target.transform.position = viewer.position + forward * depth + right * x;

            float angularRadius = 0.5f * (config != null ? config.targetAngularSizeDegrees : 1f) * Mathf.Deg2Rad;
            float diameter = 2f * Mathf.Tan(angularRadius) * depth;
            target.transform.localScale = Vector3.one * diameter;
            return target;
        }

        private void SubmitResponse(bool answeredLeft)
        {
            if (!IsRunning || trialIndex >= closerLeftSchedule.Count) return;

            float rt = Time.realtimeSinceStartup - stimulusOnsetTime;
            bool closerIsLeft = closerLeftSchedule[trialIndex];
            bool correct = answeredLeft == closerIsLeft;

            if (!training)
            {
                logger?.LogDepthTrial(CurrentCondition, CurrentPhase, trialIndex + 1,
                    referenceDepth,
                    config != null ? config.depthDifferenceMeters : 0.05f,
                    closerIsLeft ? "Left" : "Right",
                    answeredLeft ? "Left" : "Right",
                    correct, rt);
            }

            trialIndex++;
            PresentTrial();
        }

        private void FinishTest()
        {
            CleanupTargets();
            IsRunning = false;
            logger?.LogEvent("DepthTestEnded",
                CurrentPhase == DepthTestPhase.Pre ? ExperimentPhase.PreDepth :
                CurrentPhase == DepthTestPhase.Post ? ExperimentPhase.PostDepth : ExperimentPhase.TrainingDepth,
                CurrentCondition, $"Phase={CurrentPhase}");
            TestCompleted?.Invoke();
        }

        private void CleanupTargets()
        {
            if (leftTarget != null) Destroy(leftTarget);
            if (rightTarget != null) Destroy(rightTarget);
            leftTarget = null;
            rightTarget = null;
        }
    }
}