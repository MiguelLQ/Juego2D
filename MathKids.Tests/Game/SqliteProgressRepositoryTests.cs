using MathKids.Application.Abstractions;
using MathKids.Application.Progress;
using MathKids.Game.Core;
using MathKids.Infrastructure.Persistence;

namespace MathKids.Tests.Infrastructure;

public sealed class SqliteProgressRepositoryTests
{
    [Fact]
    public void Save_PersistsProgressAcrossRepositoryInstances()
    {
        var directory = Path.Combine(Path.GetTempPath(), "mathkids-tests", Guid.NewGuid().ToString("N"));
        var databasePath = Path.Combine(directory, "progress.db3");
        try
        {
            var options = new SqliteDatabaseOptions(databasePath);
            var firstState = CreateState(new SqliteProgressRepository(options));
            firstState.RecordCorrectAnswer("addition", stars: 2, coins: 8);
            firstState.RecordAttempt("addition_bingo");

            var restoredState = CreateState(new SqliteProgressRepository(options));

            Assert.Equal(2, restoredState.CompletedChallenges);
            Assert.Equal(1, restoredState.CorrectAnswers);
            Assert.Equal(2, restoredState.Stars);
            Assert.Equal(33, restoredState.Coins);
            Assert.Equal("addition_bingo", restoredState.LastGame);
            Assert.Equal(FixedClock.Timestamp, restoredState.UpdatedAtUtc);
        }
        finally
        {
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    [Fact]
    public void Get_UnknownProfile_ReturnsInitialValues()
    {
        var directory = Path.Combine(Path.GetTempPath(), "mathkids-tests", Guid.NewGuid().ToString("N"));
        var databasePath = Path.Combine(directory, "progress.db3");
        try
        {
            var repository = new SqliteProgressRepository(new(databasePath));

            var progress = repository.Get(Guid.NewGuid());

            Assert.Equal(0, progress.CompletedChallenges);
            Assert.Equal(0, progress.CorrectAnswers);
            Assert.Equal(0, progress.Stars);
            Assert.Equal(25, progress.Coins);
        }
        finally
        {
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
        }
    }

    private static PlayerGameState CreateState(IProgressRepository repository)
    {
        var service = new ProgressService(repository, new FixedClock());
        return new PlayerGameState(service);
    }

    private sealed class FixedClock : IClock
    {
        public static DateTimeOffset Timestamp { get; } = new(2026, 7, 16, 23, 15, 0, TimeSpan.Zero);
        public DateTimeOffset UtcNow => Timestamp;
    }
}
