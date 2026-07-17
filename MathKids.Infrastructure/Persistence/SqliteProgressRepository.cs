using System.Globalization;
using MathKids.Application.Progress;
using MathKids.Domain.Progress;
using Microsoft.Data.Sqlite;

namespace MathKids.Infrastructure.Persistence;

public sealed class SqliteProgressRepository : IProgressRepository
{
    private readonly string _connectionString;
    private readonly Lock _lock = new();

    public SqliteProgressRepository(SqliteDatabaseOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var databasePath = Path.GetFullPath(options.DatabasePath);
        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
            Pooling = false
        }.ToString();
        InitializeDatabase();
    }

    public PlayerProgress Get(Guid profileId)
    {
        lock (_lock)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT completed_challenges, correct_answers, stars, coins, updated_at_utc, last_game
                FROM player_progress
                WHERE profile_id = $profileId;
                """;
            command.Parameters.AddWithValue("$profileId", profileId.ToString("D"));
            using var reader = command.ExecuteReader();
            if (!reader.Read()) return PlayerProgress.Empty(profileId);
            var updatedAt = DateTimeOffset.TryParse(reader.GetString(4), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsed)
                ? parsed
                : DateTimeOffset.UnixEpoch;
            return new PlayerProgress(
                profileId,
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3),
                updatedAt,
                reader.IsDBNull(5) ? null : reader.GetString(5));
        }
    }

    public void Save(PlayerProgress progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        lock (_lock)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO player_progress (
                    profile_id,
                    completed_challenges,
                    correct_answers,
                    stars,
                    coins,
                    updated_at_utc,
                    last_game)
                VALUES ($profileId, $completedChallenges, $correctAnswers, $stars, $coins, $updatedAtUtc, $lastGame)
                ON CONFLICT(profile_id) DO UPDATE SET
                    completed_challenges = excluded.completed_challenges,
                    correct_answers = excluded.correct_answers,
                    stars = excluded.stars,
                    coins = excluded.coins,
                    updated_at_utc = excluded.updated_at_utc,
                    last_game = excluded.last_game;
                """;
            command.Parameters.AddWithValue("$profileId", progress.ProfileId.ToString("D"));
            command.Parameters.AddWithValue("$completedChallenges", progress.CompletedChallenges);
            command.Parameters.AddWithValue("$correctAnswers", progress.CorrectAnswers);
            command.Parameters.AddWithValue("$stars", progress.Stars);
            command.Parameters.AddWithValue("$coins", progress.Coins);
            command.Parameters.AddWithValue("$updatedAtUtc", progress.UpdatedAtUtc.ToString("O", CultureInfo.InvariantCulture));
            command.Parameters.AddWithValue("$lastGame", (object?)progress.LastGame ?? DBNull.Value);
            command.ExecuteNonQuery();
        }
    }

    private void InitializeDatabase()
    {
        lock (_lock)
        {
            using var connection = OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = """
                PRAGMA journal_mode = WAL;
                PRAGMA synchronous = NORMAL;
                PRAGMA busy_timeout = 5000;

                CREATE TABLE IF NOT EXISTS player_progress (
                    profile_id TEXT PRIMARY KEY NOT NULL,
                    completed_challenges INTEGER NOT NULL DEFAULT 0,
                    correct_answers INTEGER NOT NULL DEFAULT 0,
                    stars INTEGER NOT NULL DEFAULT 0,
                    coins INTEGER NOT NULL DEFAULT 25,
                    updated_at_utc TEXT NOT NULL,
                    last_game TEXT NULL
                );

                PRAGMA user_version = 1;
                """;
            command.ExecuteNonQuery();
        }
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
