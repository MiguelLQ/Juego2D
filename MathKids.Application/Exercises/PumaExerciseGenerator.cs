using MathKids.Application.Abstractions;
using MathKids.Domain.Exercises;

namespace MathKids.Application.Exercises;

public sealed class PumaExerciseGenerator(IRandomProvider randomProvider) : IPumaExerciseGenerator
{
    public MathExercise Generate()
    {
        var left = randomProvider.Next(1, 7);
        var right = randomProvider.Next(1, 7);
        var answer = left + right;
        var values = new[] { answer, 0, 0 };
        for (var index = 1; index < values.Length; index++)
        {
            int candidate;
            do candidate = Math.Clamp(answer + randomProvider.Next(-3, 4), 0, 12);
            while (Array.IndexOf(values, candidate, 0, index) >= 0);
            values[index] = candidate;
        }
        for (var index = values.Length - 1; index > 0; index--)
        {
            var swapIndex = randomProvider.Next(0, index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }
        var options = new AnswerOption[3];
        for (var index = 0; index < options.Length; index++) options[index] = new(values[index], values[index] == answer);
        return new MathExercise(Guid.NewGuid(), MathOperation.Addition, DifficultyLevel.Beginner, left, right, answer, options);
    }
}
