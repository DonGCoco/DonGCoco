using UnityEngine;

namespace VACExperiment
{
    public static class TetrominoShapes
    {
        public static Vector2Int[] GetCells(TetrominoType type)
        {
            switch (type)
            {
                case TetrominoType.I:
                    return new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) };
                case TetrominoType.O:
                    return new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) };
                case TetrominoType.T:
                    return new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,1) };
                case TetrominoType.S:
                    return new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) };
                case TetrominoType.Z:
                    return new[] { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(-1,1), new Vector2Int(0,1) };
                case TetrominoType.J:
                    return new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(-1,1) };
                case TetrominoType.L:
                default:
                    return new[] { new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(1,1) };
            }
        }
    }
}