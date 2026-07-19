using MathKids.Application.Abstractions;
using MathKids.Domain.Exercises;

namespace MathKids.Application.Exercises;

public sealed class PumaExerciseGenerator(IRandomProvider randomProvider) : IPumaExerciseGenerator
{
    public MathExercise Generate() => Generate(MathOperation.Addition);

    public MathExercise Generate(MathOperation operation)
    {
        var (left, right, answer) = CreateOperands(operation);
        var values = new[] { answer, 0, 0 };
        for (var index = 1; index < values.Length; index++)
        {
            int candidate;
            do candidate = Math.Clamp(answer + randomProvider.Next(-2, 3), 0, 12);
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
        return new MathExercise(Guid.NewGuid(), operation, DifficultyLevel.Beginner, left, right, answer, options);
    }

    private (int Left, int Right, int Answer) CreateOperands(MathOperation operation)
    {
        if (operation == MathOperation.Multiplication)
        {
            var leftFactor = randomProvider.Next(1, 4);
            var rightFactor = randomProvider.Next(1, 4);
            return (leftFactor, rightFactor, leftFactor * rightFactor);
        }
        if (operation == MathOperation.Division)
        {
            var divisor = randomProvider.Next(1, 4);
            var quotient = randomProvider.Next(1, 4);
            return (divisor * quotient, divisor, quotient);
        }
        var left = randomProvider.Next(1, 7);
        var right = randomProvider.Next(1, 7);
        if (operation == MathOperation.Subtraction && right > left) (left, right) = (right, left);
        return (left, right, operation == MathOperation.Addition ? left + right : left - right);
    }
}
