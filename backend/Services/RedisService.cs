using StackExchange.Redis;
using WebApplication1.Models;
using System.Collections.Generic;//<list> yapısını kullanmak için
using System.Linq;//.Select(), .Where(), .OrderBy() gibi LINQ metodları için
/*Redis'te sadece tek bir Sorted Set vardı. Tüm kullanıcılar burada toplanıyordu(leaderboard). şimdi her dersin skoru ayrı tutulacak. yani key artık dinamik hale geldi.  */
namespace WebApplication1.Services
{
    public class RedisService : IRedisService//RedisService sınıfı, IRedisService arayüzünü uygular.
    {
        private readonly IDatabase _db;

        public RedisService()//redisle bağlantıyı yönetmek için
        {
            //var redis = ConnectionMultiplexer.Connect("localhost:6379");
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
                _db.SortedSetAdd(key, username, score);//edis’te ZADD komutu gibi davranır, skor ekler veya günceller.
            }
        }

        public List<LeaderboardEntry> GetLeaderboard(string course)
        {
            var key = $"leaderboard:{course}";
            var entries = _db.SortedSetRangeByRankWithScores(key, order: Order.Descending);
            //Redis’ten sıralı skor listesini çeker.
            return entries
                .Select((entry, index) => new LeaderboardEntry//Sonuç bir LeaderboardEntry listesine dönüştürülür.
                {
                    Username = entry.Element!,
                    Score = entry.Score,
                    Rank = index + 1
                })
                .ToList();
        }

        public string? GetUserRank(string course, string username)
        {
            var key = $"leaderboard:{course}";
            var rank = _db.SortedSetRank(key, username, Order.Descending);
            var score = _db.SortedSetScore(key, username);

            if (rank == null || score == null)
                return null;

            return $"{username} is ranked #{rank + 1} with score {score}";
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

        public List<string> GetAllCourses()
        {//Redis’teki leaderboard:* patternine uyan tüm key’leri çeker, sonra sadece ders adlarını alır.

            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
            var server = ConnectionMultiplexer.Connect($"{redisHost}:6379").GetServer(redisHost, 6379);
            var keys = server.Keys(pattern: "leaderboard:*")
                .Select(k => k.ToString().Replace("leaderboard:", ""))
                .ToList();

            return keys;
        }
        public void DeleteByScoreRange(string course, double minScore, double maxScore)
        {
            var key = $"leaderboard:{course}";
            _db.SortedSetRemoveRangeByScore(key, minScore, maxScore);
        }

    }
} 