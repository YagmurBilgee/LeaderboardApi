using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication1.Services.Course
{
    public class CourseService : ICourseService
    {
        public List<string> GetAllCourses()
        {
            var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
            var server = ConnectionMultiplexer.Connect($"{redisHost}:6379").GetServer(redisHost, 6379);
            var keys = server.Keys(pattern: "leaderboard:*")
                .Select(k => k.ToString().Replace("leaderboard:", ""))
                .ToList();

            return keys;
        }
    }
}
