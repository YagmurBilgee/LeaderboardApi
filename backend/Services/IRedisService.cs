using System.Collections.Generic;// List<T>, Dictionary<K,V> gibi generic koleksiyon sınıflarını sağlar.
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IRedisService//bir arayüzdür.Redis ile ilgili metotların ne yapacağını tanımlar ama nasıl yapacağını belirtmez.
    {
        void AddScore(string Username, int Score);//kullanıcı skoru ekleyecek ya da güncelleyecek
        //Redis ZADD komutunu kullanır (SortedSetAdd)
        List<LeaderboardEntry> GetLeaderboard();//tüm kullanıcıları yüksekten düşüğe sııralayıp dönecek
        //LeaderboardEntry listesini döndüren bir fonksiyon
        string? GetUserRank(string Username);//kullanıcının sırasını ve skorunu dönecek
        List<LeaderboardEntry> GetTopN(int count);//en yüksek skora sahip ilk count kişiyi getirir.
        void DeleteScore(string username);//kullanıcıyı redis sorted sette çıkarır.

/*IRedisService.cs, Redis’e yapılacak işlemlerin sözleşmesini tanımlar. Bu sayede uygulama Redis’in nasıl çalıştığını bilmeden bu arayüzle iletişim kurabilir. Gerçek işlemler RedisService içinde uygulanır.*/

    }
}
