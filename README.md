# LibraryManagement

![LibraryManagement CI](https://github.com/USERNAME/REPOSITORY/actions/workflows/ci.yml/badge.svg)

ASP.NET Core MVC ile geliştirilmiş bu proje, kitap satış ve kütüphane yönetimi senaryosunu modelleyen katmanlı bir web uygulamasıdır. Kullanıcı tarafında kitap listeleme, filtreleme, sepet, sipariş, favori, yorum, kupon ve PDF fatura akışları; admin tarafında kitap, kategori, sipariş, kupon, yorum ve rapor yönetimi; ayrıca REST API ve Swagger desteği içerir.

Proje, junior .NET geliştirici portföyü için MVC, Entity Framework Core, katmanlı mimari, temel REST API, kimlik doğrulama, yetkilendirme ve birim test konularını gösterecek şekilde hazırlanmıştır.

## Öne Çıkan Özellikler

- Kullanıcı kayıt, giriş, çıkış ve şifre değiştirme
- Google giriş altyapısı; placeholder ayarlarda sağlayıcı devre dışı kalır
- `Admin` ve `User` rolleriyle rol tabanlı yetkilendirme
- Kitap listeleme, detay sayfası ve gelişmiş filtreleme
- Kitap adı ve yazar üzerinden arama
- Kategori, fiyat, stok ve sıralama filtreleri
- Sayfalama desteği
- Sepete ekleme, kupon uygulama ve checkout akışı
- Kullanıcı sipariş geçmişi
- PDF fatura indirme
- Favori kitaplar
- Kitap yorumları ve 1-5 yıldız puanlama
- Aynı kullanıcının aynı kitaba yalnızca bir kez yorum yapabilmesi
- Admin dashboard
- Admin kitap CRUD işlemleri
- Admin kategori CRUD işlemleri
- Admin sipariş listeleme, detay görüntüleme, durum ve kargo takip güncelleme
- Admin kupon yönetimi
- Admin yorum listeleme, filtreleme, detay görüntüleme ve silme
- Excel sipariş ve stok raporları
- REST API uç noktaları
- Swagger UI
- JWT tabanlı API kimlik doğrulama
- FluentValidation ile form ve API doğrulamaları
- Serilog ile loglama ve global hata yönetimi
- Docker Compose desteği
- GitHub Actions CI
- xUnit ve Moq ile servis katmanı birim testleri

## Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core MVC
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / LocalDB
- Cookie Authentication
- Google Authentication
- JWT Bearer Authentication
- Role Based Authorization
- Bootstrap
- Swagger / Swashbuckle
- FluentValidation
- AutoMapper
- Serilog
- QuestPDF
- ClosedXML
- xUnit
- Moq
- BCrypt.Net-Next

## Katmanlı Mimari

Projede controller sınıfları doğrudan `BookContext` kullanmaz. Uygulama akışı aşağıdaki yapı üzerinden ilerler:

```text
Controller
  -> Service
    -> Repository
      -> BookContext
```

- `Controllers`: MVC ve API istek/yanıt akışını yönetir.
- `Areas/Admin`: Admin panel controller ve view yapısını içerir.
- `Services`: İş kurallarını ve uygulama mantığını içerir.
- `Repositories`: EF Core sorgularını ve veri erişimini içerir.
- `Data/BookContext.cs`: EF Core DbContext yapısıdır.
- `Entity`: Veritabanı entity sınıflarını içerir.
- `Models` ve `Areas/Admin/Models`: ViewModel ve DTO yapılarını içerir.
- `LibraryManagement.Tests`: Servis katmanı birim testlerini içerir.

## Veritabanı Yapısı

Temel tablolar:

- `Books`: Kitap bilgileri
- `Genres`: Kategori bilgileri
- `Users`: Kullanıcı bilgileri ve rol alanı
- `Orders`: Sipariş genel bilgileri ve durum bilgisi
- `OrderItems`: Sipariş ürünleri
- `Favorites`: Kullanıcı favorileri
- `Addresses`: Kullanıcı adresleri
- `BookReviews`: Kitap yorumları ve puanları
- `BookSuggestions`: Kitap önerileri

İlişkiler:

- `Book` - `Genre`: Many-to-many
- `Order` - `OrderItem`: One-to-many
- `Book` - `BookReview`: One-to-many
- `User` - `BookReview`: One-to-many
- `User` - `Favorite`: One-to-many
- `User` - `Address`: One-to-many

## Kurulum Adımları

1. Repository'yi klonlayın.

```bash
git clone <repository-url>
cd LibraryManagement
```

2. Paketleri restore edin.

```bash
dotnet restore
```

3. `LibraryManagement.Web/appsettings.json` içindeki connection string değerini kendi ortamınıza göre düzenleyin.

Örnek LocalDB connection string:

```json
"ConnectionStrings": {
  "MsSQLConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Örnek SQL Server connection string:

```json
"ConnectionStrings": {
  "MsSQLConnection": "Server=localhost;Database=LibraryManagementDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

4. Veritabanını oluşturun.

```bash
dotnet ef database update --project LibraryManagement.Web
```

5. Uygulamayı çalıştırın.

```bash
dotnet run --project LibraryManagement.Web
```

## Migration Komutları

Yeni migration oluşturmak için:

```bash
dotnet ef migrations add MigrationName --project LibraryManagement.Web
```

Veritabanını güncellemek için:

```bash
dotnet ef database update --project LibraryManagement.Web
```

Son migration'ı geri almak için:

```bash
dotnet ef migrations remove --project LibraryManagement.Web
```

## Admin Giriş Bilgileri

Seed data çalıştığında admin kullanıcısı otomatik olarak oluşturulur.

```text
Username: admin
Password: Admin123
Email: admin@library.com
Role: Admin
```

Admin panel route'u:

```text
/Admin/Dashboard
```

> Not: Canlı ortamda seed admin şifresi mutlaka değiştirilmelidir.

## Swagger Kullanımı

Development ortamında Swagger UI aktiftir.

```text
/swagger
```

API GET uç noktaları herkese açıktır. Kitap ekleme, güncelleme ve silme uç noktaları yalnızca `Admin` rolüne açıktır.

## Sürekli Entegrasyon

GitHub Actions ile her `push` ve `pull_request` işleminde otomatik CI pipeline çalışır.

Pipeline adımları:

- Repository checkout
- .NET 8 SDK kurulumu
- `dotnet restore LibraryManagement.sln`
- `dotnet build LibraryManagement.sln --configuration Release --no-restore`
- `dotnet test LibraryManagement.sln --configuration Release --no-build --no-restore`

Amaç, her değişiklikte projenin derlenebilir ve test edilebilir durumda kaldığını otomatik olarak doğrulamaktır.

## Gelişmiş Checkout Süreci

Checkout akışı daha gerçekçi bir e-ticaret deneyimi için üç bölüme ayrılmıştır:

- Teslimat adresi
- Ödeme yöntemi
- Sipariş özeti

Kullanıcı sipariş vermeden önce kayıtlı adreslerinden birini seçmelidir. Adres yoksa checkout ekranında adres ekleme bağlantısı gösterilir.

Desteklenen ödeme yöntemleri:

- Kapıda ödeme
- Kredi kartı simülasyonu
- Havale/EFT

Kredi kartı simülasyonunda kart sahibi, kart numarası, son kullanma tarihi ve CVV alanları doğrulanır. Gerçek ödeme entegrasyonu yapılmaz ve kart bilgileri veritabanına kaydedilmez.

Kargo hesaplama kuralı:

- Sepet toplamı 500 TL altındaysa kargo 49.90 TL
- 500 TL ve üzeri siparişlerde kargo ücretsiz

Sipariş oluşturulurken ürün fiyatları veritabanından tekrar hesaplanır, stok yeniden kontrol edilir ve siparişe ara toplam, kargo ücreti, genel toplam, teslimat adresi ve ödeme yöntemi yazılır.

## Kupon ve İndirim Sistemi

Checkout sırasında kullanıcı kupon kodu girebilir. Geçerli kupon uygulandığında sipariş toplamından indirim düşülür.

Kupon kuralları:

- Kupon aktif olmalıdır.
- Son kullanma tarihi dolmamış olmalıdır.
- Minimum sipariş tutarı sağlanmalıdır.
- Kullanım limiti dolmamış olmalıdır.
- `Percentage` kuponlar yüzde indirimi uygular.
- `FixedAmount` kuponlar sabit tutar indirimi uygular.
- İndirim tutarı ara toplamı geçemez.

Sipariş oluşturulurken kupon tekrar doğrulanır, `CouponCode` ve `DiscountAmount` siparişe yazılır ve kuponun `UsedCount` değeri artırılır. Admin panelde `Kupon Yönetimi` ekranından kuponlar listelenebilir, oluşturulabilir, düzenlenebilir ve pasifleştirilebilir.

## Kargo Takip Sistemi

Admin sipariş yönetiminde siparişe kargo şirketi ve takip numarası girilebilir. Sipariş durumu `Shipped` yapıldığında `ShippedAt`, `Delivered` yapıldığında `DeliveredAt` otomatik olarak atanır.

Kullanıcı sipariş geçmişinde şu kargo bilgilerini görebilir:

- Kargo şirketi
- Takip numarası
- Kargoya verildi tarihi
- Teslim edildi tarihi

Sipariş geçmişinde basit takip adımları gösterilir:

- Sipariş alındı
- Hazırlanıyor
- Kargoya verildi
- Teslim edildi

Aktif adım sipariş durumuna göre renklendirilir. Kargo bilgisi güncellendiğinde, sipariş kargoya verildiğinde ve teslim edildiğinde Serilog ile bilgi logu yazılır.

## E-posta Bildirim Sistemi

Sipariş oluşturulduğunda ve admin sipariş durumunu değiştirdiğinde kullanıcıya e-posta bildirimi gönderilecek şekilde altyapı eklenmiştir.

Development ortamında varsayılan olarak fake email sender kullanılır:

```json
"EmailSettings": {
  "UseFakeEmailSender": true
}
```

Bu modda gerçek e-posta gönderilmez; gönderilecek içerik Serilog/ILogger üzerinden loglanır. SMTP ile gerçek gönderim yapılmak istenirse `UseFakeEmailSender` değeri `false` yapılır ve SMTP ayarları environment variable veya user secrets üzerinden verilmelidir.

Örnek environment variable adları:

```text
EmailSettings__Host
EmailSettings__Port
EmailSettings__UserName
EmailSettings__Password
EmailSettings__FromEmail
EmailSettings__FromName
EmailSettings__EnableSsl
EmailSettings__UseFakeEmailSender
```

SMTP kullanıcı adı, parola ve gerçek sunucu bilgileri GitHub'a eklenmemelidir. `appsettings.json` içinde yalnızca örnek değerler tutulur.

## Excel Raporlama

Admin kullanıcılar sipariş ve stok bilgilerini Excel formatında dışa aktarabilir.

Uç noktalar:

- `GET /Admin/Reports/OrdersExcel`
- `GET /Admin/Reports/StockExcel`

Rapor dosya adları tarih bazlı oluşturulur:

- `orders-report-yyyyMMdd.xlsx`
- `stock-report-yyyyMMdd.xlsx`

Excel üretimi için `ClosedXML` kullanılır. Başlık satırları kalın biçimlendirilir, kolon genişlikleri otomatik ayarlanır ve para alanları TL formatına uygun yazılır.

Rapor indirme ve rapor oluşturma hataları Serilog ile loglanır.

## Stok Uyarı Sistemi

Admin panelde stok seviyesi düşük kitaplar daha görünür hale getirilmiştir. Stok kuralları:

- `InStock`: Stok 5'in üzerinde
- `LowStock`: Stok 1-5 arasında
- `OutOfStock`: Stok 0 veya daha az

Admin Dashboard düşük stok bölümünde `Düşük Stok` ve `Stok Yok` badge'leri gösterilir. Admin kitap listesinde stok durumu badge olarak görünür ve stok filtresi ile `Tümü`, `Stokta olanlar`, `Düşük stok`, `Stokta olmayanlar` seçenekleri kullanılabilir.

Kullanıcı tarafında stokta olmayan kitaplarda `Sepete Ekle` butonu pasif hale gelir ve `Stokta Yok` mesajı gösterilir. Checkout sırasında stok tekrar kontrol edilir; stok yetersizse hangi kitapta sorun olduğu açık mesajla kullanıcıya bildirilir.

Sipariş sonrası kitap stoku 0'a düşerse veya düşük stok seviyesine inerse Serilog ile log yazılır.

## Google Giriş

Web arayüzünde kullanıcılar Google hesabı ile giriş yapabilecek şekilde harici giriş altyapısı eklenmiştir. Mevcut kullanıcı adı/şifre ile giriş, MVC cookie authentication, JWT API login ve admin panel akışı korunur.

Google giriş kullanmak için Google Cloud Console üzerinden OAuth Client oluşturulmalıdır:

1. Google Cloud Console'da proje oluşturun veya mevcut projeyi seçin.
2. OAuth consent screen ayarlarını tamamlayın.
3. Credentials bölümünden `OAuth client ID` oluşturun.
4. Application type olarak `Web application` seçin.
5. Authorized redirect URI alanına şu adresi ekleyin:

```text
http://localhost:5000/signin-google
```

Gerçek `ClientId` ve `ClientSecret` değerleri `appsettings.json` içine yazılmamalıdır. Geliştirme ortamında `appsettings.Development.json`, user secrets veya environment variable kullanılabilir.

Örnek environment variable adları:

```text
Authentication__Google__ClientId=your-google-client-id
Authentication__Google__ClientSecret=your-google-client-secret
```

`appsettings.json` içinde yalnızca placeholder değerler bulunur. Bu değerler değiştirilmediyse Google giriş sağlayıcısı aktif edilmez ve login ekranında Google butonu gösterilmez.

## PDF Fatura Sistemi

Kullanıcılar kendi sipariş detayları için PDF fatura indirebilir. Admin kullanıcılar da admin sipariş detay ekranından aynı faturayı indirebilir.

PDF üretimi için `QuestPDF` paketi kullanılır. Bu projede portföy/geliştirme amacıyla Community lisans ayarı yapılmıştır. Oluşturulan belge resmi e-fatura entegrasyonu değildir; proje içi örnek fatura çıktısıdır.

Fatura içeriğinde şunlar yer alır:

- Sipariş numarası ve tarihi
- Kullanıcı bilgisi
- Teslimat adresi
- Ödeme yöntemi ve sipariş durumu
- Varsa kargo şirketi ve takip numarası
- Ürün listesi, adet, birim fiyat ve toplam fiyat
- Ara toplam, kargo ücreti, kupon kodu, indirim ve genel toplam

Uç noktalar:

- Kullanıcı: `GET /Cart/Invoice/{orderId}`
- Admin: `GET /Admin/Orders/Invoice/{orderId}`

Kullanıcı sadece kendi siparişinin faturasını indirebilir. Yetkisiz fatura erişim denemeleri ve fatura oluşturma hataları Serilog ile loglanır.

## Docker ile Çalıştırma

Proje Docker Compose ile web uygulaması ve SQL Server container'ı birlikte çalışacak şekilde hazırlanmıştır.

```bash
docker compose up --build
```

Çalışma adresleri:

```text
Web:     http://localhost:5000
Swagger: http://localhost:5000/swagger
```

Compose servisleri:

- `web`: ASP.NET Core MVC/API uygulaması
- `sqlserver`: SQL Server 2022 container

Docker ortamında connection string environment variable olarak verilir:

```text
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=LibraryManagementDb;User Id=sa;Password=YourStrong!Passw0rd2026;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Geriye uyumluluk için compose dosyasında `ConnectionStrings__MsSQLConnection` da aynı değere ayarlanmıştır.

Migration konusu:

- Uygulama başlarken `DataSeeding.Seed(app)` içinde `Database.Migrate()` çalışır.
- Bu nedenle container ilk açılışta migration'ları SQL Server container'a uygular.
- SQL Server ilk açılışta hazır olana kadar web container yeniden başlayabilir; `restart: on-failure` bunun için eklenmiştir.

Dikkat edilmesi gerekenler:

- `SA_PASSWORD` örnek geliştirme parolasıdır, gerçek ortamda değiştirilmelidir.
- `Jwt__Key` örnek geliştirme secret'ıdır, gerçek ortamda güvenli secret kullanılmalıdır.
- Serilog dosya logları container içinde `/app/Logs` altına yazılır ve host tarafında `LibraryManagement.Web/Logs` klasörüne map edilir.
- `Logs/` klasörü GitHub'a eklenmez.

## Test Komutları

Tüm solution'ı build etmek için:

```bash
dotnet build
```

Testleri çalıştırmak için:

```bash
dotnet test
```

Sadece test projesini çalıştırmak için:

```bash
dotnet test LibraryManagement.Tests/LibraryManagement.Tests.csproj
```

## API Uç Noktaları

Books API:

```text
GET    /api/books
GET    /api/books/{id}
POST   /api/books        Admin
PUT    /api/books/{id}   Admin
DELETE /api/books/{id}   Admin
```

Genres API:

```text
GET /api/genres
GET /api/genres/{id}
```

Orders API:

```text
GET /api/orders
GET /api/orders/{id}
```

## JWT API Kimlik Doğrulama

MVC tarafındaki cookie tabanlı giriş sistemi korunur. REST API tarafında token almak için ayrı uç nokta kullanılır.

### API Girişi

```http
POST /api/auth/login
Content-Type: application/json
```

Örnek istek:

```json
{
  "userNameOrEmail": "admin@library.com",
  "password": "Admin123"
}
```

Başarılı yanıtta `token`, `expiration`, `userName`, `email` ve `role` alanları döner.

### Swagger Yetkilendirme

1. Uygulamayı çalıştırın ve Swagger UI sayfasını açın.
2. `POST /api/auth/login` uç noktası ile token alın.
3. Swagger'daki `Authorize` butonuna tıklayın.
4. Token değerini `Bearer {token}` formatında girin.
5. Admin rolü isteyen `POST`, `PUT` ve `DELETE /api/books` uç noktalarını test edin.

> Not: `appsettings.json` içindeki `Jwt:Key` örnek geliştirme değeridir. Gerçek ortamda güvenli bir secret ile değiştirilmelidir.

## FluentValidation

Projede MVC formları ve REST API istekleri için FluentValidation kullanılır. Doğrulama kuralları `LibraryManagement.Web/Validators` klasöründe tutulur ve `Program.cs` içinde otomatik validation olarak kaydedilir.

Oluşturulan temel validatorlar:

- `BookValidator`
- `GenreValidator`
- `OrderValidator`
- `ReviewValidator`
- `UserLoginValidator`
- `UserRegisterValidator`

MVC tarafında validation mesajları mevcut `asp-validation-for` alanlarında görünmeye devam eder. API tarafında model doğrulama başarısız olursa `[ApiController]` davranışıyla otomatik `400 BadRequest` döner.

## Serilog Loglama

Projede uygulama hatalarını ve önemli iş olaylarını merkezi olarak takip etmek için Serilog kullanılır.

- Console logları geliştirme sırasında anlık takip için kullanılır.
- File sink ile loglar `LibraryManagement.Web/Logs/` klasörüne günlük dosyalar halinde yazılır.
- Dosya formatı `logs-.txt` rolling file yapısındadır.
- Varsayılan minimum seviye `Information`, `Microsoft` ve `System` logları `Warning` seviyesindedir.
- GlobalExceptionMiddleware içindeki mevcut `ILogger` kullanımı Serilog pipeline'ı üzerinden çalışır.

Loglanan temel olaylar:

- Kullanıcı girişi başarılı
- Kullanıcı girişi başarısız
- Yeni sipariş oluşturuldu
- Admin sipariş durumunu değiştirdi
- Yorum eklendi
- Yorum silindi

`Logs/` klasörü `.gitignore` içindedir. Log dosyaları kullanıcı davranışları, hata detayları veya ortam bilgileri içerebileceği için GitHub'a eklenmez.

## Global Hata Yönetimi

Beklenmeyen hatalar `LibraryManagement.Web/Middleware/GlobalExceptionMiddleware.cs` içinde tek noktadan yönetilir.

- `/api` ile başlayan veya `Accept: application/json` isteyen isteklerde standart JSON hata cevabı döner.
- MVC isteklerinde kullanıcı stack trace görmez, `/Home/Error` sayfasına yönlendirilir.
- Development ortamında API hata cevabına kısa teknik `detail` alanı eklenir.
- NotFound gibi beklenen senaryolar controller içinde uygun HTTP sonucu ile yönetilmeye devam eder.
