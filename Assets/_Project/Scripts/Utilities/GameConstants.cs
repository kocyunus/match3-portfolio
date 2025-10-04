/// <summary>
/// Oyunun tüm sabit değerlerini içeren merkezi sınıf.
/// 
/// NE İŞE YARAR:
/// "Magic number" (sihirli sayı) kullanımını önler. Kodda rastgele
/// sayılar görmek yerine, anlamlı isimlerle sabitleri tutarız.
/// 
/// KÖTÜ ÖRNEK:
/// if (score > 1000) { }  // 1000 ne anlama geliyor?
/// 
/// İYİ ÖRNEK:
/// if (score > GameConstants.THREE_STAR_THRESHOLD) { }  // Açık ve anlaşılır!
/// 
/// AVANTAJLARI:
/// ✅ Okunabilirlik: Kodun ne yaptığı hemen anlaşılır
/// ✅ Bakım kolaylığı: Tek yerden tüm değerleri değiştir
/// ✅ Hata önleme: Yanlış değer kullanımını engeller
/// ✅ Team çalışması: Herkes aynı değerleri kullanır
/// 
/// SEKTÖR STANDARDI:
/// Clean Code prensiplerinin temel taşlarından biri.
/// Profesyonel her projede bulunması gereken bir yapı.
/// 
/// NOT:
/// İlerleye ScriptableObject'e geçiş yapabiliriz ama başlangıç için
/// constants daha pratik ve hızlıdır.
/// </summary>
public static class GameConstants
{
    #region Board Settings (Tahta Ayarları)
    // Oyun tahtasının standart boyutları
    // Match-3 oyunlarında en yaygın kullanılan boyut 8x8'dir
    // (Candy Crush, Bejeweled, Homescapes)

    /// <summary>
    /// Oyun tahtasının genişliği (sütun sayısı).
    /// Standart: 8 tile genişliğinde
    /// </summary>
    public const int BOARD_WIDTH = 8;

    /// <summary>
    /// Oyun tahtasının yüksekliği (satır sayısı).
    /// Standart: 8 tile yüksekliğinde
    /// </summary>
    public const int BOARD_HEIGHT = 8;

    /// <summary>
    /// İki tile arasındaki boşluk (Unity units cinsinden).
    /// 1.0f = tile'lar tam yanyana
    /// 1.1f = tile'lar arasında hafif boşluk (daha şık görünüm)
    /// </summary>
    public const float TILE_SPACING = 1.0f;

    #endregion

    #region Match Rules (Eşleşme Kuralları)
    // Match-3 mekaniklerinin temel kuralları

    /// <summary>
    /// Minimum kaç tile yan yana olmalı ki eşleşme sayılsın.
    /// Standart Match-3: 3 tile
    /// Bazı oyunlar 4+ için özel bonuslar verir
    /// </summary>
    public const int MIN_MATCH_COUNT = 3;

    /// <summary>
    /// 4 tile eşleşmesi için eşik.
    /// 4-match özel güç (line bomb) oluşturur.
    /// </summary>
    public const int SPECIAL_MATCH_4 = 4;

    /// <summary>
    /// 5 tile eşleşmesi için eşik.
    /// 5-match süper güç (color bomb) oluşturur.
    /// </summary>
    public const int SPECIAL_MATCH_5 = 5;

    #endregion

    #region Scoring (Puanlama)
    // Puan hesaplama sistemi

    /// <summary>
    /// Her bir eşleşen tile için verilen temel puan.
    /// 3 tile = 3 x 10 = 30 puan
    /// </summary>
    public const int POINTS_PER_TILE = 10;

    /// <summary>
    /// Combo çarpanı (maksimum).
    /// 1. hamle: 1x
    /// 2. hamle: 2x
    /// ...
    /// 5. hamle: 5x (maksimum)
    /// </summary>
    public const int MAX_COMBO_MULTIPLIER = 5;

    /// <summary>
    /// 1 yıldız için gereken minimum puan.
    /// </summary>
    public const int ONE_STAR_SCORE = 1000;

    /// <summary>
    /// 2 yıldız için gereken minimum puan.
    /// </summary>
    public const int TWO_STAR_SCORE = 2000;

    /// <summary>
    /// 3 yıldız için gereken minimum puan.
    /// </summary>
    public const int THREE_STAR_SCORE = 3000;

    #endregion

    #region Animation Durations (Animasyon Süreleri)
    // Tüm animasyon sürelerini buradan kontrol ederiz
    // Unity'de süre saniye cinsinden ölçülür (float)

    /// <summary>
    /// Tile swap (takas) animasyon süresi (saniye).
    /// 0.3f = Hızlı ve akıcı (mobile oyunlar için ideal)
    /// 0.5f = Daha yavaş ve görsel (desktop için uygun)
    /// </summary>
    public const float TILE_SWAP_DURATION = 0.3f;

    /// <summary>
    /// Tile match (patlama) animasyon süresi (saniye).
    /// </summary>
    public const float TILE_MATCH_DURATION = 0.25f;

    /// <summary>
    /// Tile fall (düşme) animasyon süresi (saniye).
    /// Her tile için ayrı ayrı uygulanır (cascading effect)
    /// </summary>
    public const float TILE_FALL_DURATION = 0.4f;

    /// <summary>
    /// UI transition (menü geçişi) süresi (saniye).
    /// </summary>
    public const float UI_TRANSITION_DURATION = 0.3f;

    #endregion

    #region Input Settings (Giriş Ayarları)
    // Kullanıcı girişi ile ilgili ayarlar

    /// <summary>
    /// Swipe (kaydırma) için minimum hareket mesafesi (piksel).
    /// Çok düşük: Kazara hamleler
    /// Çok yüksek: Kullanıcı zorlanır
    /// 50f: Mobile için ideal değer
    /// </summary>
    public const float MIN_SWIPE_DISTANCE = 50f;

    /// <summary>
    /// Swipe hız limiti (saniye).
    /// Kullanıcı bu süre içinde swipe'ı tamamlamalı.
    /// 0.5f: Hızlı ama rahat bir limit
    /// </summary>
    public const float MAX_SWIPE_TIME = 0.5f;

    #endregion

    #region Audio Settings (Ses Ayarları)
    // Ses sistemi için sabit değerler

    /// <summary>
    /// Eşzamanlı çalabilecek maksimum ses sayısı.
    /// Object pooling için kullanılır.
    /// 10: Çoğu match-3 oyun için yeterli
    /// </summary>
    public const int MAX_AUDIO_SOURCES = 10;

    /// <summary>
    /// Varsayılan SFX (efekt sesi) ses seviyesi.
    /// 0.0f = Sessiz
    /// 1.0f = Maksimum
    /// 0.7f = Rahat bir seviye
    /// </summary>
    public const float DEFAULT_SFX_VOLUME = 0.7f;

    /// <summary>
    /// Varsayılan müzik ses seviyesi.
    /// Müzik genellikle efektlerden daha sessiz olmalı.
    /// </summary>
    public const float DEFAULT_MUSIC_VOLUME = 0.5f;

    #endregion

    #region Performance Settings (Performans Ayarları)
    // Performans optimizasyonu için sabitler

    /// <summary>
    /// Object pool için başlangıç tile sayısı.
    /// Pre-allocate ederek runtime'da instantiate yapmayız.
    /// 64 = 8x8 tahta + ekstra tile'lar
    /// </summary>
    public const int INITIAL_POOL_SIZE = 64;

    /// <summary>
    /// Particle effect pool boyutu.
    /// Her match için bir particle kullanılır.
    /// </summary>
    public const int PARTICLE_POOL_SIZE = 20;

    /// <summary>
    /// Hedef FPS (frame per second).
    /// Mobile oyunlar için 60 FPS standart.
    /// </summary>
    public const int TARGET_FPS = 60;

    #endregion

    #region Game Rules (Oyun Kuralları)
    // Seviye bazlı oyun kuralları

    /// <summary>
    /// Standart seviye için hamle sayısı.
    /// Kullanıcı bu kadar hamle hakkına sahip.
    /// </summary>
    public const int DEFAULT_MOVE_LIMIT = 20;

    /// <summary>
    /// Combo için maksimum bekleme süresi (saniye).
    /// Kullanıcı bu süre içinde yeni eşleşme yapmazsa combo sıfırlanır.
    /// </summary>
    public const float COMBO_TIMEOUT = 2.0f;

    #endregion

    #region Layer Names (Katman İsimleri)
    // Unity layer'ları için string constant'lar
    // String kullanmak yerine constant kullanmak typo hatalarını önler

    /// <summary>
    /// Tile nesnelerinin layer'ı.
    /// Raycast ve collision detection için kullanılır.
    /// </summary>
    public const string LAYER_TILE = "Tile";

    /// <summary>
    /// UI elementlerinin layer'ı.
    /// </summary>
    public const string LAYER_UI = "UI";

    #endregion

    #region Tag Names (Tag İsimleri)
    // Unity tag'leri için string constant'lar

    public const string TAG_PLAYER = "Player";
    public const string TAG_TILE = "Tile";

    #endregion

    #region Scene Names (Sahne İsimleri)
    // SceneManager.LoadScene() için scene isimleri

    /// <summary>
    /// Ana menü sahnesi.
    /// </summary>
    public const string SCENE_MAIN_MENU = "MainMenu";

    /// <summary>
    /// Oyun sahnesi.
    /// </summary>
    public const string SCENE_GAME = "Game";

    /// <summary>
    /// Yükleme sahnesi (loading screen).
    /// </summary>
    public const string SCENE_LOADING = "Loading";

    #endregion
}

