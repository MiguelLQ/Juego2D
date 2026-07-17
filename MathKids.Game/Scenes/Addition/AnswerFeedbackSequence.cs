namespace MathKids.Game.Scenes.Addition;

public sealed class AnswerFeedbackSequence
{
    private readonly float _incorrectDurationSeconds;
    private readonly float _correctDurationSeconds;
    private float _duration;

    public AnswerFeedbackSequence(float incorrectDurationSeconds = 0.7f, float correctDurationSeconds = 1.35f)
    {
        _incorrectDurationSeconds = Math.Max(0.1f, incorrectDurationSeconds);
        _correctDurationSeconds = Math.Max(0.1f, correctDurationSeconds);
    }

    public AnswerFeedbackState State { get; private set; }
    public float ElapsedSeconds { get; private set; }
    public float Progress => _duration <= 0f ? 0f : Math.Clamp(ElapsedSeconds / _duration, 0f, 1f);
    public bool IsActive => State != AnswerFeedbackState.None;

    public void Start(bool isCorrect)
    {
        State = isCorrect ? AnswerFeedbackState.Correct : AnswerFeedbackState.Incorrect;
        _duration = isCorrect ? _correctDurationSeconds : _incorrectDurationSeconds;
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
