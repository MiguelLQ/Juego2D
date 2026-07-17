using MathKids.Domain.Exercises;

namespace MathKids.Application.Sessions;

public interface IGameSessionService { ExerciseResult Evaluate(MathExercise exercise, int selectedAnswer, TimeSpan elapsed); }
