//using WebApplication1.Services;
using WebApplication1.Services.Score;
using WebApplication1.Services.User;
using WebApplication1.Services.Course;
//IRedisService ve RedisService gibi servisleri bu dosyada kullanabilmek için çağırıyoruz.
var builder = WebApplication.CreateBuilder(args);//Logging, servis ekleme, yapılandırma gibi işlemler için builder nesnesi oluşturur.
builder.WebHost.UseUrls("http://0.0.0.0:80");
builder.Services.AddScoped<IScoreService, ScoreService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
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

//builder.Services.AddSingleton<IRedisService, RedisService>();
  
//app adlı web uygulaması nesnesi
var app = builder.Build();

app.UseCors("AllowFrontend");


app.MapGet("/", () => "Leaderboard API çalışıyor");// GET / isteğine gelen cevap: basit bir kontrol mesajı (health check) döner.
//Bu sayede API'nin çalışıp çalışmadığı kontrol edilebilir.

app.MapPost("/score/{course}/{username}/{score}", (HttpContext context, string course, string username, int score) =>
{//Kullanıcının skorunu Redis'e ekler.Skor daha önce varsa, sadece daha yüksekse günceller (bu RedisService'de tanımlı)
    var scoreService = context.RequestServices.GetRequiredService<IScoreService>();

    if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(username)||score < 0)
        return Results.BadRequest("Geçersiz giriş.");

    scoreService.AddScore(course, username, score);
    return Results.Ok($"{course} için {username} skoru {score} olarak kaydedildi.");
});

app.MapGet("/leaderboard/{course}", (HttpContext context, string course) =>//Redis’teki leaderboard listesinden verileri çeker ve sıralı şekilde döner
{//Redis'teki tüm kullanıcıları en yüksekten düşüğe sıralı şekilde döner.
    var scoreService = context.RequestServices.GetRequiredService<IScoreService>();
    var list = scoreService.GetLeaderboard(course);
    return Results.Ok(list);
});

app.MapGet("/leaderboard/{course}/user/{username}", (HttpContext context,string course, string username) =>
{
    var userService = context.RequestServices.GetRequiredService<IUserService>();
    var result = userService.GetUserRank(course, username);

    if (result == null)
        return Results.NotFound($"{username}{course} adresinde bulunamadı.");

    return Results.Ok(result);
});
app.MapGet("/leaderboard/{course}/top/{n:int}", (HttpContext context,string course, int n) =>
{
    var scoreService = context.RequestServices.GetRequiredService<IScoreService>();

    if (n <= 0)
        return Results.BadRequest("Geçerli bir sayı girin.");

    var list = scoreService.GetTopN(course,n);
    return Results.Ok(list);
});
app.MapGet("/courses", (HttpContext context) =>
{
    var courseService = context.RequestServices.GetRequiredService<ICourseService>();
    var courses = courseService.GetAllCourses();
    return Results.Ok(courses);
});
app.MapDelete("/score/{course}/{username}", (HttpContext context, string course, string username) =>
{//Belirli bir kullanıcıyı Redis'ten kaldırır.Yalnızca username yeterlidir.
    var scoreService = context.RequestServices.GetRequiredService<IScoreService>();

    if (string.IsNullOrWhiteSpace(course) || string.IsNullOrWhiteSpace(username))
        return Results.BadRequest("Eksik veri girdiniz.");

    scoreService.DeleteScore(course, username);
    return Results.Ok($"{course} alanından {username} skoru silindi.");
});
app.MapDelete("/score/{course}/range/{min}/{max}", 
(HttpContext context, string course, double min, double max) =>
{
    var scoreService = context.RequestServices.GetRequiredService<IScoreService>();

    if (string.IsNullOrWhiteSpace(course) || min > max)
        return Results.BadRequest("Geçersiz aralık.");

    scoreService.DeleteByScoreRange(course, min, max);
    return Results.Ok($"{course} kursundaki {min}–{max} arası skorlar silindi.");
});



app.Run();//web sunucusunu başlatmak için
