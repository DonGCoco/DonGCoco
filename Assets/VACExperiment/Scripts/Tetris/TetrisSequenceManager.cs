using System;
using System.Collections.Generic;
using UnityEngine;

namespace VACExperiment
{
    public class TetrisSequenceManager : MonoBehaviour
    {
        [SerializeField] private int sequenceASeed = 104729;
        [SerializeField] private int sequenceBSeed = 130363;
        [SerializeField] private int preGeneratedPieceCount = 512;

        private readonly List<TetrominoType> sequence = new List<TetrominoType>();
        private int index;
        public TetrisSequenceId CurrentSequenceId { get; private set; }

        public void SetSequence(TetrisSequenceId id)
        {
            CurrentSequenceId = id;
            sequence.Clear();
            index = 0;
            GenerateBalancedSevenBagSequence(id == TetrisSequenceId.A ? sequenceASeed : sequenceBSeed);
        }

        public TetrominoType Next()
        {
            if (sequence.Count == 0)
                SetSequence(TetrisSequenceId.A);

            if (index >= sequence.Count)
                index = 0;

            return sequence[index++];
        }

        private void GenerateBalancedSevenBagSequence(int seed)
        {
            var rng = new System.Random(seed);
            var bag = new List<TetrominoType>((TetrominoType[])Enum.GetValues(typeof(TetrominoType)));

            while (sequence.Count < preGeneratedPieceCount)
            {
                for (int i = bag.Count - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    (bag[i], bag[j]) = (bag[j], bag[i]);
                }

                foreach (var piece in bag)
                {
                    if (sequence.Count >= preGeneratedPieceCount) break;
                    sequence.Add(piece);
                }
            }
        }
    }
}