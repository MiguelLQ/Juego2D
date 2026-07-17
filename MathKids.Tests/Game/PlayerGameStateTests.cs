using MathKids.Application.Abstractions;
using MathKids.Application.Progress;
using MathKids.Game.Core;
using MathKids.Infrastructure.Persistence;

namespace MathKids.Tests.Game;

public sealed class PlayerGameStateTests
{
    [Fact]
    public void CorrectAnswer_AddsProgressAndRewards()
    {
        var state = CreateState();

        state.RecordCorrectAnswer("addition");

        Assert.Equal(1, state.CorrectAnswers);
        Assert.Equal(1, state.CompletedChallenges);
        Assert.Equal(1, state.Stars);
        Assert.Equal(30, state.Coins);
        Assert.Equal("addition", state.LastGame);
    }

    [Fact]
    public void FailedAttempt_OnlyAddsChallenge()
    {
        var state = CreateState();

        state.RecordAttempt("addition_bingo");

        Assert.Equal(1, state.CompletedChallenges);
        Assert.Equal(0, state.CorrectAnswers);
        Assert.Equal(0, state.Stars);
        Assert.Equal("addition_bingo", state.LastGame);
    }

    private static PlayerGameState CreateState()
    {
        var repository = new InMemoryProgressRepository();
        var service = new ProgressService(repository, new FixedClock());
        return new PlayerGameState(service);
    }

    private sealed class FixedClock : IClock
    {
        public DateTimeOffset UtcNow => new(2026, 7, 16, 23, 0, 0, TimeSpan.Zero);
    }
}
