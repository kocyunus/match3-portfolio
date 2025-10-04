using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Oyun genelindeki tüm event'leri (olayları) yöneten merkezi sınıf.
/// 
/// NE İŞE YARAR:
/// Oyunda bir şey olduğunda (match bulundu, skor değişti vb.) bu bilgiyi
/// ilgilenen tüm sistemlere haber verir. Bu sayede sistemler birbirini
/// doğrudan tanımak zorunda kalmaz.
/// 
/// UNITY'DEKİ KARŞILIĞI:
/// UnityEvent'e benzer ama daha performanslı ve type-safe.
/// Normalde bir script diğerini GameObject.Find ile bulup metod çağırır,
/// Event System ile bu bağımlılık ortadan kalkar.
/// 
/// OBSERVER PATTERN:
/// Bu, "Observer Pattern" veya "Publish-Subscribe Pattern" olarak bilinir.
/// Bir yayıncı (Publisher) event fırlatır, birçok abone (Subscriber) dinler.
/// 
/// ÖRNEK SENARYO:
/// 1. BoardManager: "3 tile eşleşti!" diye event fırlatır
/// 2. ScoreService: Dinliyor, puanı artırır
/// 3. AudioService: Dinliyor, ses çalar
/// 4. UIManager: Dinliyor, animasyon gösterir
/// 
/// AVANTAJLARI:
/// ✅ Loose Coupling: Sistemler birbirinden bağımsız
/// ✅ Genişletilebilir: Yeni dinleyici eklemek çok kolay
/// ✅ Test edilebilir: Her sistem ayrı test edilebilir
/// ✅ Performanslı: C# delegate sistemi çok hızlı
/// 
/// SEKTÖR STANDARDI:
/// Tüm büyük Unity projelerinde kullanılır. Özellikle mobile oyunlarda
/// UI ve Game Logic'i ayırmak için kritik öneme sahiptir.
/// </summary>
public static class GameEvents
{
    #region Score Events (Skor Olayları)
    // Skor ile ilgili tüm event'ler

    /// <summary>
    /// Skor değiştiğinde tetiklenir.
    /// 
    /// PARAMETRE: int newScore - Yeni skor değeri
    /// 
    /// KİM DİNLER:
    /// - UI Score Text: Ekrandaki puanı günceller
    /// - Analytics: Puan kazanımını kaydeder
    /// - Achievement System: "10,000 puan kazan" gibi başarıları kontrol eder
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Match bulunduğunda
    /// - Combo bonusu kazanıldığında
    /// - Power-up kullanıldığında
    /// </summary>
    public static Action<int> OnScoreChanged;

    /// <summary>
    /// Combo tetiklendiğinde çağrılır.
    /// 
    /// PARAMETRE: int comboCount - Combo sayısı (2x, 3x, 4x, 5x)
    /// 
    /// KİM DİNLER:
    /// - UI Combo Display: "COMBO x3!" yazısını gösterir
    /// - AudioService: Combo sesini pitch ile çalar (yüksek combo = yüksek pitch)
    /// - VFX Manager: Özel efekt gösterir
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Ardışık match'ler bulunduğunda
    /// - Zincirleme reaksiyon (cascade) gerçekleştiğinde
    /// </summary>
    public static Action<int> OnComboTriggered;

    #endregion

    #region Match Events (Eşleşme Olayları)
    // Tile eşleşmeleri ile ilgili event'ler

    /// <summary>
    /// Tile'lar eşleştiğinde tetiklenir.
    /// 
    /// PARAMETRE: int tileCount - Eşleşen tile sayısı
    /// 
    /// KİM DİNLER:
    /// - ScoreService: Puan hesaplar ve ekler
    /// - AudioService: Match sesi çalar
    /// - VFXManager: Patlama efekti gösterir
    /// - BoardManager: Tile'ları yok eder
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - 3 veya daha fazla tile eşleştiğinde
    /// - Power-up aktive edildiğinde
    /// </summary>
    public static Action<int> OnMatchFound;

    /// <summary>
    /// Özel pattern (L, T, 4-match, 5-match) bulunduğunda tetiklenir.
    /// 
    /// PARAMETRE: string patternType - "L", "T", "Line", "Color" gibi
    /// 
    /// KİM DİNLER:
    /// - PowerUpFactory: Uygun power-up oluşturur
    /// - UIManager: "SPECIAL!" mesajı gösterir
    /// - AudioService: Özel ses efekti çalar
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - 4 tile eşleştiğinde (Line Bomb)
    /// - 5 tile eşleştiğinde (Color Bomb)
    /// - L veya T şeklinde eşleşme bulunduğunda (Area Bomb)
    /// </summary>
    public static Action<string> OnSpecialMatchFound;

    #endregion

    #region Game State Events (Oyun Durumu Olayları)
    // Oyun durumu değişiklikleri

    /// <summary>
    /// Oyun başladığında tetiklenir.
    /// 
    /// KİM DİNLER:
    /// - BoardManager: Tahtayı oluşturur
    /// - UIManager: HUD'ı gösterir
    /// - AudioService: Müziği başlatır
    /// - TimerService: Süreyi başlatır
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Level seçim ekranından oyuna geçildiğinde
    /// - "Play" butonuna basıldığında
    /// </summary>
    public static Action OnGameStarted;

    /// <summary>
    /// Oyun bittiğinde (kazanma veya kaybetme) tetiklenir.
    /// 
    /// PARAMETRE: bool isWin - Oyuncu kazandı mı?
    /// 
    /// KİM DİNLER:
    /// - UIManager: Game Over veya Victory ekranını gösterir
    /// - SaveService: Progress'i kaydeder
    /// - AudioService: Victory veya defeat müziği çalar
    /// - Analytics: Oyun sonucu verilerini gönderir
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Hamle sayısı bittiğinde (kaybetme)
    /// - Hedef skoruna ulaşıldığında (kazanma)
    /// - Süre dolduğunda (kaybetme)
    /// </summary>
    public static Action<bool> OnGameEnded;

    /// <summary>
    /// Oyun durakladığında tetiklenir.
    /// 
    /// PARAMETRE: bool isPaused - Durduruldu mu yoksa devam mı?
    /// 
    /// KİM DİNLER:
    /// - BoardManager: Input'u devre dışı bırakır
    /// - AudioService: Sesleri durdurur/devam ettirir
    /// - UIManager: Pause menüsünü gösterir/gizler
    /// - TimerService: Süreyi durdurur/devam ettirir
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Pause butonuna basıldığında
    /// - Oyun arka plana gönderildiğinde (OnApplicationPause)
    /// </summary>
    public static Action<bool> OnGamePaused;

    #endregion

    #region Level Events (Seviye Olayları)
    // Seviye yönetimi event'leri

    /// <summary>
    /// Yeni seviye yüklendiğinde tetiklenir.
    /// 
    /// PARAMETRE: int levelNumber - Seviye numarası
    /// 
    /// KİM DİNLER:
    /// - BoardManager: Seviye ayarlarını alır
    /// - UIManager: Seviye bilgilerini gösterir
    /// - AudioService: Seviyeye özel müziği çalar
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Level seçildiğinde
    /// - Bir sonraki seviyeye geçildiğinde
    /// </summary>
    public static Action<int> OnLevelLoaded;

    /// <summary>
    /// Seviye tamamlandığında tetiklenir.
    /// 
    /// PARAMETRE: int starCount - Kazanılan yıldız sayısı (1-3)
    /// 
    /// KİM DİNLER:
    /// - UIManager: Victory ekranını yıldızlarla gösterir
    /// - SaveService: Seviye ilerlemesini kaydeder
    /// - LevelManager: Bir sonraki seviyeyi açar
    /// - Analytics: Seviye tamamlanma verilerini kaydeder
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Seviye hedefine ulaşıldığında
    /// - Final skoruna göre yıldız hesaplandığında
    /// </summary>
    public static Action<int> OnLevelCompleted;

    #endregion

    #region Move Events (Hamle Olayları)
    // Oyuncu hamleleri ile ilgili event'ler

    /// <summary>
    /// Geçerli bir hamle yapıldığında tetiklenir.
    /// 
    /// KİM DİNLER:
    /// - MoveCounter: Kalan hamle sayısını azaltır
    /// - UIManager: Hamle sayısını günceller
    /// - Analytics: Hamle istatistiklerini kaydeder
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Tile swap işlemi başarılı olduğunda
    /// - Power-up kullanıldığında (bazen hamle harcamaz)
    /// </summary>
    public static Action OnMoveExecuted;

    /// <summary>
    /// Geçersiz hamle denendiğinde tetiklenir.
    /// 
    /// KİM DİNLER:
    /// - AudioService: "Hata" sesi çalar
    /// - UIManager: Tile'ları geri kaydırır veya titreşim animasyonu gösterir
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Eşleşme oluşturmayan swap denendiğinde
    /// - Swap edilemeyen tile'lara tıklandığında
    /// </summary>
    public static Action OnInvalidMove;

    #endregion

    #region Power-Up Events (Güç Olayları)
    // Power-up sistemleri için event'ler

    /// <summary>
    /// Power-up oluşturulduğunda tetiklenir.
    /// 
    /// PARAMETRE: string powerUpType - "Bomb", "Rocket", "ColorBomb" vb.
    /// 
    /// KİM DİNLER:
    /// - UIManager: Animasyon ve mesaj gösterir
    /// - AudioService: Özel ses çalar
    /// - TutorialManager: İlk kez oluşturuluyorsa öğretici gösterir
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - 4-match oluştuğunda
    /// - 5-match oluştuğunda
    /// - L/T-shape match bulunduğunda
    /// </summary>
    public static Action<string> OnPowerUpCreated;

    /// <summary>
    /// Power-up kullanıldığında tetiklenir.
    /// 
    /// PARAMETRE: string powerUpType - Kullanılan power-up tipi
    /// 
    /// KİM DİNLER:
    /// - BoardManager: Power-up etkisini uygular
    /// - ScoreService: Bonus puan ekler
    /// - VFXManager: Büyük patlama efekti gösterir
    /// - AudioService: Etkileyici ses çalar
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Power-up tile'ına tıklandığında
    /// - İki power-up birleştirildiğinde (combo)
    /// </summary>
    public static Action<string> OnPowerUpActivated;

    #endregion

    #region UI Events (Kullanıcı Arayüzü Olayları)
    // UI etkileşimleri için event'ler

    /// <summary>
    /// Butona basıldığında tetiklenir.
    /// 
    /// PARAMETRE: string buttonName - Buton ismi
    /// 
    /// KİM DİNLER:
    /// - AudioService: Tıklama sesi çalar
    /// - Analytics: Buton tıklama istatistikleri
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Herhangi bir UI butonuna basıldığında
    /// </summary>
    public static Action<string> OnButtonClicked;

    #endregion

    #region Debug Events (Test ve Geliştirme)
    // Geliştirme aşamasında kullanılan debug event'leri

    /// <summary>
    /// Test amaçlı basit bir event.
    /// 
    /// PARAMETRE: string message - Test mesajı
    /// 
    /// KİM DİNLER:
    /// - DebugConsole: Mesajı ekrana yazdırır
    /// 
    /// NE ZAMAN TETİKLENİR:
    /// - Test scriptleri tarafından
    /// - Development build'lerde debug için
    /// </summary>
    public static Action<string> OnDebugMessage;

    #endregion

    #region Event Helper Methods
    // Event'leri güvenli bir şekilde tetiklemek için yardımcı metodlar

    /// <summary>
    /// Tüm event'leri temizler.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Scene değişimlerinde memory leak önlemek için
    /// - Test senaryolarında temiz başlamak için
    /// - Oyun yeniden başlatılırken
    /// 
    /// ÖNEMLİ:
    /// Event'lerin null olmaması garbage collection'ı engelleyebilir.
    /// Scene değişimlerinde mutlaka temizlenmelidir!
    /// </summary>
    public static void ClearAllEvents()
    {
        // Score Events
        OnScoreChanged = null;
        OnComboTriggered = null;

        // Match Events
        OnMatchFound = null;
        OnSpecialMatchFound = null;

        // Game State Events
        OnGameStarted = null;
        OnGameEnded = null;
        OnGamePaused = null;

        // Level Events
        OnLevelLoaded = null;
        OnLevelCompleted = null;

        // Move Events
        OnMoveExecuted = null;
        OnInvalidMove = null;

        // Power-Up Events
        OnPowerUpCreated = null;
        OnPowerUpActivated = null;

        // UI Events
        OnButtonClicked = null;

        // Debug Events
        OnDebugMessage = null;

        Debug.Log("[GameEvents] ✓ Tüm event'ler temizlendi.");
    }

    /// <summary>
    /// Tüm aktif event'lerin ve dinleyici sayılarının listesini verir.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Debug amaçlı, hangi event'lerin aktif olduğunu görmek için
    /// - Memory leak tespiti için
    /// 
    /// KULLANIM:
    /// GameEvents.PrintActiveEvents();
    /// </summary>
    public static void PrintActiveEvents()
    {
        Debug.Log("===== AKTİF EVENT'LER =====");

        LogEventInfo("OnScoreChanged", OnScoreChanged);
        LogEventInfo("OnComboTriggered", OnComboTriggered);
        LogEventInfo("OnMatchFound", OnMatchFound);
        LogEventInfo("OnSpecialMatchFound", OnSpecialMatchFound);
        LogEventInfo("OnGameStarted", OnGameStarted);
        LogEventInfo("OnGameEnded", OnGameEnded);
        LogEventInfo("OnGamePaused", OnGamePaused);
        LogEventInfo("OnLevelLoaded", OnLevelLoaded);
        LogEventInfo("OnLevelCompleted", OnLevelCompleted);
        LogEventInfo("OnMoveExecuted", OnMoveExecuted);
        LogEventInfo("OnInvalidMove", OnInvalidMove);
        LogEventInfo("OnPowerUpCreated", OnPowerUpCreated);
        LogEventInfo("OnPowerUpActivated", OnPowerUpActivated);
        LogEventInfo("OnButtonClicked", OnButtonClicked);
        LogEventInfo("OnDebugMessage", OnDebugMessage);

        Debug.Log("===========================");
    }

    /// <summary>
    /// Tek bir event'in bilgilerini loglar.
    /// </summary>
    private static void LogEventInfo(string eventName, Delegate eventDelegate)
    {
        if (eventDelegate == null)
        {
            Debug.Log($"  {eventName}: Dinleyici yok");
        }
        else
        {
            int listenerCount = eventDelegate.GetInvocationList().Length;
            Debug.Log($"  {eventName}: {listenerCount} dinleyici");
        }
    }

    #endregion
}

/* 
 * ===== EVENT KULLANIM KILAVUZU =====
 * 
 * --- EVENT'E ABONE OLMA (Subscribe) ---
 * 
 * void OnEnable()
 * {
 *     // Event'i dinlemeye başla
 *     GameEvents.OnScoreChanged += HandleScoreChanged;
 * }
 * 
 * void OnDisable()
 * {
 *     // Event dinlemeyi durdur (MEMORY LEAK ÖNLEME!)
 *     GameEvents.OnScoreChanged -= HandleScoreChanged;
 * }
 * 
 * private void HandleScoreChanged(int newScore)
 * {
 *     Debug.Log($"Yeni skor: {newScore}");
 *     scoreText.text = newScore.ToString();
 * }
 * 
 * --- EVENT TETİKLEME (Invoke) ---
 * 
 * // Güvenli yöntem (null check ile)
 * GameEvents.OnScoreChanged?.Invoke(1000);
 * 
 * // Tehlikeli yöntem (null check olmadan - KULLANMA!)
 * // GameEvents.OnScoreChanged(1000); // NullReferenceException riski!
 * 
 * --- ÖNEMLİ KURALLAR ---
 * 
 * 1. MUTLAKA OnDisable'da unsubscribe et!
 *    Yoksa memory leak olur ve performans düşer.
 * 
 * 2. Event'i tetiklerken MUTLAKA ?. kullan!
 *    OnScoreChanged?.Invoke(...) şeklinde.
 * 
 * 3. Event handler metodları private olmalı!
 *    Dışarıdan çağrılmamalı, sadece event tetiklemeli.
 * 
 * 4. Scene değişimlerinde ClearAllEvents() çağır!
 *    Aksi halde eski scene'deki listener'lar kalır.
 * 
 * --- UNITY'DE KARŞILAŞTIĞI SORUNLAR VE ÇÖZÜM ---
 * 
 * SORUN: UnityEvent Inspector'da görünür ama yavaş
 * ÇÖZÜM: C# Action daha hızlı ama Inspector'da görünmez
 * 
 * SORUN: Event'ler bazen null oluyor
 * ÇÖZÜM: Tetiklerken ?. operatörü kullan
 * 
 * SORUN: Memory leak oluşuyor
 * ÇÖZÜM: OnDisable'da -= ile unsubscribe et
 * 
 * --- SEKTÖR BİLGİSİ ---
 * 
 * Bu pattern şu oyunlarda kullanılır:
 * - Candy Crush: Match ve UI olayları
 * - Clash Royale: Kart oynatma ve animasyonlar
 * - Among Us: Oyuncu etkileşimleri
 * 
 * Neden bu kadar yaygın?
 * ✅ Sistemleri birbirinden ayırır
 * ✅ Yeni özellik eklemek çok kolay
 * ✅ Test edilebilir kod
 * ✅ Performanslı (C# delegate hızlıdır)
 */

