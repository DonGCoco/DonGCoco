using UnityEngine;

namespace VACExperiment
{
    [CreateAssetMenu(fileName = "ExperimentConfig", menuName = "VAC Experiment/Experiment Config")]
    public class ExperimentConfig : ScriptableObject
    {
        [Header("VAC distances (PLACEHOLDERS until headset optics are confirmed)")]
        [Min(0.1f)] public float lowVACDistanceMeters = 1.0f;
        [Min(0.1f)] public float highVACDistanceMeters = 2.0f;

        [Header("Apparent-size reference")]
        [Min(0.1f)] public float referenceDistanceMeters = 1.0f;

        [Header("Tetris")]
        [Min(10f)] public float formalTetrisDurationSeconds = 900f;
        [Min(1f)] public float trainingDropIntervalSeconds = 0.8f;
        [Min(0.05f)] public float formalDropIntervalSeconds = 0.8f;

        [Header("Depth judgment")]
        [Min(2)] public int formalDepthTrials = 20;
        [Range(2, 10)] public int trainingDepthTrials = 5;
        [Min(0.001f)] public float depthDifferenceMeters = 0.05f;
        [Range(0.5f, 15f)] public float targetHorizontalAngleDegrees = 4f;
        [Range(0.1f, 5f)] public float targetAngularSizeDegrees = 1f;

        [Header("Data")]
        public string outputFolderName = "VACExperimentData";

        public float GetDistance(VACCondition condition)
        {
            return condition == VACCondition.Low ? lowVACDistanceMeters : highVACDistanceMeters;
        }
    }
}