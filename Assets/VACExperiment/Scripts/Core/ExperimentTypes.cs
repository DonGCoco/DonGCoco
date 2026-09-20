namespace VACExperiment
{
    public enum VACCondition
    {
        Low,
        High
    }

    public enum TetrisSequenceId
    {
        A,
        B
    }

    public enum ExperimentPhase
    {
        NotConfigured,
        TrainingTetris,
        TrainingDepth,
        BaselineQuestionnaireExternal,
        PreDepth,
        Tetris,
        PostDepth,
        PostConditionQuestionnaireExternal,
        Recovery,
        Complete
    }

    public enum DepthTestPhase
    {
        Training,
        Pre,
        Post
    }
}