using MathKids.Domain.Exercises;

namespace MathKids.Application.Exercises;

public interface IExerciseGenerator { MathExercise Generate(DifficultyLevel difficulty); }
