using UnityEngine;

/// <summary>
/// Oyunun başlangıç noktası - Tüm sistemleri başlatır ve yönetir.
/// 
/// NE İŞE YARAR:
/// Unity'de oyun başladığında birçok sistem (ses, skor, input vb.) 
/// başlatılmalıdır. Bootstrapper, bunların DOĞRU SIRADA ve GÜVENLİ
/// bir şekilde başlatılmasını sağlar.
/// 
/// UNITY'DEKİ KARŞILIĞI:
/// Unity'nin Awake/Start sistemini daha kontrollü hale getirir.
/// Normalde her script kendi Awake'ini çalıştırır ve sıralama 
/// garanti değildir. Bootstrapper ile sıralamayı biz kontrol ederiz.
/// 
/// NEDEN ÖNEMLİ:
/// ✅ Execution Order sorunlarını önler (Unity'nin klasik sorunu)
/// ✅ Dependency problemlerini çözer (A servisi B'ye bağımlıysa)
/// ✅ Oyun tüm platformlarda aynı şekilde başlar
/// ✅ Test ortamında kolayca mock edilebilir
/// 
/// NASIL KULLANILIR:
/// 1. Boş bir GameObject oluştur
/// 2. Bu scripti ekle
/// 3. DontDestroyOnLoad ile scene geçişlerinde kaybolmaz
/// 
/// SEKTÖR STANDARDI:
/// Tüm büyük Unity projelerinde (özellikle mobile) kullanılır.
/// "Game Manager" veya "App Controller" olarak da bilinir.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    #region Singleton Pattern
    // Singleton: Oyunun her yerinden erişilebilen tek bir instance
    // Unity'de DontDestroyOnLoad ile birleştirilir

    /// <summary>
    /// Bootstrapper'ın tek instance'ı.
    /// Diğer scriptler Bootstrapper.Instance ile erişebilir.
    /// </summary>
    public static Bootstrapper Instance { get; private set; }

    #endregion

    #region Unity Lifecycle
    // Unity'nin yaşam döngüsü metodları

    /// <summary>
    /// Awake: Unity'de ilk çalışan metod.
    /// Burada:
    /// 1. Singleton kontrolü yaparız
    /// 2. Scene geçişlerinde yok olmamasını sağlarız
    /// 3. Servisleri kaydederiz (henüz başlatmayız)
    /// </summary>
    private void Awake()
    {
        // Singleton kontrolü
        if (Instance != null && Instance != this)
        {
            // Zaten bir Bootstrapper varsa, yeni oluşanı yok et
            Debug.LogWarning("[Bootstrapper] Birden fazla Bootstrapper bulundu! Yeni olanı yok ediliyor.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Scene değişikliklerinde bu GameObject'i yok etme
        // Bu sayede servisler tüm oyun boyunca hayatta kalır
        DontDestroyOnLoad(gameObject);

        Debug.Log("[Bootstrapper] ===== OYUN BAŞLATILIYOR =====");

        // Servisleri kaydet
        RegisterServices();
    }

    /// <summary>
    /// Start: Awake'den sonra çalışır, tüm Awake'ler bittikten sonra.
    /// Burada servisleri başlatırız (Initialize).
    /// 
    /// NEDEN AYRI:
    /// Bazı servisler başka servislere bağımlıdır.
    /// Önce hepsini kaydet (Register), sonra hepsini başlat (Initialize).
    /// </summary>
    private void Start()
    {
        // Tüm servisleri başlat
        ServiceLocator.InitializeAll();

        Debug.Log("[Bootstrapper] ===== OYUN BAŞARIYLA BAŞLATILDI =====");

        // Oyun başlangıç durumuna geç
        // TODO: Game State Machine eklenince aktif edilecek
        // GameStateManager.ChangeState(new MainMenuState());
    }

    /// <summary>
    /// Update: Her frame çağrılır.
    /// Servislerin Tick() metodlarını çağırır.
    /// 
    /// PERFORMANS NOTU:
    /// Update her frame çalıştığı için dikkatli olunmalı!
    /// Sadece gerekli servisler Tick() içinde işlem yapmalı.
    /// </summary>
    private void Update()
    {
        // Tüm servislerin Tick metodunu çağır
        ServiceLocator.TickAll();
    }

    /// <summary>
    /// OnDestroy: GameObject yok edildiğinde çağrılır.
    /// Servisleri temizler ve kaynakları serbest bırakır.
    /// 
    /// NEDEN ÖNEMLİ:
    /// Memory leak'leri (bellek sızıntısı) önlemek için kritik!
    /// Özellikle mobile cihazlarda bellek yönetimi çok önemli.
    /// </summary>
    private void OnDestroy()
    {
        // Eğer bu instance siliniyorsa, servisleri temizle
        if (Instance == this)
        {
            Debug.Log("[Bootstrapper] Servisler temizleniyor...");
            ServiceLocator.Reset();
        }
    }

    /// <summary>
    /// OnApplicationQuit: Uygulama kapatılırken çağrılır.
    /// Son kaydetme işlemleri için ideal yer.
    /// </summary>
    private void OnApplicationQuit()
    {
        Debug.Log("[Bootstrapper] Oyun kapatılıyor...");
        
        // TODO: SaveService eklenince aktif edilecek
        // ServiceLocator.Get<ISaveService>().SaveAll();
    }

    #endregion

    #region Service Registration
    // Servislerin kaydedildiği yer

    /// <summary>
    /// Tüm servisleri ServiceLocator'a kaydeder.
    /// 
    /// SERVİS EKLEME SIRASI ÖNEMLİ!
    /// Eğer ServisA, ServisB'ye bağımlıysa, ServisB önce eklenmelidir.
    /// 
    /// ÖRNEK BAĞIMLILIK:
    /// - ScoreService, AudioService'e bağımlı (puan arttığında ses çalar)
    /// - Önce AudioService, sonra ScoreService kaydedilmeli
    /// 
    /// TODO:
    /// Şu an hiç servis yok, ilerleyen aşamalarda eklenecek.
    /// Her yeni servis eklendiğinde buraya kayıt kodu eklenecek.
    /// </summary>
    private void RegisterServices()
    {
        Debug.Log("[Bootstrapper] Servisler kaydediliyor...");

        // ========== PHASE 1: Core Services (Temel Servisler) ==========
        // Bu servisler hiçbir şeye bağımlı değil, ilk başlatılabilirler
        
        // TODO: AudioService eklenince
        // ServiceLocator.Register<IAudioService>(new AudioService());

        // TODO: SaveService eklenince
        // ServiceLocator.Register<ISaveService>(new SaveService());

        // ========== PHASE 2: Game Logic Services (Oyun Mantığı Servisleri) ==========
        // Bu servisler Phase 1 servislerine bağımlı olabilir
        
        // TODO: ScoreService eklenince
        // ServiceLocator.Register<IScoreService>(new ScoreService());

        // TODO: LevelService eklenince
        // ServiceLocator.Register<ILevelService>(new LevelService());

        // ========== PHASE 3: Input Services (Giriş Servisleri) ==========
        // Kullanıcı girişi ile ilgili servisler
        
        // TODO: InputService eklenince
        // ServiceLocator.Register<IInputService>(new InputService());

        // ========== PHASE 4: Analytics & Optional Services (Opsiyonel Servisler) ==========
        // Oyun çalışması için zorunlu olmayan servisler
        
        // TODO: AnalyticsService eklenince
        // ServiceLocator.Register<IAnalyticsService>(new AnalyticsService());

        Debug.Log("[Bootstrapper] ✓ Tüm servisler kaydedildi.");
    }

    #endregion

    #region Public Methods
    // Dışarıdan erişilebilir yardımcı metodlar

    /// <summary>
    /// Oyunu yeniden başlatır.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - "Restart" butonu
    /// - Game Over'dan sonra tekrar oyna
    /// - Hata durumunda oyunu sıfırlama
    /// 
    /// DİKKAT:
    /// Bu metod tüm servisleri sıfırlar!
    /// Kaydetmek istediğiniz verileri önce kaydedin.
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("[Bootstrapper] Oyun yeniden başlatılıyor...");

        // Servisleri temizle
        ServiceLocator.Reset();

        // Servisleri tekrar kaydet ve başlat
        RegisterServices();
        ServiceLocator.InitializeAll();

        Debug.Log("[Bootstrapper] ✓ Oyun yeniden başlatıldı!");
    }

    #endregion

    #region Editor Helper Methods
    // Unity Editor'de test etmek için yardımcı metodlar

#if UNITY_EDITOR
    /// <summary>
    /// Editor'de test için: Servislerin durumunu konsola yazdırır.
    /// 
    /// KULLANIM:
    /// Unity Editor'de bu scripti inspector'da seçin,
    /// sağ üst menüden bu metodu çağırabilirsiniz.
    /// </summary>
    [ContextMenu("Debug: Servis Durumlarını Göster")]
    private void DebugPrintServiceStatus()
    {
        Debug.Log("===== SERVİS DURUMLARI =====");
        
        // TODO: Her servis eklenince buraya kontrol eklenecek
        // Debug.Log($"AudioService: {ServiceLocator.IsRegistered<IAudioService>()}");
        // Debug.Log($"ScoreService: {ServiceLocator.IsRegistered<IScoreService>()}");
        
        Debug.Log("Henüz kayıtlı servis yok.");
    }
#endif

    #endregion
}

/* 
 * ===== KULLANIM KILAVUZU =====
 * 
 * 1. Unity'de boş bir GameObject oluştur
 * 2. Adını "Bootstrapper" koy
 * 3. Bu scripti ekle
 * 4. Play'e bas - otomatik çalışacak
 * 
 * ===== YENİ SERVİS EKLEME =====
 * 
 * 1. IService interface'ini implement eden bir servis yaz:
 *    public class MyService : IService { ... }
 * 
 * 2. Bir interface oluştur:
 *    public interface IMyService : IService { ... }
 * 
 * 3. RegisterServices() metoduna ekle:
 *    ServiceLocator.Register<IMyService>(new MyService());
 * 
 * 4. Oyunun herhangi bir yerinden kullan:
 *    ServiceLocator.Get<IMyService>().DoSomething();
 * 
 * Neden bu kadar yaygın?
 * ✅ Hızlı development
 * ✅ Kolay test edilebilir
 * ✅ Performanslı (mobile için kritik)
 * ✅ Unity ile iyi entegre
 */

