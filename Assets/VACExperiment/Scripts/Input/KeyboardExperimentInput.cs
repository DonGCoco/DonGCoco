using UnityEngine;

namespace VACExperiment
{
    public class KeyboardExperimentInput : MonoBehaviour
    {
        [SerializeField] private TetrisManager tetris;
        [SerializeField] private DepthJudgmentManager depth;

        private void Update()
        {
            if (depth != null && depth.IsRunning)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A))
                    depth.SubmitLeft();
                if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D))
                    depth.SubmitRight();
                return;
            }

            if (tetris == null || !tetris.IsRunning) return;

            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A)) tetris.MoveLeft();
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D)) tetris.MoveRight();
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.W)) tetris.Rotate();
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || UnityEngine.Input.GetKeyDown(KeyCode.S)) tetris.SoftDrop();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) tetris.HardDrop();
        }
    }
}