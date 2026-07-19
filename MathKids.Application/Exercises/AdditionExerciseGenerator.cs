using MathKids.Application.Abstractions;
using MathKids.Domain.Exercises;

namespace MathKids.Application.Exercises;

public sealed class AdditionExerciseGenerator(IRandomProvider randomProvider) : IExerciseGenerator
{
    public MathExercise Generate(DifficultyLevel difficulty) => Generate(MathOperation.Addition, difficulty);

    public MathExercise Generate(MathOperation operation, DifficultyLevel difficulty)
    {
        var (minimum, maximum) = difficulty switch
        {
            DifficultyLevel.Beginner => (1, 8),
            DifficultyLevel.Intermediate => (5, 30),
            DifficultyLevel.Advanced => (10, 100),
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
        };
        var (left, right, answer, maximumAnswer) = CreateOperation(operation, minimum, maximum, difficulty);
        var values = CreateUniqueOptions(answer, maximumAnswer);
        Shuffle(values);
        var options = new AnswerOption[values.Length];
        for (var index = 0; index < values.Length; index++)
            options[index] = new AnswerOption(values[index], values[index] == answer);
        return new MathExercise(Guid.NewGuid(), operation, difficulty, left, right, answer, options);
    }

    private (int Left, int Right, int Answer, int MaximumAnswer) CreateOperation(MathOperation operation, int minimum, int maximum, DifficultyLevel difficulty)
    {
        if (operation == MathOperation.Multiplication)
        {
            var factorMaximum = difficulty == DifficultyLevel.Beginner ? 3 : difficulty == DifficultyLevel.Intermediate ? 10 : 12;
            var leftFactor = randomProvider.Next(1, factorMaximum + 1);
            var rightFactor = randomProvider.Next(1, factorMaximum + 1);
            return (leftFactor, rightFactor, leftFactor * rightFactor, factorMaximum * factorMaximum);
        }
        if (operation == MathOperation.Division)
        {
            var factorMaximum = difficulty == DifficultyLevel.Beginner ? 3 : difficulty == DifficultyLevel.Intermediate ? 10 : 12;
            var divisor = randomProvider.Next(1, factorMaximum + 1);
            var quotient = randomProvider.Next(1, factorMaximum + 1);
            return (divisor * quotient, divisor, quotient, factorMaximum);
        }

        var left = randomProvider.Next(minimum, maximum + 1);
        var right = randomProvider.Next(minimum, maximum + 1);
        if (operation == MathOperation.Subtraction && right > left) (left, right) = (right, left);
        var answer = operation == MathOperation.Addition ? left + right : left - right;
        return (left, right, answer, operation == MathOperation.Addition ? maximum * 2 : maximum);
    }

    private int[] CreateUniqueOptions(int answer, int maximumAnswer)
    {
        var values = new[] { answer, 0, 0 };
        for (var index = 1; index < values.Length; index++)
        {
            int candidate;
            do
            {
                candidate = Math.Clamp(answer + randomProvider.Next(-2, 3), 0, maximumAnswer);
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
