using MathKids.Application.Abstractions;
using MathKids.Application.Exercises;
using MathKids.Domain.Exercises;

namespace MathKids.Tests.Exercises;

public sealed class AdditionExerciseGeneratorTests
{
    [Fact]
    public void Generate_CreatesAdditionWithCorrectResult()
    {
        var exercise = CreateGenerator().Generate(DifficultyLevel.Beginner);
        Assert.Equal(MathOperation.Addition, exercise.Operation);
        Assert.Equal(exercise.LeftOperand + exercise.RightOperand, exercise.CorrectAnswer);
        Assert.True(exercise.IsCorrect(exercise.CorrectAnswer));
    }

    [Theory]
    [InlineData(DifficultyLevel.Beginner, 1, 8)]
    [InlineData(DifficultyLevel.Intermediate, 5, 30)]
    [InlineData(DifficultyLevel.Advanced, 10, 100)]
    public void Generate_RespectsOperandRange(DifficultyLevel difficulty, int minimum, int maximum)
    {
        var generator = CreateGenerator();
        for (var index = 0; index < 100; index++)
        {
            var exercise = generator.Generate(difficulty);
            Assert.InRange(exercise.LeftOperand, minimum, maximum);
            Assert.InRange(exercise.RightOperand, minimum, maximum);
        }
    }

    [Fact]
    public void Generate_CreatesThreeUniqueOptionsWithOneCorrectAnswer()
    {
        var generator = CreateGenerator();
        for (var index = 0; index < 100; index++)
        {
            var exercise = generator.Generate(DifficultyLevel.Beginner);
            Assert.Equal(3, exercise.Options.Count);
            Assert.Equal(3, exercise.Options.Select(option => option.Value).Distinct().Count());
            Assert.Single(exercise.Options, option => option.IsCorrect);
        }
    }

    [Theory]
    [InlineData(MathOperation.Addition)]
    [InlineData(MathOperation.Subtraction)]
    [InlineData(MathOperation.Multiplication)]
    [InlineData(MathOperation.Division)]
    public void Generate_CreatesValidExerciseForEveryOperation(MathOperation operation)
    {
        var generator = CreateGenerator();
        for (var index = 0; index < 50; index++)
        {
            var exercise = generator.Generate(operation, DifficultyLevel.Beginner);
            var expected = operation switch
            {
                MathOperation.Addition => exercise.LeftOperand + exercise.RightOperand,
                MathOperation.Subtraction => exercise.LeftOperand - exercise.RightOperand,
                MathOperation.Multiplication => exercise.LeftOperand * exercise.RightOperand,
                MathOperation.Division => exercise.LeftOperand / exercise.RightOperand,
                _ => throw new ArgumentOutOfRangeException(nameof(operation))
            };
            Assert.Equal(operation, exercise.Operation);
            Assert.Equal(expected, exercise.CorrectAnswer);
            Assert.True(exercise.LeftOperand >= 0);
            Assert.True(exercise.RightOperand > 0);
            if (operation == MathOperation.Subtraction) Assert.True(exercise.LeftOperand >= exercise.RightOperand);
            if (operation == MathOperation.Division) Assert.Equal(0, exercise.LeftOperand % exercise.RightOperand);
            if (operation == MathOperation.Multiplication) Assert.InRange(exercise.CorrectAnswer, 1, 9);
            if (operation == MathOperation.Division) Assert.InRange(exercise.LeftOperand, 1, 9);
        }
    }

    private static AdditionExerciseGenerator CreateGenerator() => new(new SeededRandomProvider(1729));
    private sealed class SeededRandomProvider(int seed) : IRandomProvider
    {
        private readonly Random _random = new(seed);
        public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }
}
