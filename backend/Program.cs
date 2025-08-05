using WebApplication1.Services;
//IRedisService ve RedisService gibi servisleri bu dosyada kullanabilmek için çağırıyoruz.
var builder = WebApplication.CreateBuilder(args);//Logging, servis ekleme, yapılandırma gibi işlemler için builder nesnesi oluşturur.
builder.WebHost.UseUrls("http://0.0.0.0:80");

// toplantıda bu şuanda çalışmıyor olarka konuşuldu. ama ihityaç duyulacak.
builder.Services.AddCors(options =>
{//Farklı portlardaki frontend'in (React: 3000) API'ye erişebilmesini sağlar.
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IRedisService, RedisService>();
  
//app adlı web uygulaması nesnesi
var app = builder.Build();

app.UseCors("AllowFrontend");


app.MapGet("/", () => "Leaderboard API çalışıyor");// GET / isteğine gelen cevap: basit bir kontrol mesajı (health check) döner.
//Bu sayede API'nin çalışıp çalışmadığı kontrol edilebilir.

app.MapPost("/score/{course}/{username}/{score}", (HttpContext context, string course, string username, int score) =>
{//Kullanıcının skorunu Redis'e ekler.Skor daha önce varsa, sadece daha yüksekse günceller (bu RedisService'de tanımlı)
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(username)||score < 0)
        return Results.BadRequest("Geçersiz giriş.");

    redisService.AddScore(course, username, score);
    return Results.Ok($"{course} için {username} skoru {score} olarak kaydedildi.");
});

app.MapGet("/leaderboard/{course}", (HttpContext context, string course) =>//Redis’teki leaderboard listesinden verileri çeker ve sıralı şekilde döner
{//Redis'teki tüm kullanıcıları en yüksekten düşüğe sıralı şekilde döner.
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();
    var list = redisService.GetLeaderboard(course);
    return Results.Ok(list);
});

app.MapGet("/leaderboard/{course}/user/{username}", (HttpContext context,string course, string username) =>
{
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();
    var result = redisService.GetUserRank(course, username);

    if (result == null)
        return Results.NotFound($"{username}{course} adresinde bulunamadı.");

    return Results.Ok(result);
});
app.MapGet("/leaderboard/{course}/top/{n:int}", (HttpContext context,string course, int n) =>
{
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (n <= 0)
        return Results.BadRequest("Geçerli bir sayı girin.");

    var list = redisService.GetTopN(course,n);
    return Results.Ok(list);
});
app.MapGet("/courses", (HttpContext context) =>
{
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();
    var courses = redisService.GetAllCourses();
    return Results.Ok(courses);
});
app.MapDelete("/score/{course}/{username}", (HttpContext context, string course, string username) =>
{//Belirli bir kullanıcıyı Redis'ten kaldırır.Yalnızca username yeterlidir.
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(username))
        return Results.BadRequest("Eksik veri girdiniz.");

    redisService.DeleteScore(course, username);
    return Results.Ok($"{course} alanından {username} skoru silindi.");
});
app.MapDelete("/score/{course}/range/{min}/{max}", 
(HttpContext context, string course, double min, double max) =>
{
    var redisService = context.RequestServices.GetRequiredService<IRedisService>();

    if (string.IsNullOrWhiteSpace(course) || min > max)
        return Results.BadRequest("Geçersiz aralık.");

    redisService.DeleteByScoreRange(course, min, max);
    return Results.Ok($"{course} kursundaki {min}–{max} arası skorlar silindi.");
});


app.Run();//web sunucusunu başlatmak için
