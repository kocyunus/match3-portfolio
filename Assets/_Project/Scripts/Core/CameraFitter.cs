using UnityEngine;

/// <summary>
/// Kamerayı board'a ve ekran boyutuna göre otomatik ayarlar
/// Tüm cihazlarda board tam ekrana sığar (mobil, tablet, desktop)
/// </summary>
public class CameraFitter : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int boardWidth = 8;
    [SerializeField] private int boardHeight = 8;
    
    [Header("Camera Settings")]
    [SerializeField] private float padding = 1f;
    [SerializeField] private bool useSafeArea = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    private void AdjustCamera()
    {
        // Ekran bilgilerini al
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // Safe Area kullan (iPhone notch için)
        if (useSafeArea)
        {
            Rect safeArea = Screen.safeArea;
            screenWidth = safeArea.width;
            screenHeight = safeArea.height;
        }

        // Aspect ratio hesapla
        float screenRatio = screenWidth / screenHeight;
        float boardRatio = (float)boardWidth / boardHeight;

        // Kamera boyutunu ayarla
        if (screenRatio >= boardRatio)
        {
            // Ekran daha geniş (landscape veya geniş telefon)
            // Height'a göre fit et
            cam.orthographicSize = (boardHeight / 2f) + padding;
        }
        else
        {
            // Ekran daha dar (portrait veya dar telefon)
            // Width'e göre fit et
            float ratio = boardRatio / screenRatio;
            cam.orthographicSize = (boardHeight / 2f) * ratio + padding;
        }

        // Kamerayı board'un ortasına getir
        float centerX = (boardWidth - 1) / 2f;
        float centerY = (boardHeight - 1) / 2f;
        transform.position = new Vector3(centerX, centerY, -10);

        // Debug bilgisi
        if (showDebugInfo)
        {
            Debug.Log($"[CameraFitter] Ekran: {Screen.width}x{Screen.height}");
            Debug.Log($"[CameraFitter] Safe Area: {Screen.safeArea}");
            Debug.Log($"[CameraFitter] Screen Ratio: {screenRatio:F2}");
            Debug.Log($"[CameraFitter] Board Ratio: {boardRatio:F2}");
            Debug.Log($"[CameraFitter] Camera Size: {cam.orthographicSize:F2}");
            Debug.Log($"[CameraFitter] Camera Pos: {transform.position}");
        }
    }

    // Ekran döndüğünde yeniden hesapla
    private void OnRectTransformDimensionsChange()
    {
        if (cam != null)
        {
            AdjustCamera();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Test: Recalculate Camera")]
    private void TestRecalculate()
    {
        if (cam == null) cam = GetComponent<Camera>();
        AdjustCamera();
    }
#endif
}

