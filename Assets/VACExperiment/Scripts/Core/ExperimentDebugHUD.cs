using UnityEngine;

namespace VACExperiment
{
    public class ExperimentDebugHUD : MonoBehaviour
    {
        [SerializeField] private ExperimentManager experiment;
        private string participantId = "P01";
        private Rect windowRect = new Rect(20, 20, 360, 390);

        private void OnGUI()
        {
            windowRect = GUI.Window(2026, windowRect, DrawWindow, "VAC Experiment Control");
        }

        private void DrawWindow(int id)
        {
            GUILayout.Label($"Phase: {(experiment != null ? experiment.Phase.ToString() : "NO MANAGER")}");
            GUILayout.Space(8);

            GUILayout.Label("Participant ID");
            participantId = GUILayout.TextField(participantId);
            if (GUILayout.Button("Configure Participant"))
                experiment?.ConfigureParticipant(participantId);

            GUILayout.Space(8);
            if (GUILayout.Button("Start Tetris Training"))
                experiment?.StartTetrisTraining();

            if (GUILayout.Button("Finish Tetris Training / Start Depth Practice"))
                experiment?.FinishTetrisTrainingAndStartDepthPractice();

            GUILayout.Space(8);
            GUILayout.Label("Questionnaires are completed OUTSIDE the headset.");

            if (GUILayout.Button("Baseline Questionnaire Completed"))
                experiment?.ConfirmBaselineQuestionnaireComplete();

            if (GUILayout.Button("Post-Condition Questionnaire Completed"))
                experiment?.ConfirmPostConditionQuestionnaireComplete();

            if (GUILayout.Button("Recovery Complete / Start Condition 2"))
                experiment?.StartSecondConditionAfterRecovery();

            GUILayout.Space(8);
            if (GUILayout.Button("ABORT EXPERIMENT"))
                experiment?.AbortExperiment("Manual abort from researcher HUD");

            GUILayout.Space(8);
            GUILayout.Label("Editor input:");
            GUILayout.Label("Tetris: arrows/WASD + Space");
            GUILayout.Label("Depth: Left/A or Right/D");

            GUI.DragWindow();
        }
    }
}