using System;
using System.Linq;

namespace VACExperiment
{
    [Serializable]
    public struct ConditionAssignment
    {
        public VACCondition condition;
        public TetrisSequenceId sequence;

        public ConditionAssignment(VACCondition condition, TetrisSequenceId sequence)
        {
            this.condition = condition;
            this.sequence = sequence;
        }
    }

    [Serializable]
    public struct ParticipantAssignment
    {
        public string participantId;
        public ConditionAssignment first;
        public ConditionAssignment second;

        public static ParticipantAssignment FromParticipantId(string rawId)
        {
            string id = string.IsNullOrWhiteSpace(rawId) ? "P01" : rawId.Trim().ToUpperInvariant();
            int number = ParseNumber(id);
            int cell = Math.Abs(number - 1) % 4;

            ConditionAssignment first;
            ConditionAssignment second;

            switch (cell)
            {
                case 0:
                    first = new ConditionAssignment(VACCondition.Low, TetrisSequenceId.A);
                    second = new ConditionAssignment(VACCondition.High, TetrisSequenceId.B);
                    break;
                case 1:
                    first = new ConditionAssignment(VACCondition.High, TetrisSequenceId.A);
                    second = new ConditionAssignment(VACCondition.Low, TetrisSequenceId.B);
                    break;
                case 2:
                    first = new ConditionAssignment(VACCondition.Low, TetrisSequenceId.B);
                    second = new ConditionAssignment(VACCondition.High, TetrisSequenceId.A);
                    break;
                default:
                    first = new ConditionAssignment(VACCondition.High, TetrisSequenceId.B);
                    second = new ConditionAssignment(VACCondition.Low, TetrisSequenceId.A);
                    break;
            }

            return new ParticipantAssignment
            {
                participantId = id,
                first = first,
                second = second
            };
        }

        private static int ParseNumber(string id)
        {
            string digits = new string(id.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out int n) && n > 0)
                return n;

            unchecked
            {
                int hash = 17;
                foreach (char c in id)
                    hash = hash * 31 + c;
                return Math.Abs(hash == int.MinValue ? int.MaxValue : hash) + 1;
            }
        }
    }
}