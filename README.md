# LibraryManagement

ASP.NET Core MVC ile geliştirilmiş, kitap kataloglama, kullanıcı işlemleri, sepet, sipariş, admin yönetimi, REST API, raporlama ve test katmanlarını içeren kütüphane yönetim uygulaması.

---

# Genel Bakış

LibraryManagement, kitapların kategori bazlı listelenebildiği, kullanıcıların sepet ve sipariş akışı üzerinden işlem yapabildiği bir ASP.NET Core MVC projesidir. Uygulama; kullanıcı kayıt/giriş işlemleri, profil, adres yönetimi, favoriler, kitap yorumları, kupon kullanımı, sipariş oluşturma ve PDF fatura indirme gibi temel kullanıcı senaryolarını içerir.

Admin alanı; kitap, kategori, sipariş, yorum ve kupon yönetimi için ayrı bir MVC area olarak yapılandırılmıştır. Admin kullanıcılar dashboard ekranına erişebilir, sipariş durumlarını güncelleyebilir, PDF fatura indirebilir ve sipariş/stok raporlarını Excel dosyası olarak dışa aktarabilir.

Proje tek solution altında bir web projesi ve bir test projesinden oluşur. Web projesinde Controller, Service, Repository, Entity, Validator, DTO/ViewModel, Middleware ve Mapping katmanları ayrılmıştır. REST API tarafında Swagger, JWT tabanlı admin yetkilendirmesi ve DTO dönüşümleri kullanılır.

---

# Öne Çıkan Mimari Özellikler

* ASP.NET Core MVC ve Area tabanlı admin paneli
* Controller, Service ve Repository katmanları
* Repository Pattern
* Dependency Injection
* Entity Framework Core ve SQL Server
* EF Core Migration ve uygulama başlangıcında seed/migration akışı
* FluentValidation ile form ve API doğrulama
* AutoMapper ile Entity-DTO dönüşümleri
* Cookie Authentication
* JWT Bearer Authentication
* Google Authentication yapılandırması
* Rol bazlı yetkilendirme
* Swagger/OpenAPI
* Serilog ile console ve dosya loglama
* Global exception middleware
* Session tabanlı sepet yönetimi
* QuestPDF ile PDF fatura üretimi
* ClosedXML ile Excel rapor üretimi
* Docker ve Docker Compose
* GitHub Actions CI
* xUnit ve Moq ile servis testleri

---

# Teknolojiler

| Alan | Teknoloji |
| --- | --- |
| Backend | ASP.NET Core MVC, C#, .NET 8 |
| Web UI | Razor Views, MVC Controllers, ViewComponent, static assets |
| ORM | Entity Framework Core 8 |
| Veritabanı | SQL Server / LocalDB |
| Authentication | Cookie Authentication, JWT Bearer, Google Authentication |
| Authorization | Role-based Authorization, `Admin` ve `User` rolleri |
| Validation | FluentValidation |
| Logging | Serilog, Console sink, File sink |
| Mapping | AutoMapper |
| API | ASP.NET Core API Controllers, Swagger / Swashbuckle |
| Testing | xUnit, Moq, Microsoft.NET.Test.Sdk, coverlet.collector |
| Container | Docker, Docker Compose, SQL Server 2022 container |

---

# Proje Yapısı

```text
LibraryManagement.sln                 Solution dosyası

├── LibraryManagement.Web/             ASP.NET Core MVC ve API projesi
│   ├── Areas/                         Admin area yapısı, admin controller ve view dosyaları
│   ├── Controllers/                   MVC controller ve API controller sınıfları
│   ├── Data/                          BookContext, DbContext factory ve seed işlemleri
│   ├── Entity/                        Veritabanı entity sınıfları
│   ├── Extensions/                    Session için yardımcı extension metotları
│   ├── Mappings/                      AutoMapper profil tanımları
│   ├── Middleware/                    Global hata yönetimi middleware sınıfı
│   ├── Migrations/                    EF Core migration dosyaları
│   ├── Models/                        ViewModel, DTO ve ayar modelleri
│   ├── Repositories/                  Repository interface ve implementasyonları
│   ├── Services/                      İş kuralları ve servis interface/implementasyonları
│   ├── Validators/                    FluentValidation doğrulama sınıfları
│   ├── ViewComponent/                 Genre listeleme ViewComponent yapısı
│   ├── Views/                         Razor view dosyaları
│   ├── wwwroot/                       CSS, JavaScript, görsel ve client asset dosyaları
│   ├── Program.cs                     Uygulama servisleri, middleware ve route yapılandırması
│   └── appsettings.json               Uygulama yapılandırması
│
├── LibraryManagement.Tests/           Unit test projesi
│   ├── Helpers/                       Test yardımcı sınıfları
│   └── Services/                      Servis katmanı testleri
│
├── Dockerfile                         Web uygulaması için çok aşamalı Docker build dosyası
├── docker-compose.yml                 Web ve SQL Server servislerini çalıştıran compose dosyası
└── .github/workflows/ci.yml           GitHub Actions build ve test workflow dosyası
```

---

# Bağımlılık Akışı

```text
HTTP Request

↓

MVC Controller / API Controller

↓

Service

↓

Repository

↓

BookContext

↓

Entity Framework Core

↓

SQL Server
```

Admin panel, kullanıcı arayüzü ve API katmanları aynı servis ve repository bağımlılıklarını kullanır. Sepet işlemleri session üzerinde tutulur; sipariş, kullanıcı, kitap, yorum, kupon ve rapor işlemleri veritabanı üzerinden yürür.

---

# Gereksinimler

* .NET 8 SDK
* SQL Server veya SQL Server LocalDB
* Entity Framework Core CLI aracı
* Docker ve Docker Compose
* Git

EF Core CLI yüklü değilse:

```bash
dotnet tool install --global dotnet-ef
```

---

# Kurulum

1. Depoyu klonlayın.

```bash
git clone https://github.com/rabiaakdas/LibraryManagement.git
cd LibraryManagement
```

2. NuGet paketlerini geri yükleyin.

```bash
dotnet restore LibraryManagement.sln
```

3. Veritabanı bağlantısını yapılandırın.

```json
{
  "ConnectionStrings": {
    "MsSQLConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

4. Migration dosyalarını veritabanına uygulayın.

```bash
dotnet ef database update --project LibraryManagement.Web
```

5. Uygulamayı çalıştırın.

```bash
dotnet run --project LibraryManagement.Web
```

6. Uygulamayı tarayıcıda açın.

```text
http://localhost:5000
```

Uygulama başlangıcında `DataSeeding.Seed(app)` çalışır. Bu metot `context.Database.Migrate()` çağırır ve veritabanında veri yoksa kategori, kitap ve varsayılan kullanıcı kayıtlarını oluşturur.

---

# Docker ile Çalıştırma

Projede `Dockerfile` ve `docker-compose.yml` bulunur. Compose dosyası web uygulamasını ve SQL Server 2022 container'ını birlikte ayağa kaldırır.

```bash
docker compose up --build
```

| Servis | Açıklama |
| --- | --- |
| `web` | `LibraryManagement.Web` uygulamasını .NET 8 runtime üzerinde çalıştırır |
| `sqlserver` | SQL Server 2022 Developer container'ı |

Docker ile varsayılan erişim adresi:

```text
http://localhost:5000
```

Compose dosyasında örnek geliştirme değerleri bulunur. Gerçek ortamda SQL Server parolası ve JWT anahtarı environment variable veya secret yönetimiyle değiştirilmelidir.

---

# Testleri Çalıştırma

Tüm testleri solution üzerinden çalıştırmak için:

```bash
dotnet test LibraryManagement.sln --configuration Release
```

Sadece test projesini çalıştırmak için:

```bash
dotnet test LibraryManagement.Tests/LibraryManagement.Tests.csproj
```

`LibraryManagement.Tests` projesi `LibraryManagement.Web` projesine referans verir. Testler servis katmanına odaklanır ve mock bağımlılıklar için Moq kullanır.

---

# Varsayılan Kullanıcılar

Seed işlemi veritabanında kullanıcı yoksa aşağıdaki kullanıcıları oluşturur.

| Rol | Kullanıcı | E-posta | Parola |
| --- | --- | --- | --- |
| User | `usera` | `usera@gmail.com` | `12345` |
| User | `userb` | `userb@gmail.com` | `12345` |
| User | `userc` | `userc@gmail.com` | `12345` |
| Admin | `admin` | `admin@library.com` | `Admin123` |

Parolalar seed sırasında BCrypt ile hashlenerek kaydedilir.

---

# API Endpointleri

| Method | Route | Açıklama | Auth |
| --- | --- | --- | --- |
| POST | `/api/auth/login` | Kullanıcı adı/e-posta ve parola ile JWT token üretir | Yok |
| GET | `/api/books` | Kitap listesini döndürür | Yok |
| GET | `/api/books/{id}` | Tek kitap bilgisini döndürür | Yok |
| POST | `/api/books` | Yeni kitap oluşturur | JWT Bearer, Admin |
| PUT | `/api/books/{id}` | Kitap bilgisini günceller | JWT Bearer, Admin |
| DELETE | `/api/books/{id}` | Kitabı siler | JWT Bearer, Admin |
| GET | `/api/genres` | Kategori listesini döndürür | Yok |
| GET | `/api/genres/{id}` | Tek kategori bilgisini döndürür | Yok |
| GET | `/api/orders` | Sipariş listesini döndürür | Yok |
| GET | `/api/orders/{id}` | Tek sipariş bilgisini döndürür | Yok |

Swagger yalnızca development ortamında aktiftir.

```text
http://localhost:5000/swagger
```

---

# Kullanıcı Akışı

```text
Kullanıcı

↓

Kayıt olur veya giriş yapar

↓

Kitapları görüntüler ve filtreler

↓

Kitap detayını inceler

↓

Favorilere ekler veya yorum yapar

↓

Sepete kitap ekler

↓

Adres ve ödeme bilgisiyle sipariş oluşturur

↓

Siparişlerini görüntüler

↓

PDF fatura indirir
```

---

# Admin Akışı

```text
Admin

↓

Dashboard

↓

Kitap Yönetimi

↓

Kategori Yönetimi

↓

Sipariş Yönetimi

↓

Yorum Yönetimi

↓

Kupon Yönetimi

↓

Excel Raporları
```

---

# Veritabanı

| Entity | Açıklama |
| --- | --- |
| `Book` | Kitap adı, yazar, görsel, sayfa sayısı, fiyat, stok, kategori ve yorum ilişkilerini tutar |
| `Genre` | Kitap kategorilerini tutar; kitaplarla çoktan çoğa ilişkilidir |
| `User` | Kullanıcı adı, e-posta, parola, rol ve harici giriş sağlayıcı bilgilerini tutar |
| `BookReview` | Kitap yorumlarını, puanları ve kullanıcı-kitap ilişkisini tutar |
| `BookSuggestion` | Kullanıcı kitap önerilerini ve beğeni sayısını tutar |
| `Favorite` | Kullanıcı ile favori kitap ilişkisini tutar |
| `Address` | Kullanıcı adres bilgilerini tutar |
| `Order` | Sipariş tarihi, tutar, durum, adres, ödeme, kupon, kargo ve takip bilgilerini tutar |
| `OrderItem` | Sipariş içindeki kitap kalemlerini, adet ve fiyat bilgilerini tutar |
| `Coupon` | Kupon kodu, indirim tipi, indirim değeri, minimum tutar, kullanım limiti ve aktiflik bilgisini tutar |

`BookContext` içinde yapılandırılan temel ilişkiler:

```text
Book - Genre             many-to-many
Book - BookReview        one-to-many
User - BookReview        one-to-many
Order - OrderItem        one-to-many
Order - Address          optional one-to-one/one-to-many kullanım
Favorite - Book          many-to-one
```

---

# Test Kapsamı

| Test Sınıfı | Açıklama |
| --- | --- |
| `BookServiceTests` | Kitap listeleme, detay, stok ve API kitap işlemleri için servis davranışlarını test eder |
| `CouponServiceTests` | Kupon doğrulama, indirim ve kullanım kurallarını test eder |
| `EmailServiceTests` | Fake e-posta gönderim davranışını test eder |
| `ExportServiceTests` | Sipariş ve stok Excel raporu üretimini test eder |
| `GenreServiceTests` | Kategori servis işlemlerini test eder |
| `InvoiceServiceTests` | PDF fatura üretimi ve erişim kontrollerini test eder |
| `OrderServiceTests` | Sipariş oluşturma, kargo, durum ve dashboard işlemlerini test eder |
| `ReviewServiceTests` | Kitap yorum ekleme ve yorum yönetimi davranışlarını test eder |
| `UserServiceTests` | Kullanıcı giriş, API login, e-posta/kullanıcı adı kontrolü ve harici kullanıcı oluşturmayı test eder |

---

# Konfigürasyon

Gerçek secret, parola ve bağlantı bilgileri repoya yazılmamalıdır. Aşağıdaki örnek yalnızca gerekli yapılandırma alanlarını gösterir.

| Alan | Açıklama |
| --- | --- |
| `ConnectionStrings:MsSQLConnection` | SQL Server bağlantısı |
| `Jwt:Key` | JWT imzalama anahtarı |
| `Jwt:Issuer` | Token issuer bilgisi |
| `Jwt:Audience` | Token audience bilgisi |
| `Jwt:ExpireMinutes` | Token geçerlilik süresi |
| `Authentication:Google` | Google OAuth istemci bilgileri |
| `EmailSettings` | SMTP veya fake email sender ayarları |
| `Serilog` | Console ve dosya loglama ayarları |

```json
{
  "ConnectionStrings": {
    "MsSQLConnection": "YOUR_SQL_SERVER_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "YOUR_STRONG_JWT_KEY",
    "Issuer": "LibraryManagement.Web",
    "Audience": "LibraryManagement.Api",
    "ExpireMinutes": 60
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },
  "EmailSettings": {
    "Host": "YOUR_SMTP_HOST",
    "Port": 587,
    "UserName": "YOUR_SMTP_USERNAME",
    "Password": "YOUR_SMTP_PASSWORD",
    "FromEmail": "YOUR_FROM_EMAIL",
    "FromName": "LibraryManagement",
    "EnableSsl": true,
    "UseFakeEmailSender": true
  }
}
```

Development ortamında `appsettings.Development.json` yalnızca logging seviyelerini içerir. Yerel çalışma profili `LibraryManagement.Web/Properties/launchSettings.json` içinde `http://localhost:5000` olarak tanımlıdır.

---

# Kullanılan NuGet Paketleri

## LibraryManagement.Web

| Paket | Amaç |
| --- | --- |
| `BCrypt.Net-Next` | Seed ve harici kullanıcı parolaları için BCrypt hash/doğrulama desteği |
| `ClosedXML` | Excel raporu üretimi |
| `FluentValidation.AspNetCore` | MVC ve API model doğrulama |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | AutoMapper servis kaydı ve DTO mapping |
| `Microsoft.AspNetCore.Authentication.Google` | Google Authentication |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer Authentication |
| `Microsoft.EntityFrameworkCore.Design` | EF Core design-time işlemleri |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server EF Core provider |
| `Microsoft.EntityFrameworkCore.Tools` | Migration ve EF araçları |
| `Newtonsoft.Json` | Session object serialize/deserialize işlemleri |
| `QuestPDF` | PDF fatura üretimi |
| `Serilog.AspNetCore` | ASP.NET Core Serilog entegrasyonu |
| `Serilog.Sinks.Console` | Console log çıktısı |
| `Serilog.Sinks.File` | Dosya log çıktısı |
| `Swashbuckle.AspNetCore` | Swagger/OpenAPI dokümantasyonu |

## LibraryManagement.Tests

| Paket | Amaç |
| --- | --- |
| `coverlet.collector` | Test coverage toplama altyapısı |
| `Microsoft.NET.Test.Sdk` | .NET test çalıştırma altyapısı |
| `Moq` | Mock nesne oluşturma |
| `xunit` | Unit test framework |
| `xunit.runner.visualstudio` | Visual Studio ve `dotnet test` runner entegrasyonu |

---

# Loglama

Projede Serilog yapılandırılmıştır. `Program.cs` içinde `builder.Host.UseSerilog(...)` ile aktif edilir ve `app.UseSerilogRequestLogging()` middleware pipeline'a eklenir.

| Hedef | Açıklama |
| --- | --- |
| Console | Geliştirme sırasında logların terminalde izlenmesi |
| File | `LibraryManagement.Web/Logs/logs-.txt` dosya desenine günlük log yazımı |

`appsettings.json` içinde dosya logları için `rollingInterval: Day` ve `retainedFileCountLimit: 14` tanımlıdır. Email, rapor, fatura ve kullanıcı giriş işlemlerinde servisler üzerinden log kayıtları üretilir.

---

# Kimlik Doğrulama

| Yöntem | Kullanım |
| --- | --- |
| Cookie Authentication | MVC kullanıcı girişi ve admin panel erişimi |
| JWT Bearer Authentication | API üzerinden admin kitap oluşturma, güncelleme ve silme işlemleri |
| Google Authentication | Google OAuth ayarları geçerliyse harici kullanıcı girişi |
| Role-based Authorization | Admin area ve bazı API işlemlerinde `Admin` rolü kontrolü |

Cookie login yolu:

```text
/Users/Login
```

Cookie logout yolu:

```text
/Users/Logout
```

JWT token alma endpoint'i:

```text
POST /api/auth/login
```

Google Authentication yalnızca `Authentication:Google:ClientId` ve `Authentication:Google:ClientSecret` placeholder olmayan değerlerle yapılandırıldığında eklenir.

---

# Hata Yönetimi

Projede `GlobalExceptionMiddleware` kullanılır. Middleware, beklenmeyen hataları yakalar ve `ILogger` üzerinden loglar.

| İstek Tipi | Davranış |
| --- | --- |
| API isteği | JSON hata cevabı döndürür |
| JSON kabul eden istek | JSON hata cevabı döndürür |
| MVC isteği | `/Home/Error` sayfasına yönlendirir |
| Development ortamı | API hata cevabına exception mesajını `detail` alanı olarak ekler |

Middleware `Program.cs` içinde route ve authentication işlemlerinden önce pipeline'a eklenmiştir.

---

# Lisans

Bu proje MIT lisansı ile lisanslanmıştır. Ayrıntılar için `LICENSE` dosyasını inceleyebilirsiniz.
