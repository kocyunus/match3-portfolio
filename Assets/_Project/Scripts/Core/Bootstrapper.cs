using UnityEngine;
using DG.Tweening;

/// <summary>
/// Oyun başlangıç noktası - Servisleri başlatır
/// DontDestroyOnLoad ile scene'ler arası hayatta kalır
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    public static Bootstrapper Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[Bootstrapper] Oyun başlatılıyor...");
        
        // DOTween başlat
        InitializeDOTween();
        
        RegisterServices();
    }
    
    private void InitializeDOTween()
    {
        // DOTween ayarları
        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
        DOTween.defaultEaseType = Ease.OutCubic;
        DOTween.defaultAutoPlay = AutoPlay.All;
        DOTween.defaultAutoKill = true;
        
        Debug.Log("[DOTween] Başlatıldı");
    }

    private void Start()
    {
        ServiceLocator.InitializeAll();
        Debug.Log("[Bootstrapper] Oyun başarıyla başlatıldı!");
    }

    private void Update()
    {
        ServiceLocator.TickAll();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            ServiceLocator.Reset();
        }
    }

    private void RegisterServices()
    {
        Debug.Log("[Bootstrapper] Servisler kaydediliyor...");
        
        // DOTween kullanıyoruz, AnimationService yok artık
        
        Debug.Log("[Bootstrapper] Servisler kaydedildi.");
    }

    public void RestartGame()
    {
        ServiceLocator.Reset();
        RegisterServices();
        ServiceLocator.InitializeAll();
    }
}
