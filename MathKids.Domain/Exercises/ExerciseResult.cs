namespace MathKids.Domain.Exercises;

public sealed record ExerciseResult(Guid ExerciseId, int SelectedAnswer, int CorrectAnswer, bool IsCorrect, TimeSpan Elapsed);
