# LibraryManagement.Web

![LibraryManagement CI](https://github.com/USERNAME/REPOSITORY/actions/workflows/ci.yml/badge.svg)

ASP.NET Core MVC ile geliştirilmiş, kitap satış/kütüphane yönetimi senaryosunu modelleyen katmanlı bir web uygulamasıdır. Proje; kullanıcı tarafında kitap listeleme, filtreleme, sepet, sipariş, favori, yorum, kupon ve PDF fatura akışları; admin tarafında kitap, kategori, sipariş, kupon, yorum ve rapor yönetimi; ayrıca REST API ve Swagger desteği içerir.

Bu proje junior .NET developer portfoyu icin MVC, Entity Framework Core, katmanli mimari, temel REST API, authentication/authorization ve unit test konularini gosterecek sekilde hazirlanmistir.

## Özellikler

- Kullanıcı kayıt, giriş, çıkış ve şifre değiştirme
- Google login altyapısı; placeholder ayarlarda provider devre dışı kalır
- Role tabanli yetkilendirme: `Admin` ve `User`
- Kitap listeleme, detay sayfası ve gelişmiş filtreleme
- Kitap arama: kitap adı ve yazar üzerinden
- Kategori, fiyat, stok ve sıralama filtreleri
- Sayfalama desteği
- Sepete ekleme, kupon uygulama ve checkout akışı
- Kullanıcı sipariş geçmişi
- PDF fatura indirme
- Favori kitaplar
- Kitap yorumları ve 1-5 yıldız puanlama
- Aynı kullanıcının aynı kitaba tek yorum yapması
- Admin dashboard
- Admin kitap CRUD
- Admin kategori CRUD
- Admin sipariş listeleme, detay, durum ve kargo takip güncelleme
- Admin kupon yönetimi
- Admin yorum listeleme, filtreleme, detay ve silme
- Excel sipariş ve stok raporları
- REST API endpointleri
- Swagger UI
- JWT tabanlı API authentication
- FluentValidation ile form/API validasyonları
- Serilog logging ve global exception middleware
- Docker Compose desteği
- GitHub Actions CI
- xUnit ve Moq ile servis unit testleri

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

## Katmanli Mimari

Projede controller'lar dogrudan `BookContext` kullanmaz. Akis asagidaki sekildedir:

```text
Controller
  -> Service
    -> Repository
      -> BookContext
```

- `Controllers`: MVC ve API request/response akisini yonetir.
- `Areas/Admin`: Admin panel controller ve view yapisini icerir.
- `Services`: Is kurallari ve uygulama mantigini icerir.
- `Repositories`: EF Core sorgularini ve veri erisimini icerir.
- `Data/BookContext.cs`: EF Core DbContext yapisidir.
- `Entity`: Veritabani entity siniflarini icerir.
- `Models` ve `Areas/Admin/Models`: ViewModel ve DTO yapilarini icerir.
- `LibraryManagement.Tests`: Servis katmani unit testlerini icerir.

## Veritabani Yapisi

Temel tablolar:

- `Books`: Kitap bilgileri
- `Genres`: Kategori bilgileri
- `Users`: Kullanici bilgileri ve role alani
- `Orders`: Siparis genel bilgileri ve durum bilgisi
- `OrderItems`: Siparis urunleri
- `Favorites`: Kullanici favorileri
- `Addresses`: Kullanici adresleri
- `BookReviews`: Kitap yorumlari ve puanlari
- `BookSuggestions`: Kitap onerileri

Iliskiler:

- `Book` - `Genre`: Many-to-many
- `Order` - `OrderItem`: One-to-many
- `Book` - `BookReview`: One-to-many
- `User` - `BookReview`: One-to-many
- `User` - `Favorite`: One-to-many
- `User` - `Address`: One-to-many

## Kurulum Adimlari

1. Repository'yi klonlayin.

```bash
git clone <repository-url>
cd LibraryManagement
```

2. Paketleri restore edin.

```bash
dotnet restore
```

3. `LibraryManagement.Web/appsettings.json` icindeki connection string'i kendi ortaminiza gore duzenleyin.

Ornek LocalDB connection string:

```json
"ConnectionStrings": {
  "MsSQLConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Ornek SQL Server connection string:

```json
"ConnectionStrings": {
  "MsSQLConnection": "Server=localhost;Database=LibraryManagementDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}
```

4. Veritabanini olusturun.

```bash
dotnet ef database update --project LibraryManagement.Web
```

5. Uygulamayi calistirin.

```bash
dotnet run --project LibraryManagement.Web
```

## Migration Komutlari

Yeni migration olusturmak icin:

```bash
dotnet ef migrations add MigrationName --project LibraryManagement.Web
```

Veritabanini guncellemek icin:

```bash
dotnet ef database update --project LibraryManagement.Web
```

Son migration'i geri almak icin:

```bash
dotnet ef migrations remove --project LibraryManagement.Web
```

## Admin Giris Bilgileri

Seed data calistiginda admin kullanicisi otomatik olusturulur.

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

> Not: Canli ortamda seed admin sifresi mutlaka degistirilmelidir.

## Swagger Kullanimi

Development ortaminda Swagger UI aktiftir.

```text
/swagger
```

API GET endpointleri public olarak kullanilabilir. Kitap ekleme, guncelleme ve silme endpointleri `Admin` rolune aciktir.



## Continuous Integration

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

- Teslimat Adresi
- Ödeme Yöntemi
- Sipariş Özeti

Kullanıcı sipariş vermeden önce kayıtlı adreslerinden birini seçmelidir. Adres yoksa checkout ekranında adres ekleme linki gösterilir.

Desteklenen ödeme yöntemleri:

- Kapıda ödeme
- Kredi kartı simülasyonu
- Havale/EFT

Kredi kartı simülasyonunda kart sahibi, kart numarası, son kullanma tarihi ve CVV alanları doğrulanır. Gerçek ödeme entegrasyonu yapılmaz ve kart bilgileri veritabanına kaydedilmez.

Kargo hesaplama kuralı:

- Sepet toplamı 500 TL altındaysa kargo 49.90 TL
- 500 TL ve üzeri siparişlerde kargo ücretsiz

Sipariş oluşturulurken ürün fiyatları veritabanından tekrar hesaplanır, stok yeniden kontrol edilir ve siparişe ara toplam, kargo ücreti, genel toplam, teslimat adresi ve ödeme yöntemi yazılır.

## Kupon/İndirim Sistemi

Checkout sırasında kullanıcı kupon kodu girebilir ve geçerli kupon uygulandığında sipariş toplamından indirim düşülür.

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

Admin sipariş yönetiminde siparişe kargo şirketi ve takip numarası girilebilir. Sipariş durumu `Shipped` yapıldığında `ShippedAt`, `Delivered` yapıldığında `DeliveredAt` otomatik olarak set edilir.

Kullanıcı sipariş geçmişinde şu kargo bilgilerini görebilir:

- Kargo şirketi
- Takip numarası
- Kargoya verildi tarihi
- Teslim edildi tarihi

Sipariş geçmişinde basit takip adımları gösterilir:

- Sipariş Alındı
- Hazırlanıyor
- Kargoya Verildi
- Teslim Edildi

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

Endpointler:

- `GET /Admin/Reports/OrdersExcel`
- `GET /Admin/Reports/StockExcel`

Rapor dosya adları tarih bazlı oluşturulur:

- `orders-report-yyyyMMdd.xlsx`
- `stock-report-yyyyMMdd.xlsx`

Excel üretimi için `ClosedXML` kullanılır. Header satırları kalın biçimlendirilir, kolon genişlikleri otomatik ayarlanır ve para alanları TL formatına uygun yazılır.

Rapor indirme ve rapor oluşturma hataları Serilog ile loglanır.
## Stok Uyarı Sistemi

Admin panelde stok seviyesi düşük kitaplar daha görünür hale getirilmiştir. Stok kuralları:

- `InStock`: Stok 5'in üzerinde
- `LowStock`: Stok 1-5 arasında
- `OutOfStock`: Stok 0 veya daha az

Admin Dashboard düşük stok bölümünde `Düşük Stok` ve `Stok Yok` badge'leri gösterilir. Admin kitap listesinde stok durumu badge olarak görünür ve stok filtresi ile `Tümü`, `Stokta olanlar`, `Düşük stok`, `Stokta olmayanlar` seçenekleri kullanılabilir.

Kullanıcı tarafında stokta olmayan kitaplarda `Sepete Ekle` butonu pasif hale gelir ve `Stokta Yok` mesajı gösterilir. Checkout sırasında stok tekrar kontrol edilir; stok yetersizse hangi kitapta sorun olduğu açık mesajla kullanıcıya bildirilir.

Sipariş sonrası kitap stoku 0'a düşerse veya düşük stok seviyesine inerse Serilog ile log yazılır.
## Google Login

Web arayüzünde kullanıcılar Google hesabı ile giriş yapabilecek şekilde external login altyapısı eklenmiştir. Mevcut kullanıcı adı/şifre ile giriş, MVC cookie authentication, JWT API login ve admin panel akışı korunur.

Google login kullanmak için Google Cloud Console üzerinden OAuth Client oluşturulmalıdır:

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

`appsettings.json` içinde yalnızca placeholder değerler bulunur. Bu değerler değiştirilmediyse Google login provider aktif edilmez ve login ekranında Google butonu gösterilmez.
## PDF Fatura Sistemi

Kullanıcılar kendi sipariş detayları için PDF fatura indirebilir. Admin kullanıcılar da admin sipariş detay ekranından aynı faturayı indirebilir.

PDF üretimi için `QuestPDF` paketi kullanılır. Bu projede portfolio/geliştirme amacıyla Community lisans ayarı yapılmıştır. Oluşturulan belge resmi e-fatura entegrasyonu değildir; proje içi örnek fatura çıktısıdır.

Fatura içeriğinde şunlar yer alır:

- Sipariş numarası ve tarihi
- Kullanıcı bilgisi
- Teslimat adresi
- Ödeme yöntemi ve sipariş durumu
- Varsa kargo şirketi ve takip numarası
- Ürün listesi, adet, birim fiyat ve toplam fiyat
- Ara toplam, kargo ücreti, kupon kodu, indirim ve genel toplam

Endpointler:

- Kullanıcı: `GET /Cart/Invoice/{orderId}`
- Admin: `GET /Admin/Orders/Invoice/{orderId}`

Kullanıcı sadece kendi siparişinin faturasını indirebilir. Yetkisiz fatura erişim denemeleri ve fatura oluşturma hataları Serilog ile loglanır.
## Docker ile Çalıştırma

Proje Docker Compose ile web uygulamasi ve SQL Server container'i birlikte calisacak sekilde hazirlanmistir.

```bash
docker compose up --build
```

Calisma adresleri:

```text
Web:     http://localhost:5000
Swagger: http://localhost:5000/swagger
```

Compose servisleri:

- `web`: ASP.NET Core MVC/API uygulamasi
- `sqlserver`: SQL Server 2022 container

Docker ortaminda connection string environment variable olarak verilir:

```text
ConnectionStrings__DefaultConnection=Server=sqlserver,1433;Database=LibraryManagementDb;User Id=sa;Password=YourStrong!Passw0rd2026;TrustServerCertificate=True;MultipleActiveResultSets=true
```

Geriye uyumluluk icin compose dosyasinda `ConnectionStrings__MsSQLConnection` da ayni degere ayarlanmistir.

Migration konusu:

- Uygulama baslarken `DataSeeding.Seed(app)` icinde `Database.Migrate()` calisir.
- Bu nedenle container ilk acilista migration'lari SQL Server container'a uygular.
- SQL Server ilk acilista hazir olana kadar web container yeniden baslayabilir; `restart: on-failure` bunun icin eklenmistir.

Dikkat edilmesi gerekenler:

- `SA_PASSWORD` ornek gelistirme parolasidir, gercek ortamda degistirilmelidir.
- `Jwt__Key` ornek gelistirme secret'idir, gercek ortamda guvenli secret kullanilmalidir.
- Serilog dosya loglari container icinde `/app/Logs` altina yazilir ve host tarafinda `LibraryManagement.Web/Logs` klasorune map edilir.
- `Logs/` klasoru GitHub'a eklenmez.
## Test Komutlari

Tum solution'i build etmek icin:

```bash
dotnet build
```

Testleri calistirmak icin:

```bash
dotnet test
```

Sadece test projesini calistirmak icin:

```bash
dotnet test LibraryManagement.Tests/LibraryManagement.Tests.csproj
```

## API Endpoint Listesi

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

## Ekran Görüntüsü Alanları

GitHub README icin asagidaki ekran goruntuleri eklenebilir:

```text
docs/screenshots/home.png
docs/screenshots/books-filter.png
docs/screenshots/book-detail-reviews.png
docs/screenshots/cart-checkout.png
docs/screenshots/admin-dashboard.png
docs/screenshots/admin-books.png
docs/screenshots/admin-orders.png
docs/screenshots/swagger.png
```

Ornek Markdown kullanimi:

```md
![Admin Dashboard](docs/screenshots/admin-dashboard.png)
```

## Gelecekte Yapılabilecek Geliştirmeler

- Admin panelde raporlama grafikleri
- Siparis iptal/iade akisi
- Kullanici profil sayfasi gelistirmeleri
- Kitap gorselleri icin dosya yukleme
- Integration testler

## Lisans

Bu proje MIT lisansi ile lisanslanmistir.

## JWT API Authentication

MVC tarafındaki cookie tabanlı giriş sistemi korunur. REST API tarafında token almak için ayrı endpoint kullanılır.

### API Login

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

### Swagger Authorize

1. Uygulamayı çalıştırın ve Swagger UI sayfasını açın.
2. `POST /api/auth/login` endpointi ile token alın.
3. Swagger'daki `Authorize` butonuna tıklayın.
4. Token değerini `Bearer {token}` formatında girin.
5. Admin rolü isteyen `POST`, `PUT` ve `DELETE /api/books` endpointlerini test edin.

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


## Serilog Logging

Projede uygulama hatalarini ve onemli is olaylarini merkezi olarak takip etmek icin Serilog kullanilir.

- Console loglari gelistirme sirasinda anlik takip icin kullanilir.
- File sink ile loglar `LibraryManagement.Web/Logs/` klasorune gunluk dosyalar halinde yazilir.
- Dosya formati `logs-.txt` rolling file yapisindadir.
- Varsayilan minimum seviye `Information`, `Microsoft` ve `System` loglari `Warning` seviyesindedir.
- GlobalExceptionMiddleware icindeki mevcut `ILogger` kullanimi Serilog pipeline'i uzerinden calisir.

Loglanan temel olaylar:

- Kullanici login basarili
- Kullanici login basarisiz
- Yeni siparis olusturuldu
- Admin siparis durumu degistirdi
- Yorum eklendi
- Yorum silindi

`Logs/` klasoru `.gitignore` icindedir. Log dosyalari kullanici davranislari, hata detaylari veya ortam bilgileri icerebilecegi icin GitHub'a eklenmez.

## Global Exception Middleware

Beklenmeyen hatalar `LibraryManagement.Web/Middleware/GlobalExceptionMiddleware.cs` içinde tek noktadan yönetilir.

- `/api` ile başlayan veya `Accept: application/json` isteyen isteklerde standart JSON hata cevabı döner.
- MVC isteklerinde kullanıcı stack trace görmez, `/Home/Error` sayfasına yönlendirilir.
- Development ortamında API hata cevabına kısa teknik `detail` alanı eklenir.
- NotFound gibi beklenen senaryolar controller içinde uygun HTTP sonucu ile yönetilmeye devam eder.













