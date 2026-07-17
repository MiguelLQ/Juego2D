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
    [InlineData(DifficultyLevel.Beginner, 1, 10)]
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

    private static AdditionExerciseGenerator CreateGenerator() => new(new SeededRandomProvider(1729));
    private sealed class SeededRandomProvider(int seed) : IRandomProvider
    {
        private readonly Random _random = new(seed);
        public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }
}
