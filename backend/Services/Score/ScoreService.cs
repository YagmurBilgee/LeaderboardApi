using StackExchange.Redis;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Services.Score
{
    public class ScoreService : IScoreService
    {
        private readonly IDatabase _db;

        public ScoreService()
        {
            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
            var redis = ConnectionMultiplexer.Connect($"{redisHost}:6379");
            _db = redis.GetDatabase();
        }

        public void AddScore(string course, string username, int score)
        {
            var key = $"leaderboard:{course}";
            var existingScore = _db.SortedSetScore(key, username);

            if (existingScore == null || score > existingScore)
            {
                _db.SortedSetAdd(key, username, score);
            }
        }

        public List<LeaderboardEntry> GetLeaderboard(string course)
        {
            var key = $"leaderboard:{course}";
            var entries = _db.SortedSetRangeByRankWithScores(key, order: Order.Descending);
            return entries
                .Select((entry, index) => new LeaderboardEntry
                {
                    Username = entry.Element!,
                    Score = entry.Score,
                    Rank = index + 1
                })
                .ToList();
        }

        public List<LeaderboardEntry> GetTopN(string course, int count)
        {
            var key = $"leaderboard:{course}";
            var entries = _db.SortedSetRangeByRankWithScores(key, 0, count - 1, Order.Descending);
            return entries
                .Select((entry, index) => new LeaderboardEntry
                {
                    Username = entry.Element!,
                    Score = entry.Score,
                    Rank = index + 1
                })
                .ToList();
        }

        public void DeleteScore(string course, string username)
        {
            var key = $"leaderboard:{course}";
            _db.SortedSetRemove(key, username);
        }
        public void DeleteByScoreRange(string course, double minScore, double maxScore)
        {
            var key = $"leaderboard:{course}";
            _db.SortedSetRemoveRangeByScore(key, minScore, maxScore);
        }

    }
}
