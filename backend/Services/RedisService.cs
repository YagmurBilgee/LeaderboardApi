using StackExchange.Redis;
using WebApplication1.Models;
using System.Collections.Generic;//<list> yapısını kullanmak için
using System.Linq;//.Select(), .Where(), .OrderBy() gibi LINQ metodları için
/*RedisService sınıfı Redis ile tüm etkileşimi yöneten servistir.
Kullanıcı skorları eklenir, silinir, sıralanır, sorgulanır.
Tüm işlemler interface’e uyar, böylece uygulamanın başka katmanları Redis’i bilmeden çalışabilir.*/
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

        public void AddScore(string username, int score)
        {
            var existingScore = _db.SortedSetScore("leaderboard", username);

            if (existingScore == null || score > existingScore)
            {
                _db.SortedSetAdd("leaderboard", username, score);//edis’te ZADD komutu gibi davranır, skor ekler veya günceller.
            }
        }

        public List<LeaderboardEntry> GetLeaderboard()
        {
            var entries = _db.SortedSetRangeByRankWithScores("leaderboard", order: Order.Descending);
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

        public string? GetUserRank(string username)
        {
            var rank = _db.SortedSetRank("leaderboard", username, Order.Descending);
            var score = _db.SortedSetScore("leaderboard", username);

            if (rank == null || score == null)
                return null;

            return $"{username} is ranked #{rank + 1} with score {score}";
        }
        public List<LeaderboardEntry> GetTopN(int count)
        {
            var entries = _db.SortedSetRangeByRankWithScores("leaderboard", 0, count - 1, Order.Descending);
            return entries
                .Select((entry, index) => new LeaderboardEntry
                {
                    Username = entry.Element!,
                    Score = entry.Score,
                    Rank = index + 1
                })
                .ToList();
        }
        public void DeleteScore(string username)
        {//SortedSetRemove(...) ile kullanıcıyı Redis’ten kaldırır.
            _db.SortedSetRemove("leaderboard", username);
        }
/*RedisService, Redis SortedSet yapısıyla çalışan bir servis sınıfıdır. 
Sıralama, silme, güncelleme gibi işlemler bu sınıf aracılığıyla yapılır.
 Uygulamada başka hiçbir sınıf doğrudan Redis’e bağlanmaz*/

    }
}
