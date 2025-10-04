# 🎮 Match-3 Puzzle Game - Portfolio Projesi

> **Unity Game Developer** pozisyonu için hazırladığım profesyonel portfolyo projesi.  
> Temiz mimari, endüstri standartları, test edilebilir kod ve performans optimizasyonlarına odaklanılarak geliştirildi.

---

## 📋 Proje Hakkında

Bu proje, modern oyun geliştirme standartlarını ve best practice'leri göstermek amacıyla sıfırdan geliştirilen bir **Match-3 Puzzle** oyunudur.

### 🎯 Proje Amaçları
- ✅ **Clean Architecture** prensiplerine uygun kod yapısı
- ✅ **SOLID** prensiplerinin uygulanması
- ✅ **Design Pattern** kullanımı ve doğru uygulanması
- ✅ **Unit Test** ile test edilebilir kod yazımı
- ✅ **Performans odaklı** optimizasyon teknikleri
- ✅ **Takım çalışmasına uygun** kod organizasyonu
- ✅ **Ölçeklenebilir** mimari tasarımı

---

## 🏗️ Mimari ve Tasarım Desenleri

### **Kullanılan Mimari**
- **Clean Architecture**: Katmanlı mimari ile sorumlulukların ayrılması
- **MVC/MVVM Hybrid**: View ve Logic ayrımı
- **Dependency Injection**: Service Locator pattern ile

### **Uygulanan Design Pattern'ler**

#### 1️⃣ **Service Locator Pattern**
```csharp
// Merkezi servis yönetimi ve dependency resolution
ServiceLocator.Register<IAudioService>(new AudioService());
var audioService = ServiceLocator.Get<IAudioService>();
```
**Avantajları:**
- Servisler arası gevşek bağlantı (loose coupling)
- Kolay test edilebilirlik
- Merkezi servis yaşam döngüsü yönetimi

#### 2️⃣ **Observer Pattern (Event System)**
```csharp
// Sistemler arası loosely-coupled iletişim
GameEvents.OnMatchFound += HandleMatchFound;
GameEvents.OnScoreChanged?.Invoke(newScore);
```
**Avantajları:**
- Sistemler arası bağımlılık azaltma
- Event-driven mimari
- Kolay debug ve takip edilebilirlik

#### 3️⃣ **Singleton Pattern**
```csharp
// Bootstrapper ile oyun başlangıç noktası
public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }
}
```
**Kullanım Alanı:**
- Oyun yaşam döngüsü yönetimi
- DontDestroyOnLoad objeleri

#### 4️⃣ **Factory Pattern**
```csharp
// Tile oluşturma için factory pattern (yakında)
TileFactory.CreateTile(TileType.Red, x, y);
```

#### 5️⃣ **Object Pool Pattern**
```csharp
// Bellek optimizasyonu için pooling (yakında)
TilePool.Get() / TilePool.Return(tile);
```

---

## 🧩 Proje Yapısı

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Core/              # Temel altyapı
│   │   │   ├── IService.cs           # Servis arayüzü
│   │   │   ├── ServiceLocator.cs     # Servis yönetimi
│   │   │   ├── Bootstrapper.cs       # Oyun başlatıcı
│   │   │   └── GameEvents.cs         # Event sistemi
│   │   │
│   │   ├── Game/              # Oyun mantığı
│   │   │   ├── Tile.cs               # Tile veri modeli
│   │   │   ├── Grid.cs               # Grid sistemi
│   │   │   └── Game.asmdef           # Assembly definition
│   │   │
│   │   └── Utilities/         # Yardımcı sınıflar
│   │       └── GameConstants.cs      # Sabitler
│   │
│   └── Scenes/                # Oyun sahneleri
│       └── Test.unity
│
└── Tests/                     # Unit testler
    ├── TileTests.cs          # Tile testleri (6 test)
    ├── GridTests.cs          # Grid testleri (10 test)
    └── Tests.asmdef          # Test assembly definition
```

---

## 🧪 Test Coverage

### **Unit Test Stratejisi**
- ✅ **EditMode Tests**: Core logic testleri (şu anki durum)
- ⏳ **PlayMode Tests**: Oyun mekaniği testleri (gelecek)

### **Mevcut Test Kapsamı**
```
✅ TileTests.cs (6 test)
   - Constructor validation
   - Neighbor detection (horizontal, vertical, diagonal)
   - Match validation (same type, different type)

✅ GridTests.cs (10 test)
   - Grid initialization
   - Tile placement and retrieval
   - Position validation
   - Tile swapping
   - Neighbor queries
   - Grid clearing
```

**Test Sonucu:** 🟢 **16/16 başarılı**

### **Test Felsefesi**
- **Arrange-Act-Assert** pattern kullanımı
- Açıklayıcı test isimlendirmesi (`Method_Scenario_ExpectedResult`)
- Her test tek bir sorumluluğu test eder
- Edge case'lerin test edilmesi

---

## 💻 Teknik Özellikler

### **Kod Kalitesi**
- ✅ **XML Documentation**: Tüm public member'lar için
- ✅ **Naming Conventions**: C# ve Unity standartlarına uygun
- ✅ **SOLID Principles**: Her sınıf tek sorumluluk prensibine uygun
- ✅ **Namespace Organization**: `Yunus.Match3` namespace yapısı
- ✅ **Assembly Definitions**: Derleme optimizasyonu için

### **Performans Hedefleri**
- 🎯 **60 FPS** stabil frame rate
- 🎯 **<150MB** bellek kullanımı
- 🎯 **<5KB/frame** GC allocation
- 🎯 Object Pooling ile allocation azaltma

### **Unity Özellikleri**
- **Unity Version**: 2022.3 LTS (önerilen)
- **Scripting Backend**: IL2CPP (release build)
- **API Compatibility**: .NET Standard 2.1

---

## 🚀 Şu Ana Kadar Tamamlananlar

### ✅ **Sprint 1: Core Infrastructure**
- [x] Service Locator pattern implementasyonu
- [x] Event System (Observer pattern)
- [x] Bootstrapper (Singleton pattern)
- [x] Game Constants
- [x] IService interface

### ✅ **Sprint 2: Game Logic Foundation**
- [x] Tile class (Pure C# data model)
- [x] Grid class (Pure C# board logic)
- [x] TileType enum
- [x] Namespace organization (Yunus.Match3)

### ✅ **Sprint 3: Testing Infrastructure**
- [x] Assembly Definition setup
- [x] Unit Test framework configuration
- [x] TileTests implementation (6 tests)
- [x] GridTests implementation (10 tests)
- [x] 100% test success rate

---

## 📅 Gelecek Adımlar (Roadmap)

### ⏳ **Sprint 4: Visual System**
- [ ] TileView (MonoBehaviour - Visual representation)
- [ ] BoardView (Board rendering)
- [ ] TileFactory (Factory pattern)
- [ ] TilePool (Object Pool pattern)

### ⏳ **Sprint 5: Game Mechanics**
- [ ] Match Detection (Flood fill algorithm)
- [ ] Input System (Touch & Mouse support)
- [ ] Tile Swap Animation (DOTween)
- [ ] Gravity System

### ⏳ **Sprint 6: Scoring & Progression**
- [ ] Score System
- [ ] Combo detection
- [ ] Level System (ScriptableObject-based)
- [ ] Save/Load System

### ⏳ **Sprint 7: Polish & Optimization**
- [ ] Audio System (Pooled audio sources)
- [ ] Particle effects
- [ ] UI/UX polish
- [ ] Performance profiling

---

## 📚 Öğrenme Kaynakları ve Referanslar

### **Design Patterns**
- Game Programming Patterns (Robert Nystrom)
- Unity Design Patterns (Habrador)

### **Clean Architecture**
- Clean Architecture (Robert C. Martin)
- Unity Clean Code Principles

### **Testing**
- Unity Test Framework Documentation
- TDD Best Practices

---

## 🛠️ Kurulum ve Çalıştırma

### **Gereksinimler**
- Unity 2022.3 LTS veya üzeri
- Git
- Visual Studio 2022 veya JetBrains Rider

### **Kurulum Adımları**
```bash
# Repository'yi klonlayın
git clone https://github.com/kocyunus/match3-portfolio.git

# Unity ile projeyi açın
# Unity Hub → Add → match3-portfolio klasörünü seçin

# Test Runner'ı açın
# Unity Editor → Window → General → Test Runner

# Testleri çalıştırın
# EditMode → Run All
```

---

## 📊 Proje İstatistikleri

- **Toplam Kod Satırı**: ~2300 satır
- **Test Coverage**: 16 unit test
- **Commit Sayısı**: Düzenli ve anlamlı commit'ler
- **Code Quality**: Clean Code prensiplerine uygun

---

## 👤 Geliştirici

**Yunus Koç**  
Unity Game Developer

### **İletişim**
- GitHub: [github.com/kocyunus](https://github.com/kocyunus)
- LinkedIn: [Profiliniz]
- Email: [Email adresiniz]

---

## 📝 Notlar

### **Neden Bu Yaklaşımlar?**

#### **Service Locator vs Dependency Injection**
Service Locator seçtim çünkü:
- Unity MonoBehaviour lifecycle'ı ile uyumlu
- Constructor injection Unity'de pratik değil
- Kolay test edilebilir (mock service'ler)
- Takım çalışmasında daha anlaşılır

#### **Pure C# Classes (Tile, Grid)**
Tile ve Grid'i MonoBehaviour yapmadım çünkü:
- Unit test daha kolay (PlayMode'a ihtiyaç yok)
- Performans artışı (MonoBehaviour overhead'i yok)
- Data ve Logic ayrımı (Clean Architecture)
- View katmanı bağımsız (TileView/BoardView ayrı)

#### **Assembly Definitions**
Assembly Definition kullandım çünkü:
- Compile time azaltma (incremental compilation)
- Test izolasyonu
- Dependency management
- Production build'de test kodları yok

---

## 🏆 Hedef Şirket Profili

Bu proje özellikle şu tür şirketler için tasarlandı:
- ✅ Mid-large scale game studios
- ✅ Clean code ve best practice odaklı ekipler
- ✅ Test-driven development uygulayan şirketler
- ✅ Scrum/Agile metodolojisi kullanan teams

---

## 📄 Lisans

Bu proje portfolyo amaçlı geliştirilmiştir.

---

## 🙏 Teşekkürler

Bu projeyi incelediğiniz için teşekkür ederim!  
Sorularınız için benimle iletişime geçebilirsiniz.

---

**Son Güncelleme:** Ekim 2025  
**Durum:** 🟢 Aktif Geliştirme

