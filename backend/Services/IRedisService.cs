using System.Collections.Generic;// List<T>
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IRedisService//bir arayüzdür.Redis ile ilgili metotların ne yapacağını tanımlar ama nasıl yapacağını belirtmez.
    {
        void AddScore(string course, string Username, int Score);//kullanıcı skoru ekleyecek ya da güncelleyecek
        //Redis ZADD komutunu kullanır (SortedSetAdd)
        List<LeaderboardEntry> GetLeaderboard(string course);//tüm kullanıcıları yüksekten düşüğe sııralayıp dönecek
        //LeaderboardEntry listesini döndüren bir fonksiyon
        string? GetUserRank(string course, string Username);//kullanıcının sırasını ve skorunu dönecek
        List<LeaderboardEntry> GetTopN(string course, int count);//en yüksek skora sahip ilk count kişiyi getirir.
        void DeleteScore(string course, string username);//kullanıcıyı redis sorted sette çıkarır.
        List<string> GetAllCourses();
/*IRedisService.cs, Redis’e yapılacak işlemlerin sözleşmesini tanımlar. Bu sayede uygulama Redis’in nasıl çalıştığını bilmeden bu arayüzle iletişim kurabilir. Gerçek işlemler RedisService içinde uygulanır.*/
        void DeleteByScoreRange(string course, double minScore, double maxScore);

    }
} 
