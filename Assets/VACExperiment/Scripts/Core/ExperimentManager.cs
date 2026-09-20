using UnityEngine;

namespace VACExperiment
{
    public class ExperimentManager : MonoBehaviour
    {
        [SerializeField] private ExperimentConfig config;
        [SerializeField] private Transform viewer;
        [SerializeField] private VACController vacController;
        [SerializeField] private TetrisManager tetris;
        [SerializeField] private TetrisSequenceManager sequenceManager;
        [SerializeField] private DepthJudgmentManager depth;
        [SerializeField] private DataLogger logger;

        private ParticipantAssignment assignment;
        private int formalConditionIndex = -1;

        public ExperimentPhase Phase { get; private set; } = ExperimentPhase.NotConfigured;
        public string ParticipantId => assignment.participantId;
        public VACCondition CurrentCondition { get; private set; }
        public TetrisSequenceId CurrentSequence { get; private set; }

        private void Awake()
        {
            if (logger != null) logger.Configure(config);
            if (depth != null) depth.Configure(config, viewer, logger);

            if (depth != null) depth.TestCompleted += HandleDepthTestCompleted;
            if (tetris != null) tetris.FormalRunCompleted += HandleTetrisCompleted;
        }

        private void OnDestroy()
        {
            if (depth != null) depth.TestCompleted -= HandleDepthTestCompleted;
            if (tetris != null) tetris.FormalRunCompleted -= HandleTetrisCompleted;
        }

        public void ConfigureParticipant(string participantId)
        {
            assignment = ParticipantAssignment.FromParticipantId(participantId);
            formalConditionIndex = -1;
            logger?.BeginParticipant(assignment.participantId);
            Phase = ExperimentPhase.NotConfigured;
            logger?.LogEvent("AssignmentCreated", Phase, null,
                $"First={assignment.first.condition}+{assignment.first.sequence};Second={assignment.second.condition}+{assignment.second.sequence}");
        }

        public void StartTetrisTraining()
        {
            EnsureParticipant();
            Phase = ExperimentPhase.TrainingTetris;
            logger?.LogEvent("TetrisTrainingStarted", Phase, null);
            vacController.ApplyCondition(VACCondition.Low);
            tetris.StartTraining(TetrisSequenceId.A);
        }

        public void FinishTetrisTrainingAndStartDepthPractice()
        {
            tetris.StopTraining();
            Phase = ExperimentPhase.TrainingDepth;
            logger?.LogEvent("TetrisTrainingEnded", Phase, null);
            depth.StartTraining(VACCondition.Low);
        }

        public void ConfirmBaselineQuestionnaireComplete()
        {
            if (Phase != ExperimentPhase.BaselineQuestionnaireExternal)
                Debug.LogWarning($"Expected BaselineQuestionnaireExternal, current phase is {Phase}.");
            StartFormalCondition(0);
        }

        public void ConfirmPostConditionQuestionnaireComplete()
        {
            if (formalConditionIndex == 0)
            {
                Phase = ExperimentPhase.Recovery;
                logger?.LogEvent("RecoveryStarted", Phase, CurrentCondition);
            }
            else
            {
                Phase = ExperimentPhase.Complete;
                logger?.LogEvent("ExperimentCompleted", Phase, CurrentCondition);
            }
        }

        public void StartSecondConditionAfterRecovery()
        {
            if (formalConditionIndex != 0)
            {
                Debug.LogWarning("Second condition can only start after the first condition.");
                return;
            }
            StartFormalCondition(1);
        }

        public void AbortExperiment(string reason = "ExperimenterAbort")
        {
            logger?.LogEvent("ExperimentAborted", Phase,
                formalConditionIndex >= 0 ? CurrentCondition : (VACCondition?)null, reason);
            Phase = ExperimentPhase.Complete;
        }

        private void StartFormalCondition(int index)
        {
            EnsureParticipant();
            formalConditionIndex = index;
            ConditionAssignment ca = index == 0 ? assignment.first : assignment.second;
            CurrentCondition = ca.condition;
            CurrentSequence = ca.sequence;

            vacController.ApplyCondition(CurrentCondition);
            Phase = ExperimentPhase.PreDepth;
            logger?.LogEvent("ConditionStarted", Phase, CurrentCondition, $"Sequence={CurrentSequence};Index={index + 1}");
            depth.StartFormal(CurrentCondition, DepthTestPhase.Pre);
        }

        private void HandleDepthTestCompleted()
        {
            if (Phase == ExperimentPhase.TrainingDepth)
            {
                Phase = ExperimentPhase.BaselineQuestionnaireExternal;
                logger?.LogEvent("TrainingCompleted", Phase, null,
                    "Complete baseline questionnaire outside the headset, then confirm in app.");
                return;
            }

            if (Phase == ExperimentPhase.PreDepth)
            {
                Phase = ExperimentPhase.Tetris;
                tetris.StartFormal(CurrentCondition, CurrentSequence);
                return;
            }

            if (Phase == ExperimentPhase.PostDepth)
            {
                Phase = ExperimentPhase.PostConditionQuestionnaireExternal;
                logger?.LogEvent("ExternalQuestionnaireRequired", Phase, CurrentCondition,
                    "Complete post-condition questionnaire outside the headset.");
            }
        }

        private void HandleTetrisCompleted()
        {
            if (Phase != ExperimentPhase.Tetris) return;
            Phase = ExperimentPhase.PostDepth;
            depth.StartFormal(CurrentCondition, DepthTestPhase.Post);
        }

        private void EnsureParticipant()
        {
            if (string.IsNullOrEmpty(assignment.participantId))
                ConfigureParticipant("P01");
        }
    }
}