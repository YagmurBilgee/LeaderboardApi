using WebApplication1.Models;
using System.Collections.Generic;

namespace WebApplication1.Services.Score
{
    public interface IScoreService
    {
        void AddScore(string course, string username, int score);
        List<LeaderboardEntry> GetLeaderboard(string course);
        List<LeaderboardEntry> GetTopN(string course, int count);
        void DeleteScore(string course, string username);

        void DeleteByScoreRange(string course, double minScore, double maxScore);

    }
}
