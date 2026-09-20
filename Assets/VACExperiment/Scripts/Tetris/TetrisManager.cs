using System;
using UnityEngine;

namespace VACExperiment
{
    public class TetrisManager : MonoBehaviour
    {
        [SerializeField] private ExperimentConfig config;
        [SerializeField] private TetrisBoard board;
        [SerializeField] private TetrisSequenceManager sequenceManager;
        [SerializeField] private DataLogger logger;
        [SerializeField] private Material blockMaterial;

        public event Action FormalRunCompleted;

        private TetrisPiece currentPiece;
        private float dropTimer;
        private float runStartTime;
        private float pieceSpawnTime;
        private float formalDuration;
        private int score;
        private int lines;
        private int piecesPlaced;
        private int topouts;
        private float placementTimeSum;
        private bool training;

        public bool IsRunning { get; private set; }
        public VACCondition CurrentCondition { get; private set; }
        public TetrisSequenceId CurrentSequence { get; private set; }

        public void StartTraining(TetrisSequenceId sequence = TetrisSequenceId.A)
        {
            ResetRun();
            training = true;
            CurrentSequence = sequence;
            sequenceManager.SetSequence(sequence);
            IsRunning = true;
            SpawnNext();
        }

        public void StopTraining()
        {
            IsRunning = false;
            training = false;
            if (currentPiece != null) Destroy(currentPiece.gameObject);
            currentPiece = null;
            board.ResetBoard();
        }

        public void StartFormal(VACCondition condition, TetrisSequenceId sequence)
        {
            ResetRun();
            training = false;
            CurrentCondition = condition;
            CurrentSequence = sequence;
            formalDuration = config != null ? config.formalTetrisDurationSeconds : 900f;
            sequenceManager.SetSequence(sequence);
            runStartTime = Time.realtimeSinceStartup;
            IsRunning = true;
            SpawnNext();
            logger?.LogEvent("TetrisStarted", ExperimentPhase.Tetris, condition, $"Sequence={sequence}");
        }

        private void Update()
        {
            if (!IsRunning || currentPiece == null) return;

            if (!training && Time.realtimeSinceStartup - runStartTime >= formalDuration)
            {
                EndFormalRun();
                return;
            }

            dropTimer += Time.deltaTime;
            float interval = training
                ? (config != null ? config.trainingDropIntervalSeconds : 0.8f)
                : (config != null ? config.formalDropIntervalSeconds : 0.8f);

            if (dropTimer >= interval)
            {
                dropTimer = 0f;
                if (!currentPiece.TryMove(Vector2Int.down))
                    LockCurrentPiece();
            }
        }

        public void MoveLeft() { if (IsRunning) currentPiece?.TryMove(Vector2Int.left); }
        public void MoveRight() { if (IsRunning) currentPiece?.TryMove(Vector2Int.right); }
        public void Rotate() { if (IsRunning) currentPiece?.TryRotateClockwise(); }

        public void SoftDrop()
        {
            if (!IsRunning || currentPiece == null) return;
            if (!currentPiece.TryMove(Vector2Int.down)) LockCurrentPiece();
        }

        public void HardDrop()
        {
            if (!IsRunning || currentPiece == null) return;
            currentPiece.HardDrop();
            LockCurrentPiece();
        }

        private void ResetRun()
        {
            IsRunning = false;
            if (currentPiece != null) Destroy(currentPiece.gameObject);
            currentPiece = null;
            board.ResetBoard();
            score = 0;
            lines = 0;
            piecesPlaced = 0;
            topouts = 0;
            placementTimeSum = 0f;
            dropTimer = 0f;
        }

        private void SpawnNext()
        {
            TetrominoType type = sequenceManager.Next();
            GameObject go = new GameObject($"Piece_{type}");
            go.transform.SetParent(board.transform, false);
            currentPiece = go.AddComponent<TetrisPiece>();
            currentPiece.Initialize(board, type, new Vector2Int(board.Width / 2 - 1, board.Height), blockMaterial);
            pieceSpawnTime = Time.realtimeSinceStartup;

            if (!board.IsValid(currentPiece.CurrentCells, currentPiece.Origin))
            {
                topouts++;
                if (training)
                {
                    board.ResetBoard();
                    Destroy(currentPiece.gameObject);
                    currentPiece = null;
                    SpawnNext();
                }
                else
                {
                    EndFormalRun();
                }
            }
        }

        private void LockCurrentPiece()
        {
            if (currentPiece == null) return;
            float placementSeconds = Time.realtimeSinceStartup - pieceSpawnTime;
            string pieceType = currentPiece.Type.ToString();
            int pieceIndex = piecesPlaced + 1;
            int cleared = board.Lock(currentPiece);
            currentPiece = null;

            piecesPlaced++;
            lines += cleared;
            int scoreIncrement = ScoreForLines(cleared);
            score += scoreIncrement;
            placementTimeSum += placementSeconds;

            if (!training)
                logger?.LogTetrisPiece(CurrentCondition, CurrentSequence, pieceIndex, pieceType,
                    placementSeconds, cleared, score);

            SpawnNext();
        }

        private static int ScoreForLines(int cleared)
        {
            switch (cleared)
            {
                case 1: return 40;
                case 2: return 100;
                case 3: return 300;
                case 4: return 1200;
                default: return 0;
            }
        }

        private void EndFormalRun()
        {
            if (!IsRunning) return;
            IsRunning = false;
            float duration = Mathf.Max(0f, Time.realtimeSinceStartup - runStartTime);
            if (currentPiece != null)
            {
                Destroy(currentPiece.gameObject);
                currentPiece = null;
            }

            float avgPlacement = piecesPlaced > 0 ? placementTimeSum / piecesPlaced : 0f;
            logger?.LogTetrisSummary(CurrentCondition, CurrentSequence, duration, score, lines,
                piecesPlaced, avgPlacement, topouts);
            logger?.LogEvent("TetrisEnded", ExperimentPhase.Tetris, CurrentCondition,
                $"Score={score};Lines={lines};Pieces={piecesPlaced}");
            FormalRunCompleted?.Invoke();
        }
    }
}