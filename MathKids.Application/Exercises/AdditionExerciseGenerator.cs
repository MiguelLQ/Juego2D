using MathKids.Application.Abstractions;
using MathKids.Domain.Exercises;

namespace MathKids.Application.Exercises;

public sealed class AdditionExerciseGenerator(IRandomProvider randomProvider) : IExerciseGenerator
{
    public MathExercise Generate(DifficultyLevel difficulty)
    {
        var (minimum, maximum) = difficulty switch
        {
            DifficultyLevel.Beginner => (1, 10),
            DifficultyLevel.Intermediate => (5, 30),
            DifficultyLevel.Advanced => (10, 100),
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
        var left = randomProvider.Next(minimum, maximum + 1);
        var right = randomProvider.Next(minimum, maximum + 1);
        var answer = left + right;
        var values = CreateUniqueOptions(answer, maximum * 2);
        Shuffle(values);
        var options = new AnswerOption[values.Length];
        for (var index = 0; index < values.Length; index++)
            options[index] = new AnswerOption(values[index], values[index] == answer);
        return new MathExercise(Guid.NewGuid(), MathOperation.Addition, difficulty, left, right, answer, options);
    }

    private int[] CreateUniqueOptions(int answer, int maximumAnswer)
    {
        var values = new[] { answer, 0, 0 };
        for (var index = 1; index < values.Length; index++)
        {
            int candidate;
            do
            {
                candidate = Math.Clamp(answer + randomProvider.Next(-5, 6), 0, maximumAnswer);
            } while (Array.IndexOf(values, candidate, 0, index) >= 0);
            values[index] = candidate;
        }
        return values;
    }

    private void Shuffle(int[] values)
    {
        for (var index = values.Length - 1; index > 0; index--)
        {
            var swapIndex = randomProvider.Next(0, index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }
    }
}
