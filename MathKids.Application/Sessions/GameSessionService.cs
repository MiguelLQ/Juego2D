using MathKids.Domain.Exercises;

namespace MathKids.Application.Sessions;

public sealed class GameSessionService : IGameSessionService
{
    public ExerciseResult Evaluate(MathExercise exercise, int selectedAnswer, TimeSpan elapsed)
    {
        ArgumentNullException.ThrowIfNull(exercise);
        return new ExerciseResult(exercise.Id, selectedAnswer, exercise.CorrectAnswer, exercise.IsCorrect(selectedAnswer), elapsed);
    }
}
