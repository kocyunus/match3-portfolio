using System;
using System.Collections.Generic;

/// <summary>
/// Merkezi servis yöneticisi - Tüm servislere global erişim sağlar.
/// 
/// NE İŞE YARAR:
/// Bu sınıf, oyundaki tüm önemli sistemlere (ses, skor, seviye vb.) 
/// her yerden kolay erişim sağlar. GameObject.Find() gibi yavaş metodlar 
/// yerine çok hızlı dictionary araması kullanır.
/// 
/// UNITY'DEKİ KARŞILIĞI:
/// GameObject.Find("AudioManager").GetComponent<AudioManager>() 
/// yerine sadece:
/// ServiceLocator.Get<IAudioService>()
/// 
/// AVANTAJLARI:
/// ✅ Çok hızlı (dictionary lookup: O(1))
/// ✅ Type-safe (compile-time tip kontrolü)
/// ✅ Test edilebilir (mock servisler eklenebilir)
/// ✅ Null reference hatalarını önler
/// 
/// SEKTÖR STANDARDI:
/// Mobile game stüdyolarında en yaygın kullanılan pattern.
/// Dependency Injection'a göre daha performanslı ve Unity'de daha pratik.
/// 
/// DİKKAT:
/// Bu "static" bir sınıf çünkü oyunun her yerinden erişilebilir olmalı.
/// Ancak, test sırasında Reset() ile temizlenebilir.
/// </summary>
public static class ServiceLocator
{
    // Servislerin tutulduğu dictionary
    // Key: Servisin interface tipi (örn: typeof(IAudioService))
    // Value: Servisin gerçek instance'ı (örn: AudioService object'i)
    private static readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();

    /// <summary>
    /// Yeni bir servisi sisteme kaydeder.
    /// 
    /// KULLANIM:
    /// var audioService = new AudioService();
    /// ServiceLocator.Register<IAudioService>(audioService);
    /// 
    /// NEDEN INTERFACE KULLANIYORUZ:
    /// - Kodun test edilebilirliğini artırır
    /// - SOLID prensiplerinden "Dependency Inversion" ilkesini uygular
    /// - Gerçek implementasyonu gizler (encapsulation)
    /// 
    /// PARAMETRE:
    /// T: Interface tipi (IAudioService, IScoreService vb.)
    /// service: Gerçek servis implementasyonu
    /// </summary>
    public static void Register<T>(IService service) where T : IService
    {
        Type serviceType = typeof(T);

        // Servis zaten kayıtlı mı kontrol et
        if (services.ContainsKey(serviceType))
        {
            UnityEngine.Debug.LogWarning($"[ServiceLocator] {serviceType.Name} zaten kayıtlı! Üzerine yazılıyor.");
        }

        services[serviceType] = service;
        UnityEngine.Debug.Log($"[ServiceLocator] ✓ {serviceType.Name} başarıyla kaydedildi.");
    }

    /// <summary>
    /// Kayıtlı bir servisi getirir.
    /// 
    /// KULLANIM:
    /// ServiceLocator.Get<IAudioService>().PlaySound("match");
    /// var score = ServiceLocator.Get<IScoreService>().GetCurrentScore();
    /// 
    /// PERFORMANS:
    /// Dictionary lookup çok hızlıdır (O(1) - sabit zaman)
    /// GameObject.Find() ile karşılaştırıldığında 1000x daha hızlı!
    /// 
    /// GÜVENLİK:
    /// Eğer servis kayıtlı değilse açıklayıcı hata mesajı verir.
    /// Bu sayede hangi servisin eksik olduğunu hemen anlarız.
    /// </summary>
    public static T Get<T>() where T : IService
    {
        Type serviceType = typeof(T);

        // Servis kayıtlı mı kontrol et
        if (!services.ContainsKey(serviceType))
        {
            throw new Exception(
                $"[ServiceLocator] HATA: {serviceType.Name} servisi bulunamadı!\n" +
                $"Bootstrapper'da Register<{serviceType.Name}>() çağrısını yaptığınızdan emin olun."
            );
        }

        return (T)services[serviceType];
    }

    /// <summary>
    /// Bir servisin kayıtlı olup olmadığını kontrol eder.
    /// 
    /// KULLANIM:
    /// if (ServiceLocator.IsRegistered<IAudioService>())
    /// {
    ///     // Ses sistemi hazır, kullanabiliriz
    /// }
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Opsiyonel servisler için (analytics gibi)
    /// - Debug modunda kontroller için
    /// - Test senaryolarında
    /// </summary>
    public static bool IsRegistered<T>() where T : IService
    {
        return services.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Bir servisi sistemden kaldırır.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Genellikle kullanılmaz
    /// - Test senaryolarında servisleri temizlerken
    /// - Runtime'da servis değiştirirken (A/B test gibi)
    /// </summary>
    public static void Unregister<T>() where T : IService
    {
        Type serviceType = typeof(T);

        if (services.ContainsKey(serviceType))
        {
            services.Remove(serviceType);
            UnityEngine.Debug.Log($"[ServiceLocator] {serviceType.Name} kaydı silindi.");
        }
    }

    /// <summary>
    /// TÜM servisleri temizler.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Test senaryolarında (her test öncesi temiz başlamak için)
    /// - Oyun yeniden başlarken
    /// - Scene değişimlerinde
    /// 
    /// DİKKAT:
    /// Bu metod çağrıldıktan sonra tüm servisler tekrar 
    /// Register edilmelidir!
    /// </summary>
    public static void Reset()
    {
        // Önce tüm servislerin Cleanup metodunu çağır
        foreach (var service in services.Values)
        {
            service.Cleanup();
        }

        // Sonra dictionary'yi temizle
        services.Clear();
        UnityEngine.Debug.Log("[ServiceLocator] Tüm servisler temizlendi.");
    }

    /// <summary>
    /// Tüm kayıtlı servislerin Initialize() metodunu çağırır.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// Bootstrapper tarafından, tüm servisler kaydedildikten SONRA çağrılır.
    /// 
    /// NEDEN AYRI BİR METOD:
    /// Bazı servisler başlatılırken başka servislere ihtiyaç duyabilir.
    /// Önce hepsini Register, sonra hepsini Initialize etmek güvenlidir.
    /// 
    /// ÖRNEK:
    /// ScoreService, AudioService'e bağımlı olabilir (puan arttığında ses çalmak için).
    /// İkisi de önce Register edilir, sonra Initialize edilir.
    /// </summary>
    public static void InitializeAll()
    {
        UnityEngine.Debug.Log("[ServiceLocator] Tüm servisler başlatılıyor...");

        foreach (var service in services.Values)
        {
            service.Initialize();
        }

        UnityEngine.Debug.Log("[ServiceLocator] ✓ Tüm servisler başarıyla başlatıldı!");
    }

    /// <summary>
    /// Tüm servislerin Tick() metodunu çağırır.
    /// 
    /// NE ZAMAN KULLANILIR:
    /// Bootstrapper'ın Update() metodunda her frame çağrılır.
    /// 
    /// PERFORMANS NOTU:
    /// Sadece gerçekten her frame güncellenmesi gereken servisler
    /// Tick() içinde kod çalıştırmalıdır. Diğerleri boş bırakabilir.
    /// </summary>
    public static void TickAll()
    {
        foreach (var service in services.Values)
        {
            service.Tick();
        }
    }
}

