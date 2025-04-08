# 🛒 OnlineShopApp - Çok Katmanlı ASP.NET Core Web API

Bu proje, çok katmanlı mimari (Multi-Layer Architecture) yapısında geliştirilmiş, ASP.NET Core Web API ile oluşturulmuş bir online alışveriş platformudur.  
Proje Entity Framework Core ile veritabanı işlemlerini yönetir ve modern web servis gereksinimlerini karşılayacak şekilde yapılandırılmıştır.

---

## 🔧 Kullanılan Teknolojiler

- **ASP.NET Core Web API**
- **Entity Framework Core - Code First**
- **JWT Authentication & Authorization**
- **Middleware (Logging & Maintenance)**
- **Model Validation**
- **Global Exception Handling**
- **Caching**
- **Paging (Sayfalama)**
- **Dependency Injection**
- **Data Protection**

## 📁 Katmanlar

### 🟢 OnlineShopApp.WebApi (Presentation Layer)

- Controller'lar (Order, Product, Auth, Settings)
- Middleware (Logging, Maintenance)
- Filter: TimeControlFilter
- JWT işlemleri
- Request/Response modelleri

### 🟡 OnlineShopApp.Business (Business Layer)

- Manager sınıfları (UserManager, ProductManager, OrderManager, vs.)
- Servis arayüzleri
- DTO’lar
- DataProtection

### 🔵 OnlineShopApp.Data (Data Access Layer)

- Entity sınıfları: UserEntity, ProductEntity, OrderEntity, OrderProductEntity
- Repository & Unit of Work pattern
- Enums (UserRole)
- DbContext (OnlineShopAppDbContext)
---
### 🏗️ Base Entity ve Konfigürasyon

Tüm modeller `BaseEntity` sınıfından türetilmiştir:

```csharp
public class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}
```

Ve her tablo konfigürasyonunu `BaseConfiguration<TEntity>` ile yapar:

```csharp
public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(x => x.IsDeleted == false);
        builder.Property(x => x.UpdatedDate).IsRequired(false);
    }
}
```
## 🧩 Veri Modelleri

### 👤 Kullanıcı (User)

| Alan         | Tip          |
|--------------|--------------|
| FirstName    | string       |
| LastName     | string       |
| Email        | string       |
| PhoneNumber  | string       |
| Password     | string       |
| Role         | Enum (Admin, Customer) |

---

### 📦 Ürün (Product)

| Alan         | Tip      |
|--------------|----------|
| ProductName  | string   |
| Price        | decimal  |
| StockQuantity | int     |

---

### 📄 Sipariş (Order)

| Alan         | Tip       |
|--------------|-----------|
| OrderDate    | DateTime  |
| TotalAmount  | decimal   |
| CustomerId   | int (FK)  |

---

### 🔁 Sipariş Ürün (OrderProduct)

Çoka-çok ilişkiyi temsil eder.

| Alan       | Tip     |
|------------|---------|
| OrderId    | int     |
| ProductId  | int     |
| Quantity   | int     |

İlişki yapısı:

```
OrderEntity
  ⬌ (Many-to-Many)
     ⬍ OrderProductEntity ⬌ ProductEntity
```
---
### ⚙️ Ayarlar (SettingEntity)

| Alan             | Tip     |
|------------------|---------|
| MaintenenceMode  | bool    |

---



## 🔒 Kimlik Doğrulama & Yetkilendirme

- JWT Token ile kullanıcı girişi sağlanır.
- Rol bazlı erişim: `[Authorize(Roles = "Admin")]` .
- Kendi `UserEntity` yapısıyla **custom identity** uygulanmıştır.
- Data Protection ile şifreler güvenli şekilde saklanır.

---

## 🛠 Middleware Özellikleri

- **ExceptionHandlingMiddleware**: Hataları global olarak yakalar.
- **MaintenanceMiddleware**: Sistemin bakımda olduğu durumlarda API erişimini kısıtlar.

---

## 🕑 Action Filter

- `TimeControlFilter` ile API'lere belirli saat aralığında erişim izni verilir.  
  (Örn: 09:00 - 17:00)

---

## ✅ Model Validation

- `[Required]`, `[EmailAddress]` gibi **data annotation**'lar ile model validasyonu yapılmıştır.

---

## 📦 Ek Özellikler

### 1️⃣ Paging (Sayfalama)

- Tüm listeleme işlemleri `PagedResult<T>` yapısıyla döner.
- Sayfa numarası ve sayfa başına düşen kayıt miktarı alınabilir.

### 2️⃣ Caching

- Sisteme giriş yapan kullanıcıların bilgileri cache'de saklanarak işlem performansı artırılmıştır

---

## 🗂 Projeyi Çalıştırma

1. `appsettings.json` dosyasındaki **Connection String**’i kendi veritabanı bağlantınıza göre güncelleyin.

2. Gerekli NuGet paketlerini yükleyin:

```bash
dotnet restore
```

3. Veritabanını oluşturun:

```bash
dotnet ef database update
```

4. Uygulamayı başlatın:

```bash
dotnet run
```

---

## 📬 İletişim

Proje geliştiricisi: **Gamze Bilge**  


