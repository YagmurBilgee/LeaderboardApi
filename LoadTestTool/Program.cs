using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        string course = "english";

        Console.Write("Kaç test verisi üretilecek? ");
        int count = int.Parse(Console.ReadLine()!);

        var tasks = Enumerable.Range(1, count).Select(i => new TasNesnesi
        {
            Username = $"testuser{i}",
            Score = new Random().Next(0, 100)
        }).ToList();

        var errors = new List<string>();
        var stopwatch = Stopwatch.StartNew();
        
        Console.WriteLine("Skorlar gönderiliyor...");
        await Parallel.ForEachAsync(tasks, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async (item, _) =>
        {
            try
            {
                var url = $"http://localhost:5259/score/{course}/{item.Username}/{item.Score}";
                var response = await httpClient.PostAsync(url, null);
                if (!response.IsSuccessStatusCode)
                {
                    errors.Add($"Hata: {item.Username} → {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Exception: {item.Username} → {ex.Message}");
            }
        });

        stopwatch.Stop();
        double avgTimeMs = (stopwatch.Elapsed.TotalMilliseconds / count);

        // Sonuçları göster
        Console.WriteLine($"\nTamamlandı: {count} istek gönderildi.");
        Console.WriteLine($"Geçen süre: {stopwatch.Elapsed.TotalSeconds:F2} saniye");
        Console.WriteLine($"Ortalama süre: {avgTimeMs:F2} ms");
        Console.WriteLine($"Hatalı istek sayısı: {errors.Count}");
        foreach (var error in errors)
            Console.WriteLine(error);

        // Sonucu txt dosyasına yaz
        var report = new StringBuilder();
        report.AppendLine($"– {count} Kullanıcı");
        report.AppendLine($"Süre: {stopwatch.Elapsed.TotalSeconds:F2} saniye");
        report.AppendLine($"Ortalama: {avgTimeMs:F2} ms");
        report.AppendLine($"Hata: {errors.Count}");
        if (errors.Count > 0)
        {
            report.AppendLine("Hata Detayları:");
            foreach (var error in errors)
                report.AppendLine(error);
        }

        string filePath = "loadtest-results.txt";
        File.AppendAllText(filePath, report.ToString() + "\n---\n");
        Console.WriteLine($"\nSonuçlar '{filePath}' dosyasına kaydedildi.");
        Console.Write("Belirli skor aralığını silmek ister misin? (y/n): ");
        var input = Console.ReadLine();
        if (input?.Trim().ToLower() == "y")
    {
        Console.Write("Minimum skor: ");
        double min = double.Parse(Console.ReadLine()!);
        Console.Write("Maksimum skor: ");
        double max = double.Parse(Console.ReadLine()!);

        var rangeUrl = $"http://localhost:5259/score/{course}/range/{min}/{max}";
        var response = await httpClient.DeleteAsync(rangeUrl);
        Console.WriteLine($"Temizlendi: {response.StatusCode}");
    }
    }
        

   class TasNesnesi
    {
        public string Username { get; set; }
        public int Score { get; set; }
    }
}
