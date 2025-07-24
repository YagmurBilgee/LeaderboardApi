using WebApplication1.Services;
//IRedisService ve RedisService gibi servisleri bu dosyada kullanabilmek için çağırıyoruz.
var builder = WebApplication.CreateBuilder(args);//Logging, servis ekleme, yapılandırma gibi işlemler için builder nesnesi oluşturur.
builder.WebHost.UseUrls("http://0.0.0.0:80");

// CORS ekle
builder.Services.AddCors(options =>
{//Farklı portlardaki frontend'in (React: 3000) API'ye erişebilmesini sağlar.
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IRedisService, RedisService>();
  
//app adlı web uygulaması nesnesi
var app = builder.Build();

app.UseCors("AllowFrontend"); // CORS middleware'i uygula



// Diğer endpoint'ler...


app.MapGet("/", () => "Leaderboard API çalışıyor");// GET / isteğine gelen cevap: basit bir kontrol mesajı (health check) döner.
//Bu sayede API'nin çalışıp çalışmadığı kontrol edilebilir.

app.MapPost("/score/{username}/{score}", (HttpContext context, string username, int score) =>
{//Kullanıcının skorunu Redis'e ekler.Skor daha önce varsa, sadece daha yüksekse günceller (bu RedisService'de tanımlı)
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (string.IsNullOrWhiteSpace(username) || score < 0)
        return Results.BadRequest("Geçersiz kullanıcı adı veya skor.");

    redisService.AddScore(username, score);
    return Results.Ok($"{username} skoru {score} olarak kaydedildi.");
});

app.MapGet("/leaderboard", (HttpContext context) =>//Redis’teki leaderboard listesinden verileri çeker ve sıralı şekilde döner
{//Redis'teki tüm kullanıcıları en yüksekten düşüğe sıralı şekilde döner.
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();
    var list = redisService.GetLeaderboard();
    return Results.Ok(list);
});

app.MapGet("/leaderboard/user/{username}", (HttpContext context, string username) =>
{
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();
    var result = redisService.GetUserRank(username);

    if (result == null)
        return Results.NotFound($"{username} bulunamadı.");

    return Results.Ok(result);
});
app.MapGet("/leaderboard/top/{n:int}", (HttpContext context, int n) =>
{
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (n <= 0)
        return Results.BadRequest("Geçerli bir sayı girin.");

    var list = redisService.GetTopN(n);
    return Results.Ok(list);
});
app.MapDelete("/score/{username}", (HttpContext context, string username) =>
{//Belirli bir kullanıcıyı Redis'ten kaldırır.Yalnızca username yeterlidir.
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (string.IsNullOrWhiteSpace(username))
        return Results.BadRequest("Kullanıcı adı boş olamaz.");

    redisService.DeleteScore(username);
    return Results.Ok($"{username} skoru silindi.");
});

app.Run();//web sunucusunu başlatmak için
