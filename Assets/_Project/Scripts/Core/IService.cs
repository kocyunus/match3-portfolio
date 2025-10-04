/// <summary>
/// Tüm servis sınıflarının implement etmesi gereken temel interface.
/// 
/// NE İŞE YARAR:
/// - Servis yaşam döngüsünü yönetir (başlatma, güncelleme, kapatma)
/// - ServiceLocator ile kullanılmak üzere standart bir yapı sağlar
/// 
/// NEDEN ÖNEMLİ:
/// - SOLID prensiplerinden "Interface Segregation" ilkesini uygular
/// - Test yazarken mock servis oluşturmayı kolaylaştırır
/// - Tüm servislerin aynı davranışı garanti eder
/// 
/// SEKTÖR STANDARDI:
/// - Büyük oyun stüdyolarında (Supercell, King) kullanılan pattern
/// </summary>
public interface IService
{
    /// <summary>
    /// Servis ilk kez başlatılırken çağrılır.
    /// Unity'nin Awake() metoduna benzer ama daha kontrollü.
    /// 
    /// ÖRNEK KULLANIM:
    /// - AudioService: Ses havuzunu oluştur
    /// - ScoreService: Skorları sıfırla
    /// - SaveService: Kayıtlı verileri yükle
    /// </summary>
    void Initialize();

    /// <summary>
    /// Her frame'de çağrılır (opsiyonel).
    /// Unity'nin Update() metoduna benzer.
    /// 
    /// NOT: Sadece gerçekten her frame güncellenmesi gereken servisler
    /// için kullanılmalı (performans için önemli!)
    /// 
    /// ÖRNEK KULLANIM:
    /// - InputService: Her frame input kontrolü
    /// - ComboService: Combo süresini azalt
    /// </summary>
    void Tick();

    /// <summary>
    /// Servis kapatılırken çağrılır.
    /// Unity'nin OnDestroy() metoduna benzer.
    /// 
    /// ÖRNEK KULLANIM:
    /// - SaveService: Verileri kaydet
    /// - AudioService: Sesleri durdur
    /// - NetworkService: Bağlantıyı kapat
    /// </summary>
    void Cleanup();
}

