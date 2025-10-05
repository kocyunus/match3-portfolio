/// <summary>
/// Servis interface - Tüm servisler bunu implement eder
/// Yaşam döngüsü: Initialize → Tick (her frame) → Cleanup
/// </summary>
public interface IService
{
    void Initialize();  // Başlatma
    void Tick();        // Her frame (opsiyonel)
    void Cleanup();     // Temizlik
}
