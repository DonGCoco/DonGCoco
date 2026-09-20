using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace VACExperiment
{
    public class DataLogger : MonoBehaviour
    {
        [SerializeField] private ExperimentConfig config;

        private string participantId = "UNSET";
        private string eventPath;
        private string tetrisSummaryPath;
        private string tetrisPiecesPath;
        private string depthTrialsPath;

        public void Configure(ExperimentConfig experimentConfig)
        {
            config = experimentConfig;
        }

        public void BeginParticipant(string id)
        {
            participantId = string.IsNullOrWhiteSpace(id) ? "UNSET" : id.Trim();
            string root = Path.Combine(Application.persistentDataPath,
                config != null ? config.outputFolderName : "VACExperimentData");
            string sessionFolder = Path.Combine(root, participantId);
            Directory.CreateDirectory(sessionFolder);

            eventPath = Path.Combine(sessionFolder, "events.csv");
            tetrisSummaryPath = Path.Combine(sessionFolder, "tetris_summary.csv");
            tetrisPiecesPath = Path.Combine(sessionFolder, "tetris_pieces.csv");
            depthTrialsPath = Path.Combine(sessionFolder, "depth_trials.csv");

            EnsureHeader(eventPath, "participant_id,timestamp,event,phase,condition,details");
            EnsureHeader(tetrisSummaryPath, "participant_id,condition,sequence,duration_s,score,lines,lines_per_min,pieces,avg_placement_s,topouts");
            EnsureHeader(tetrisPiecesPath, "participant_id,condition,sequence,piece_index,piece_type,placement_s,lines_cleared,score_after");
            EnsureHeader(depthTrialsPath, "participant_id,condition,phase,trial,reference_depth_m,depth_difference_m,closer_side,response,correct,reaction_time_s");

            LogEvent("ParticipantStarted", ExperimentPhase.NotConfigured, null, Application.persistentDataPath);
        }

        public void LogEvent(string eventName, ExperimentPhase phase, VACCondition? condition, string details = "")
        {
            if (string.IsNullOrEmpty(eventPath)) return;
            Append(eventPath, string.Join(",",
                Csv(participantId),
                Csv(DateTime.Now.ToString("o", CultureInfo.InvariantCulture)),
                Csv(eventName),
                Csv(phase.ToString()),
                Csv(condition.HasValue ? condition.Value.ToString() : ""),
                Csv(details)));
        }

        public void LogTetrisPiece(VACCondition condition, TetrisSequenceId sequence, int pieceIndex,
            string pieceType, float placementSeconds, int linesCleared, int scoreAfter)
        {
            Append(tetrisPiecesPath, string.Join(",",
                Csv(participantId), Csv(condition.ToString()), Csv(sequence.ToString()),
                pieceIndex.ToString(CultureInfo.InvariantCulture), Csv(pieceType),
                placementSeconds.ToString("F4", CultureInfo.InvariantCulture),
                linesCleared.ToString(CultureInfo.InvariantCulture),
                scoreAfter.ToString(CultureInfo.InvariantCulture)));
        }

        public void LogTetrisSummary(VACCondition condition, TetrisSequenceId sequence, float durationSeconds,
            int score, int lines, int pieces, float averagePlacementSeconds, int topouts)
        {
            float lpm = durationSeconds > 0f ? lines / (durationSeconds / 60f) : 0f;
            Append(tetrisSummaryPath, string.Join(",",
                Csv(participantId), Csv(condition.ToString()), Csv(sequence.ToString()),
                durationSeconds.ToString("F3", CultureInfo.InvariantCulture),
                score.ToString(CultureInfo.InvariantCulture),
                lines.ToString(CultureInfo.InvariantCulture),
                lpm.ToString("F4", CultureInfo.InvariantCulture),
                pieces.ToString(CultureInfo.InvariantCulture),
                averagePlacementSeconds.ToString("F4", CultureInfo.InvariantCulture),
                topouts.ToString(CultureInfo.InvariantCulture)));
        }

        public void LogDepthTrial(VACCondition condition, DepthTestPhase phase, int trial,
            float referenceDepth, float depthDifference, string closerSide, string response,
            bool correct, float reactionTime)
        {
            Append(depthTrialsPath, string.Join(",",
                Csv(participantId), Csv(condition.ToString()), Csv(phase.ToString()),
                trial.ToString(CultureInfo.InvariantCulture),
                referenceDepth.ToString("F4", CultureInfo.InvariantCulture),
                depthDifference.ToString("F4", CultureInfo.InvariantCulture),
                Csv(closerSide), Csv(response),
                correct ? "1" : "0",
                reactionTime.ToString("F4", CultureInfo.InvariantCulture)));
        }

        private static void EnsureHeader(string path, string header)
        {
            if (!File.Exists(path)) File.WriteAllText(path, header + Environment.NewLine);
        }

        private static void Append(string path, string row)
        {
            if (string.IsNullOrEmpty(path)) return;
            File.AppendAllText(path, row + Environment.NewLine);
        }

        private static string Csv(string value)
        {
            value ??= "";
            if (value.Contains(",") || value.Contains(""") || value.Contains("\n"))
                return """ + value.Replace(""", """") + """;
            return value;
        }
    }
}