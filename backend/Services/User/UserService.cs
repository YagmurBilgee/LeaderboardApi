using StackExchange.Redis;

namespace WebApplication1.Services.User
{
    public class UserService : IUserService
    {
        private readonly IDatabase _db;

        public UserService()
        {
            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
            var redis = ConnectionMultiplexer.Connect($"{redisHost}:6379");
            _db = redis.GetDatabase();
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
    }
}
