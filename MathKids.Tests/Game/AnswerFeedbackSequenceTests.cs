using MathKids.Game.Scenes.Addition;

namespace MathKids.Tests.Game;

public sealed class AnswerFeedbackSequenceTests
{
    [Fact]
    public void IncorrectAnswer_CompletesAsRetry()
    {
        var sequence = new AnswerFeedbackSequence();
        sequence.Start(false);

        Assert.Equal(AnswerFeedbackCompletion.None, sequence.Update(0.5f));
        Assert.Equal(AnswerFeedbackCompletion.Retry, sequence.Update(0.25f));
        Assert.False(sequence.IsActive);
    }

    [Fact]
    public void CorrectAnswer_CompletesAsNextExercise()
    {
        var sequence = new AnswerFeedbackSequence();
        sequence.Start(true);

        Assert.Equal(AnswerFeedbackCompletion.None, sequence.Update(1f));
        Assert.Equal(AnswerFeedbackCompletion.NextExercise, sequence.Update(0.4f));
        Assert.False(sequence.IsActive);
    }
}
