using System.Collections.Generic;
using UnityEngine;

namespace VACExperiment
{
    public class TetrisPiece : MonoBehaviour
    {
        private TetrisBoard board;
        private readonly List<Transform> blocks = new List<Transform>();
        private Vector2Int[] baseCells;
        private int rotationQuarterTurns;

        public TetrominoType Type { get; private set; }
        public Vector2Int Origin { get; private set; }
        public Vector2Int[] CurrentCells => GetRotatedCells(rotationQuarterTurns);

        public void Initialize(TetrisBoard targetBoard, TetrominoType type, Vector2Int spawnOrigin, Material material)
        {
            board = targetBoard;
            Type = type;
            Origin = spawnOrigin;
            baseCells = TetrominoShapes.GetCells(type);
            rotationQuarterTurns = 0;

            for (int i = 0; i < baseCells.Length; i++)
            {
                GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = $"Block_{i}";
                Destroy(block.GetComponent<Collider>());
                block.transform.SetParent(transform, false);
                block.transform.localScale = Vector3.one * board.CellSize * 0.92f;
                if (material != null)
                    block.GetComponent<Renderer>().sharedMaterial = material;
                blocks.Add(block.transform);
            }

            RefreshVisuals();
        }

        public bool TryMove(Vector2Int delta)
        {
            Vector2Int candidate = Origin + delta;
            if (!board.IsValid(CurrentCells, candidate))
                return false;
            Origin = candidate;
            RefreshVisuals();
            return true;
        }

        public bool TryRotateClockwise()
        {
            if (Type == TetrominoType.O) return true;
            int candidateRotation = (rotationQuarterTurns + 1) % 4;
            Vector2Int[] candidateCells = GetRotatedCells(candidateRotation);
            int[] kicks = { 0, -1, 1, -2, 2 };

            foreach (int kick in kicks)
            {
                Vector2Int candidateOrigin = Origin + new Vector2Int(kick, 0);
                if (!board.IsValid(candidateCells, candidateOrigin)) continue;
                rotationQuarterTurns = candidateRotation;
                Origin = candidateOrigin;
                RefreshVisuals();
                return true;
            }
            return false;
        }

        public int HardDrop()
        {
            int distance = 0;
            while (TryMove(Vector2Int.down)) distance++;
            return distance;
        }

        public Transform DetachBlock(int index)
        {
            if (index < 0 || index >= blocks.Count) return null;
            Transform t = blocks[index];
            blocks[index] = null;
            return t;
        }

        private Vector2Int[] GetRotatedCells(int turns)
        {
            var result = new Vector2Int[baseCells.Length];
            for (int i = 0; i < baseCells.Length; i++)
            {
                Vector2Int p = baseCells[i];
                if (Type == TetrominoType.O)
                {
                    result[i] = p;
                    continue;
                }

                for (int t = 0; t < turns; t++)
                    p = new Vector2Int(p.y, -p.x);
                result[i] = p;
            }
            return result;
        }

        private void RefreshVisuals()
        {
            Vector2Int[] cells = CurrentCells;
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i] == null) continue;
                Vector2Int boardCell = Origin + cells[i];
                blocks[i].localPosition = board.CellToLocal(boardCell);
            }
        }
    }
}