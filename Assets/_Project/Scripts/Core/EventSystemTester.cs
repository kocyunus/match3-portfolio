using UnityEngine;

/// <summary>
/// Event System'in çalıştığını test etmek için basit bir test scripti.
/// 
/// NE İŞE YARAR:
/// Bu script, Event System'in doğru çalıştığını test eder.
/// Hem event fırlatır hem de dinler, böylece sistem kontrolü yaparız.
/// 
/// NASIL KULLANILIR:
/// 1. Unity'de boş bir GameObject oluştur
/// 2. Bu scripti ekle
/// 3. Play'e bas
/// 4. Console'da mesajları gör
/// 5. Inspector'da butonlara bas
/// 
/// ÖNEMLİ:
/// Bu script sadece test amaçlıdır. Gerçek oyunda kullanılmaz.
/// Test tamamlandıktan sonra GameObject'i silebilirsiniz.
/// </summary>
public class EventSystemTester : MonoBehaviour
{
    #region Unity Lifecycle

    /// <summary>
    /// Script aktif olduğunda event'lere abone ol.
    /// 
    /// NOT: OnEnable kullanıyoruz çünkü:
    /// - Awake/Start'tan önce çalışır
    /// - GameObject aktif/pasif yapıldığında otomatik subscribe/unsubscribe olur
    /// - Best practice budur!
    /// </summary>
    private void OnEnable()
    {
        Debug.Log("[EventSystemTester] Event'lere abone olunuyor...");

        // Event'lere abone ol (Subscribe)
        GameEvents.OnScoreChanged += HandleScoreChanged;
        GameEvents.OnMatchFound += HandleMatchFound;
        GameEvents.OnComboTriggered += HandleComboTriggered;
        GameEvents.OnGameStarted += HandleGameStarted;
        GameEvents.OnDebugMessage += HandleDebugMessage;

        Debug.Log("[EventSystemTester] ✓ Tüm event'lere abone olundu!");
    }

    /// <summary>
    /// Script pasif olduğunda event'lerden çık.
    /// 
    /// ÇOK ÖNEMLİ:
    /// Eğer bu metodu yazmazsanız MEMORY LEAK oluşur!
    /// Script yok olsa bile event'ler onu referans tutar.
    /// 
    /// KURAL:
    /// OnEnable'da += yaptıysan, OnDisable'da -= yapmalısın!
    /// </summary>
    private void OnDisable()
    {
        Debug.Log("[EventSystemTester] Event'lerden çıkılıyor...");

        // Event'lerden çık (Unsubscribe)
        GameEvents.OnScoreChanged -= HandleScoreChanged;
        GameEvents.OnMatchFound -= HandleMatchFound;
        GameEvents.OnComboTriggered -= HandleComboTriggered;
        GameEvents.OnGameStarted -= HandleGameStarted;
        GameEvents.OnDebugMessage -= HandleDebugMessage;

        Debug.Log("[EventSystemTester] ✓ Tüm event'lerden çıkıldı!");
    }

    /// <summary>
    /// Oyun başladığında otomatik test senaryosu çalıştır.
    /// </summary>
    private void Start()
    {
        Debug.Log("[EventSystemTester] ===== EVENT SYSTEM TESTİ BAŞLIYOR =====");
        
        // 1 saniye bekle, sonra testleri başlat
        Invoke(nameof(RunAutomaticTests), 1f);
    }

    #endregion

    #region Event Handlers (Event Dinleyicileri)
    // Bu metodlar event tetiklendiğinde otomatik çağrılır

    /// <summary>
    /// Skor değiştiğinde çağrılır.
    /// </summary>
    private void HandleScoreChanged(int newScore)
    {
        Debug.Log($"<color=green>[EventSystemTester] ✓ Skor değişti! Yeni skor: {newScore}</color>");
    }

    /// <summary>
    /// Match bulunduğunda çağrılır.
    /// </summary>
    private void HandleMatchFound(int tileCount)
    {
        Debug.Log($"<color=yellow>[EventSystemTester] ✓ Match bulundu! {tileCount} tile eşleşti!</color>");
    }

    /// <summary>
    /// Combo tetiklendiğinde çağrılır.
    /// </summary>
    private void HandleComboTriggered(int comboCount)
    {
        Debug.Log($"<color=orange>[EventSystemTester] ✓ COMBO x{comboCount}!</color>");
    }

    /// <summary>
    /// Oyun başladığında çağrılır.
    /// </summary>
    private void HandleGameStarted()
    {
        Debug.Log($"<color=cyan>[EventSystemTester] ✓ Oyun başladı!</color>");
    }

    /// <summary>
    /// Debug mesajı geldiğinde çağrılır.
    /// </summary>
    private void HandleDebugMessage(string message)
    {
        Debug.Log($"<color=magenta>[EventSystemTester] ✓ Debug mesajı: {message}</color>");
    }

    #endregion

    #region Test Methods (Test Metodları)
    // Event'leri manuel ve otomatik test etmek için metodlar

    /// <summary>
    /// Otomatik test senaryosu.
    /// Tüm event'leri sırayla tetikleyerek sistemin çalıştığını kontrol eder.
    /// </summary>
    private void RunAutomaticTests()
    {
        Debug.Log("[EventSystemTester] ----- Otomatik testler başlıyor -----");

        // Test 1: Debug mesajı
        Debug.Log("[EventSystemTester] Test 1: Debug mesajı gönderiliyor...");
        GameEvents.OnDebugMessage?.Invoke("Merhaba Event System!");

        // Test 2: Oyun başlama
        Debug.Log("[EventSystemTester] Test 2: Oyun başlatılıyor...");
        GameEvents.OnGameStarted?.Invoke();

        // Test 3: Match bulma
        Debug.Log("[EventSystemTester] Test 3: 3 tile eşleşmesi simüle ediliyor...");
        GameEvents.OnMatchFound?.Invoke(3);

        // Test 4: Skor değişimi
        Debug.Log("[EventSystemTester] Test 4: Skor artırılıyor...");
        GameEvents.OnScoreChanged?.Invoke(100);

        // Test 5: Combo
        Debug.Log("[EventSystemTester] Test 5: Combo tetikleniyor...");
        GameEvents.OnComboTriggered?.Invoke(2);

        // Test 6: Daha fazla skor
        Debug.Log("[EventSystemTester] Test 6: Daha fazla skor ekleniyor...");
        GameEvents.OnScoreChanged?.Invoke(250);

        // Test 7: Daha yüksek combo
        Debug.Log("[EventSystemTester] Test 7: Yüksek combo tetikleniyor...");
        GameEvents.OnComboTriggered?.Invoke(5);

        Debug.Log("[EventSystemTester] ===== TÜM TESTLER TAMAMLANDI =====");
        Debug.Log("[EventSystemTester] Yukarıda renkli mesajlar görüyorsanız Event System ÇALIŞIYOR! ✓");
    }

    /// <summary>
    /// Event bilgilerini yazdır.
    /// Inspector'dan bu metodu çağırabilirsiniz.
    /// </summary>
    [ContextMenu("Aktif Event'leri Göster")]
    private void ShowActiveEvents()
    {
        GameEvents.PrintActiveEvents();
    }

    /// <summary>
    /// Tüm event'leri temizle.
    /// Inspector'dan bu metodu çağırabilirsiniz.
    /// </summary>
    [ContextMenu("Tüm Event'leri Temizle")]
    private void ClearAllEvents()
    {
        GameEvents.ClearAllEvents();
    }

    #endregion

    #region Inspector Test Buttons
    // Unity Inspector'da görünen test butonları

    /// <summary>
    /// Test: Skor değiştir.
    /// Inspector'da buton olarak görünür.
    /// </summary>
    [ContextMenu("Test: Skor Değiştir (1000)")]
    private void TestScoreChange()
    {
        Debug.Log("[EventSystemTester] Manuel test: Skor değiştiriliyor...");
        GameEvents.OnScoreChanged?.Invoke(1000);
    }

    /// <summary>
    /// Test: Match bul.
    /// Inspector'da buton olarak görünür.
    /// </summary>
    [ContextMenu("Test: Match Bul (5 tile)")]
    private void TestMatchFound()
    {
        Debug.Log("[EventSystemTester] Manuel test: Match simüle ediliyor...");
        GameEvents.OnMatchFound?.Invoke(5);
    }

    /// <summary>
    /// Test: Combo tetikle.
    /// Inspector'da buton olarak görünür.
    /// </summary>
    [ContextMenu("Test: Combo Tetikle (x3)")]
    private void TestCombo()
    {
        Debug.Log("[EventSystemTester] Manuel test: Combo tetikleniyor...");
        GameEvents.OnComboTriggered?.Invoke(3);
    }

    /// <summary>
    /// Test: Oyunu başlat.
    /// Inspector'da buton olarak görünür.
    /// </summary>
    [ContextMenu("Test: Oyunu Başlat")]
    private void TestGameStart()
    {
        Debug.Log("[EventSystemTester] Manuel test: Oyun başlatılıyor...");
        GameEvents.OnGameStarted?.Invoke();
    }

    #endregion
}

/* 
 * ===== UNITY'DE NASIL KULLANILIR =====
 * 
 * ADIM 1: GameObject Oluştur
 * - Hierarchy'de sağ tık → Create Empty
 * - Adını "EventSystemTester" koy
 * 
 * ADIM 2: Script Ekle
 * - EventSystemTester GameObject'ini seç
 * - Inspector'da Add Component
 * - "EventSystemTester" scriptini ara ve ekle
 * 
 * ADIM 3: Otomatik Test
 * - Play butonuna bas
 * - Console'u aç (Ctrl+Shift+C)
 * - Renkli mesajlar görmelisin!
 * 
 * ADIM 4: Manuel Test
 * - Play modundayken EventSystemTester GameObject'ini seç
 * - Inspector'da, sağ üstteki ⋮ (3 nokta) menüsüne tıkla
 * - Test butonlarını kullan:
 *   - "Test: Skor Değiştir"
 *   - "Test: Match Bul"
 *   - "Test: Combo Tetikle"
 * - Her butona bastığında Console'da mesaj görmelisin
 * 
 * ===== BEKLENEN SONUÇ =====
 * 
 * Console'da şöyle mesajlar görmelisin:
 * 
 * [EventSystemTester] Event'lere abone olunuyor...
 * [EventSystemTester] ✓ Tüm event'lere abone olundu!
 * [EventSystemTester] ===== EVENT SYSTEM TESTİ BAŞLIYOR =====
 * [EventSystemTester] ✓ Debug mesajı: Merhaba Event System!
 * [EventSystemTester] ✓ Oyun başladı!
 * [EventSystemTester] ✓ Match bulundu! 3 tile eşleşti!
 * [EventSystemTester] ✓ Skor değişti! Yeni skor: 100
 * [EventSystemTester] ✓ COMBO x2!
 * [EventSystemTester] ===== TÜM TESTLER TAMAMLANDI =====
 * 
 * Eğer bu mesajları görüyorsan, Event System MÜKEMMEL ÇALIŞIYOR! ✓
 * 
 * ===== SORUN GİDERME =====
 * 
 * SORUN: Hiçbir mesaj görünmüyor
 * ÇÖZÜM: Console'u aç (Window → General → Console)
 * 
 * SORUN: Sadece bazı mesajlar görünüyor
 * ÇÖZÜM: Console filtrelerini kontrol et (sağ üstte)
 * 
 * SORUN: "NullReferenceException" hatası
 * ÇÖZÜM: GameEvents.cs dosyasının doğru yerde olduğunu kontrol et
 * 
 * SORUN: Script component olarak eklenmiyor
 * ÇÖZÜM: Unity'yi yeniden başlat (bazen script compile olmaz)
 */

