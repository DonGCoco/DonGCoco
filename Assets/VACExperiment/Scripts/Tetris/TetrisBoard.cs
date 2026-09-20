using UnityEngine;

namespace VACExperiment
{
    public class TetrisBoard : MonoBehaviour
    {
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 20;
        [SerializeField] private float cellSize = 0.045f;
        [SerializeField] private Transform lockedBlocksRoot;

        private Transform[,] grid;

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;

        private void Awake()
        {
            grid = new Transform[width, height];
            if (lockedBlocksRoot == null)
            {
                var root = new GameObject("LockedBlocks");
                root.transform.SetParent(transform, false);
                lockedBlocksRoot = root.transform;
            }
        }

        public Vector3 CellToLocal(Vector2Int cell)
        {
            float x = (cell.x - (width - 1) * 0.5f) * cellSize;
            float y = (cell.y - (height - 1) * 0.5f) * cellSize;
            return new Vector3(x, y, 0f);
        }

        public bool IsValid(Vector2Int[] cells, Vector2Int origin)
        {
            foreach (var c in cells)
            {
                Vector2Int p = origin + c;
                if (p.x < 0 || p.x >= width || p.y < 0)
                    return false;
                if (p.y >= height)
                    continue;
                if (grid[p.x, p.y] != null)
                    return false;
            }
            return true;
        }

        public int Lock(TetrisPiece piece)
        {
            var cells = piece.CurrentCells;
            for (int i = 0; i < cells.Length; i++)
            {
                Vector2Int p = piece.Origin + cells[i];
                Transform block = piece.DetachBlock(i);
                if (block == null) continue;

                if (p.y >= height)
                {
                    Destroy(block.gameObject);
                    continue;
                }

                block.SetParent(lockedBlocksRoot, false);
                block.localPosition = CellToLocal(p);
                grid[p.x, p.y] = block;
            }

            int cleared = ClearFullLines();
            Destroy(piece.gameObject);
            return cleared;
        }

        public void ResetBoard()
        {
            if (grid == null)
                grid = new Transform[width, height];

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null)
                    Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }

        private int ClearFullLines()
        {
            int cleared = 0;
            for (int y = 0; y < height; y++)
            {
                bool full = true;
                for (int x = 0; x < width; x++)
                {
                    if (grid[x, y] == null)
                    {
                        full = false;
                        break;
                    }
                }

                if (!full) continue;

                ClearLine(y);
                DropAbove(y);
                y--;
                cleared++;
            }
            return cleared;
        }

        private void ClearLine(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                    Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }

        private void DropAbove(int clearedY)
        {
            for (int y = clearedY + 1; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] == null) continue;
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].localPosition = CellToLocal(new Vector2Int(x, y - 1));
            }
        }
    }
}