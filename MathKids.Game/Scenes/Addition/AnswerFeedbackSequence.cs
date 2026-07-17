namespace MathKids.Game.Scenes.Addition;

public sealed class AnswerFeedbackSequence
{
    private const float IncorrectDurationSeconds = 0.7f;
    private const float CorrectDurationSeconds = 1.35f;
    private float _duration;

    public AnswerFeedbackState State { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public float Progress => _duration <= 0f ? 0f : Math.Clamp(ElapsedSeconds / _duration, 0f, 1f);
    public bool IsActive => State != AnswerFeedbackState.None;

    public void Start(bool isCorrect)
    {
        State = isCorrect ? AnswerFeedbackState.Correct : AnswerFeedbackState.Incorrect;
        _duration = isCorrect ? CorrectDurationSeconds : IncorrectDurationSeconds;
        ElapsedSeconds = 0f;
    }

    public AnswerFeedbackCompletion Update(float deltaSeconds)
    {
        if (!IsActive)
        {
            return AnswerFeedbackCompletion.None;
        }

        ElapsedSeconds += Math.Max(0f, deltaSeconds);
        if (ElapsedSeconds < _duration)
        {
            return AnswerFeedbackCompletion.None;
        }

        var completion = State == AnswerFeedbackState.Correct
            ? AnswerFeedbackCompletion.NextExercise
            : AnswerFeedbackCompletion.Retry;
        Reset();
        return completion;
    }

    public void Reset()
    {
        State = AnswerFeedbackState.None;
        ElapsedSeconds = 0f;
        _duration = 0f;
    }
}

public enum AnswerFeedbackState { None, Correct, Incorrect }
public enum AnswerFeedbackCompletion { None, Retry, NextExercise }
