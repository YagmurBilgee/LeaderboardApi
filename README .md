# 🏆 Redis Tabanlı Liderlik Tablosu (Leaderboard) API

Bu proje, Redis veritabanı kullanarak yüksek skorlara sahip kullanıcıların sıralandığı bir **liderlik tablosu (leaderboard)** API'si ve frontend arayüzüdür. ASP.NET Core ile backend, React ile frontend geliştirilmiştir. Veriler Redis'in `SortedSet` veri yapısı kullanılarak saklanır ve sorgulanır.

---

## 🚀 Özellikler

- Skor ekleme veya güncelleme (sadece daha yüksekse)
- Liderlik tablosunu listeleme
- Belirli bir kullanıcının sırasını ve skorunu görüntüleme
- En iyi N kişiyi çekme (`/top/{n}`)
- Kullanıcı skorunu silme (`DELETE`)
- React frontend ile görsel kullanıcı arayüzü
- Docker ile Redis kurulumu

---

## 🧱 Teknolojiler

| Katman | Teknoloji |
|--------|-----------|
| Backend | ASP.NET Core Web API |
| Veri | Redis (Sorted Sets) |
| Frontend | React.js |
| Diğer | Docker, Postman |

---

## ⚙️ Kurulum

### 1. Projeyi klonlayın

```bash
git clone https://github.com/kullanici-adi/LeaderboardApi.git
cd LeaderboardApi
```

### 2. Redis'i Docker ile başlatın

```bash
docker-compose up -d
```

> Bu işlem Redis sunucusunu 6379 portundan başlatır.

### 3. Backend’i çalıştırın

```bash
cd backend
dotnet run
```

API artık `http://localhost:5259` üzerinden hizmet veriyor olacak.

### 4. Frontend’i başlatın

```bash
cd frontend
npm install
npm start
```

---

## 📬 API Endpoint’leri

### ▶️ Skor Ekle (veya Güncelle)
```
POST /score/{username}/{score}
```

### 📋 Tüm Kullanıcıları Listele
```
GET /leaderboard
```

### 🔝 En İyi N Kullanıcı
```
GET /leaderboard/top/{n}
```

### 🔍 Belirli Kullanıcının Sırası ve Skoru
```
GET /leaderboard/user/{username}
```

### ❌ Skor Sil
```
DELETE /score/{username}
```

---

## 🖥️ Frontend Görsel Özellikler

- Arama kutusu ile kullanıcı filtreleme
- Skor gönderme formu (kullanıcı adı + skor)
- Zebra desenli tablo görünümü
- LC Waikiki renk paletine uygun tema (`#0074c8` mavi tonu)

---

## 🧪 Geliştirme ve Test

- API testleri için [Postman](https://www.postman.com/) kullanıldı
- Frontend React bileşenleri düzenli olarak test edildi
- Redis bağlantısı canlı olarak Docker üzerinden kontrol edildi (`redis-cli ping → PONG`)

---

## 📌 Notlar

- Bu proje yalnızca demo amaçlıdır. Gerçek uygulamalarda kimlik doğrulama ve yetkilendirme mekanizmaları gereklidir.
- Veri kaybı yaşamamak için Redis volume kullanılmıştır (Docker'da `redis-data` volume tanımı).

---

## 👤 Geliştirici

> **Adın Soyadın**  
> [LinkedIn](#) • [GitHub](#)