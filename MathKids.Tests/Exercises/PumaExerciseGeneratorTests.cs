using MathKids.Application.Abstractions;
using MathKids.Application.Exercises;

namespace MathKids.Tests.Exercises;

public sealed class PumaExerciseGeneratorTests
{
    [Fact]
    public void Generate_CreatesVisualFriendlyCookieCounts()
    {
        var generator = new PumaExerciseGenerator(new SeededRandomProvider(2026));
        for (var index = 0; index < 100; index++)
        {
            var exercise = generator.Generate();
            Assert.InRange(exercise.LeftOperand, 1, 6);
            Assert.InRange(exercise.RightOperand, 1, 6);
            Assert.Equal(exercise.LeftOperand + exercise.RightOperand, exercise.CorrectAnswer);
            Assert.Equal(3, exercise.Options.Select(option => option.Value).Distinct().Count());
        }
    }

    private sealed class SeededRandomProvider(int seed) : IRandomProvider
    {
        private readonly Random _random = new(seed);
        public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }
}
