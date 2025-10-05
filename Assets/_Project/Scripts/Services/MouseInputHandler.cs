using System;
using UnityEngine;

namespace Yunus.Match3
{
    /// <summary>
    /// Mouse input handler (Single Responsibility)
    /// TileView'dan input logic ayrıldı
    /// Gelecekte TouchInputHandler eklenebilir (Open/Closed)
    /// </summary>
    public class MouseInputHandler : MonoBehaviour, IInputHandler
    {
        public event Action<TileView, Vector2Int> OnSwipe;
        public event Action<TileView> OnTileClick;
        
        private Camera mainCamera;
        private TileView currentTile;
        private Vector3 mouseStartPos;
        private bool isDragging = false;
        private float swipeThreshold = 0.3f;
        
        // Performance: Sadece Tile layer'ını kontrol et
        private LayerMask tileLayerMask;
        
        public void Initialize()
        {
            mainCamera = Camera.main;
            
            // Layer mask setup (opsiyonel - yoksa tüm layer'lar kontrol edilir)
            // Unity'de "Tile" layer'ı varsa kullan, yoksa tüm layer'lar (default)
            int tileLayer = LayerMask.NameToLayer("Tile");
            if (tileLayer != -1)
            {
                tileLayerMask = LayerMask.GetMask("Tile");
                Debug.Log("[MouseInputHandler] Tile layer kullanılıyor (optimize) ✅");
            }
            else
            {
                tileLayerMask = ~0;  // Tüm layer'lar (default)
                Debug.LogWarning("[MouseInputHandler] Tile layer yok, tüm layer'lar kontrol ediliyor (yavaş!)");
            }
            
            if (mainCamera == null)
            {
                Debug.LogError("[MouseInputHandler] Camera.main bulunamadı!");
            }
            else
            {
                Debug.Log("[MouseInputHandler] Başlatıldı ✅");
            }
        }
        
        public void Cleanup()
        {
            OnSwipe = null;
            OnTileClick = null;
            currentTile = null;
        }
        
        private void Update()
        {
            HandleMouseInput();
        }
        
        private void HandleMouseInput()
        {
            // Mouse down: Tile seç
            if (Input.GetMouseButtonDown(0))
            {
                TileView tile = GetTileAtMousePosition();
                if (tile != null)
                {
                    currentTile = tile;
                    mouseStartPos = GetMouseWorldPosition();
                    isDragging = true;
                }
            }
            
            // Mouse drag: Swipe direction hesapla
            if (Input.GetMouseButton(0) && isDragging && currentTile != null)
            {
                Vector3 currentMousePos = GetMouseWorldPosition();
                Vector3 dragOffset = currentMousePos - mouseStartPos;
                
                // Yeterince hareket etti mi?
                if (dragOffset.magnitude >= swipeThreshold)
                {
                    Vector2Int swipeDirection = GetPrimarySwipeDirection(dragOffset);
                    
                    if (swipeDirection != Vector2Int.zero)
                    {
                        Debug.Log($"[MouseInputHandler] Swipe: Tile({currentTile.X},{currentTile.Y}) → {swipeDirection}");
                        OnSwipe?.Invoke(currentTile, swipeDirection);
                        isDragging = false;  // Bir kere tetikle
                    }
                }
            }
            
            // Mouse up: Drag bitti
            if (Input.GetMouseButtonUp(0))
            {
                // Eğer drag olmadıysa, click sayılır
                if (isDragging && currentTile != null)
                {
                    Vector3 currentMousePos = GetMouseWorldPosition();
                    Vector3 dragOffset = currentMousePos - mouseStartPos;
                    
                    // Çok az hareket = Click
                    if (dragOffset.magnitude < swipeThreshold)
                    {
                        OnTileClick?.Invoke(currentTile);
                    }
                }
                
                isDragging = false;
                currentTile = null;
            }
        }
        
        /// <summary>
        /// Mouse pozisyonundaki tile'ı bul (Raycast)
        /// Layer mask ile optimize edilmiş!
        /// </summary>
        private TileView GetTileAtMousePosition()
        {
            if (mainCamera == null) return null;
            
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            // Layer mask kullan (performans için!)
            RaycastHit2D hit = Physics2D.Raycast(
                ray.origin, 
                ray.direction, 
                100f, 
                tileLayerMask  // ← SADECE bu layer'daki objeler!
            );
            
            if (hit.collider != null)
            {
                TileView tile = hit.collider.GetComponent<TileView>();
                if (tile != null)
                {
                    Debug.Log($"[MouseInputHandler] Tile seçildi: ({tile.X},{tile.Y})");
                }
                return tile;
            }
            
            return null;
        }
        
        /// <summary>
        /// Mouse world pozisyonu
        /// </summary>
        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(mousePos);
        }
        
        /// <summary>
        /// Ana swipe yönünü belirle (4 yön)
        /// </summary>
        private Vector2Int GetPrimarySwipeDirection(Vector3 dragOffset)
        {
            if (dragOffset.magnitude < 0.1f)
                return Vector2Int.zero;
            
            // Hangi eksen baskın?
            if (Mathf.Abs(dragOffset.x) > Mathf.Abs(dragOffset.y))
            {
                return dragOffset.x > 0 ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                return dragOffset.y > 0 ? Vector2Int.up : Vector2Int.down;
            }
        }
    }
}

