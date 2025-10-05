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

#### 4️⃣ **Command Pattern**
```csharp
// Tile swap işlemleri için Command Pattern (undo desteği)
SwapCommand command = new SwapCommand(tile1, tile2, grid);
command.Execute();  // Swap yap
command.Undo();     // Geri al (match yoksa)
```
**Avantajları:**
- Undo/Redo desteği
- İşlem geçmişi tutma
- Test edilebilir oyun logici

#### 5️⃣ **Strategy Pattern**
```csharp
// Farklı match türleri için Strategy Pattern
public interface IMatchStrategy
{
    bool HasMatch(Tile tile, Grid grid);
    List<Tile> FindMatches(Tile tile, Grid grid);
}

// Yatay/Dikey eşleşme stratejisi
public class LineMatchStrategy : IMatchStrategy { }

// Gelecekte: L-shape, T-shape stratejileri eklenebilir
```
**Avantajları:**
- Yeni match türleri kolay eklenebilir
- Open/Closed Principle
- A/B testing için ideal

#### 6️⃣ **Factory Pattern** ⏳
```csharp
// Tile oluşturma için factory pattern (yakında)
TileFactory.CreateTile(TileType.Red, x, y);
```

#### 7️⃣ **Object Pool Pattern** ⏳
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
│   │   ├── Match3.asmdef          # Ana assembly definition
│   │   │
│   │   ├── Core/                  # Temel altyapı
│   │   │   ├── IService.cs             # Servis arayüzü
│   │   │   ├── ServiceLocator.cs       # Servis yönetimi
│   │   │   ├── Bootstrapper.cs         # Oyun başlatıcı
│   │   │   ├── GameEvents.cs           # Event sistemi
│   │   │   └── CameraFitter.cs         # Responsive kamera
│   │   │
│   │   ├── Game/                  # Oyun mantığı
│   │   │   ├── Tile.cs                 # Tile data model
│   │   │   ├── Grid.cs                 # Grid sistemi
│   │   │   ├── TileView.cs             # Tile görsel (MonoBehaviour)
│   │   │   ├── BoardView.cs            # Board rendering
│   │   │   ├── BoardController.cs      # Board logic
│   │   │   ├── SwapCommand.cs          # Command Pattern
│   │   │   │
│   │   │   └── MatchStrategies/        # Strategy Pattern
│   │   │       ├── MatchDetector.cs
│   │   │       └── LineMatchStrategy.cs
│   │   │
│   │   ├── Interfaces/            # Abstraction layer
│   │   │   ├── IInputHandler.cs
│   │   │   ├── IMatchStrategy.cs
│   │   │   └── IMatchDetector.cs
│   │   │
│   │   ├── Services/              # Game services
│   │   │   └── MouseInputHandler.cs    # Input sistemi
│   │   │
│   │   └── Utilities/             # Yardımcı sınıflar
│   │       └── GameConstants.cs        # Sabitler
│   │
│   ├── Art/                       # Görsel varlıklar
│   │   ├── red.png
│   │   ├── blue.png
│   │   ├── green.png
│   │   ├── yellow.png
│   │   ├── purple.png
│   │   └── white.png
│   │
│   ├── Prefabs/                   # Prefab'lar
│   │   ├── Tile_Prefab.prefab
│   │   └── BackgroundTile.prefab
│   │
│   └── Scenes/                    # Oyun sahneleri
│       └── Test.unity
│
├── Plugins/                       # Third-party assets
│   └── DOTween/                   # Animation library
│
└── Tests/                         # Unit testler
    ├── Match3.Tests.asmdef        # Test assembly definition
    ├── TileTests.cs               # Tile testleri (6 test)
    └── GridTests.cs               # Grid testleri (10 test)
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
- **Third-Party**: DOTween (animation library)
- **Physics**: 2D Physics (BoxCollider2D, Raycast2D)
- **Input**: Mouse + Swipe detection (Touch-ready)

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
- [x] Assembly Definition setup (Match3.asmdef + Match3.Tests.asmdef)
- [x] Unit Test framework configuration
- [x] TileTests implementation (6 tests)
- [x] GridTests implementation (10 tests)
- [x] 100% test success rate
- [x] Assembly reference sorunu çözüldü

### ✅ **Sprint 4: Visual System**
- [x] TileView (MonoBehaviour - Visual representation)
- [x] BoardView (Board rendering)
- [x] BoardController (MVC pattern - Logic/View ayrımı)
- [x] CameraFitter (Responsive camera system)
- [x] DOTween entegrasyonu (Smooth animations)
- [x] 6 Tile sprite (red, blue, green, yellow, purple, white)
- [x] Tile prefab + Background tile prefab
- [x] Layer sistemi düzeltildi (Tile layer, BoxCollider2D)

### ✅ **Sprint 5: Game Mechanics** (Kısmen Tamamlandı)
- [x] Match Detection (Strategy Pattern)
- [x] LineMatchStrategy (Horizontal/Vertical matches)
- [x] MatchDetector (Extensible match system)
- [x] Input System (MouseInputHandler - Swipe detection)
- [x] Tile Swap (SwapCommand - Command Pattern, undo desteği)
- [x] Interface'ler (IMatchStrategy, IMatchDetector, IInputHandler)
- [ ] Tile destruction & animations
- [ ] Gravity System (tiles falling)
- [ ] Board refill system
- [ ] Cascade detection (chain reactions)

---

## 📅 Gelecek Adımlar (Roadmap)

### ⏳ **Sprint 6: Core Mechanics Completion**
- [ ] Tile destruction & match animations (DOTween)
- [ ] Gravity System (tiles falling down)
- [ ] Board refill system (spawn new tiles)
- [ ] Cascade detection (chain reactions)
- [ ] Invalid move feedback (shake animation)

### ⏳ **Sprint 7: Performance & Optimization**
- [ ] TileFactory (Factory pattern)
- [ ] TilePool (Object Pool pattern)
- [ ] Memory profiling & optimization
- [ ] GC allocation optimization
- [ ] Layer mask optimizations

### ⏳ **Sprint 8: Scoring & Progression**
- [ ] Score System (ScoreService)
- [ ] Combo detection & multipliers
- [ ] Level System (ScriptableObject-based)
- [ ] Star rating system
- [ ] Save/Load System (PlayerPrefs)

### ⏳ **Sprint 9: Polish & Juice**
- [ ] Audio System (Pooled audio sources)
- [ ] Match particle effects (VFX)
- [ ] UI/UX implementation (Main Menu, HUD, Pause)
- [ ] Screen shake on combos
- [ ] Tile spawn animations
- [ ] Special tile effects (4-match, 5-match)

### ⏳ **Sprint 10: Advanced Features** (Optional)
- [ ] Power-ups system (Bomb, Rocket, Color Bomb)
- [ ] Touch input support (mobile)
- [ ] L-shape & T-shape match detection
- [ ] Tutorial system
- [ ] Achievement system

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

- **Toplam Kod Satırı**: ~3,500 satır
- **C# Script Dosyaları**: 24 dosya
- **Test Coverage**: 16 unit test (100% passed)
- **Design Patterns**: 5 pattern uygulandı
- **Assembly Definitions**: 2 asmdef (oyun + test)
- **Commit Sayısı**: Düzenli ve anlamlı commit'ler
- **Code Quality**: Clean Code prensiplerine uygun (SOLID + XML docs)
- **Third-Party**: DOTween (animation library)

---

## 👤 Geliştirici

**Yunus Koç**  
Unity Game Developer

### **İletişim**
- GitHub: [github.com/kocyunus](https://github.com/kocyunus)
- LinkedIn: [Yunus Koç](https://www.linkedin.com/in/yunus-ko%C3%A7/)
- Email: kocyns1@gmail.com

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

## 💎 Teknik Highlights

### **Architecture & Design**
- ✅ **Clean Architecture**: Katmanlı mimari (Data → Logic → View)
- ✅ **MVC Pattern**: BoardController (Logic) + BoardView (View) ayrımı
- ✅ **SOLID Principles**: Her sınıf tek sorumluluk, interface segregation
- ✅ **5 Design Pattern**: Service Locator, Observer, Singleton, Command, Strategy

### **Code Quality**
- ✅ **XML Documentation**: Tüm public API'ler için detaylı açıklamalar
- ✅ **Unit Tests**: 16/16 test passed (100% success rate)
- ✅ **Naming Conventions**: C# ve Unity standartlarına uygun
- ✅ **Assembly Definitions**: Compile time optimizasyonu ve modüler yapı
- ✅ **Method Length**: Her metod <30 satır, anlaşılır ve maintainable

### **Performance & Optimization**
- ✅ **Layer Mask**: Raycast optimizasyonu (sadece Tile layer)
- ✅ **2D Physics**: BoxCollider2D kullanımı (2D oyun için doğru)
- ✅ **Pure C# Classes**: Tile ve Grid MonoBehaviour değil (performance)
- ✅ **Event-Driven**: Observer pattern ile loose coupling
- 🔜 **Object Pooling**: Yakında eklenecek (memory optimization)

### **Extensibility**
- ✅ **Strategy Pattern**: Yeni match türleri kolayca eklenebilir
- ✅ **Command Pattern**: Undo/Redo sistemi için hazır
- ✅ **Interface-Based**: IMatchStrategy, IInputHandler, IMatchDetector
- ✅ **Open/Closed**: Yeni özellikler eklemek için modify değil extend

### **Testing**
- ✅ **Arrange-Act-Assert**: Test pattern best practice
- ✅ **Test Isolation**: Her test bağımsız çalışır
- ✅ **Edge Cases**: Boundary conditions test edildi
- ✅ **Assembly Definition**: Test ve production kod ayrımı

### **Unity Best Practices**
- ✅ **DOTween**: Industry-standard animation library
- ✅ **Responsive Camera**: Tüm cihazlarda çalışır (CameraFitter)
- ✅ **Namespace Organization**: Yunus.Match3 namespace
- ✅ **Prefab System**: Modular tile sistemi
- ✅ **ScriptableObject Ready**: Config sistemi için hazır altyapı

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

