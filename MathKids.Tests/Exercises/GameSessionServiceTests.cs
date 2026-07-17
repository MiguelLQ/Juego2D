using MathKids.Application.Sessions;
using MathKids.Domain.Exercises;

namespace MathKids.Tests.Exercises;

public sealed class GameSessionServiceTests
{
    [Fact]
    public void Evaluate_ReportsSelectedAnswerAndCorrectness()
    {
        var exercise = new MathExercise(Guid.NewGuid(), MathOperation.Addition, DifficultyLevel.Beginner, 2, 3, 5, [new(4, false), new(5, true), new(6, false)]);
        var result = new GameSessionService().Evaluate(exercise, 5, TimeSpan.FromSeconds(2));
        Assert.True(result.IsCorrect);
        Assert.Equal(5, result.SelectedAnswer);
        Assert.Equal(TimeSpan.FromSeconds(2), result.Elapsed);
    }
}
