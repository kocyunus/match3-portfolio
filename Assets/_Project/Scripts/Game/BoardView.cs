using UnityEngine;
using System.Collections.Generic;

namespace Yunus.Match3
{
    /// <summary>
    /// Board görsel yöneticisi (Single Responsibility - SADECE görsel!)
    /// Grid oluşturur, tile spawn eder, render işleri yapar
    /// Game logic BoardController'da!
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] private int width = 8;
        [SerializeField] private int height = 8;
        
        [Header("Tile Settings")]
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Sprite[] tileSprites;
        
        [Header("Visual Settings")]
        [SerializeField] private GameObject backgroundTilePrefab;
        [SerializeField] private Vector2 boardOffset = Vector2.zero;
        
        private Grid grid;
        private Dictionary<Tile, TileView> tileViews = new Dictionary<Tile, TileView>();
        
        // Public accessors (BoardController için)
        public Grid Grid => grid;
        public Dictionary<Tile, TileView> TileViews => tileViews;
        
        private void Awake()
        {
            InitializeGrid();
            CreateTiles();
        }
        
        /// <summary>
        /// Grid'i oluştur (sadece data)
        /// </summary>
        private void InitializeGrid()
        {
            grid = new Grid(width, height);
        }
        
        private void CreateTiles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Background tile spawn et (opsiyonel)
                    if (backgroundTilePrefab != null)
                    {
                        SpawnBackgroundTile(x, y);
                    }
                    
                    // Tile spawn et
                    TileType randomType = (TileType)Random.Range(0, 6);
                    Tile tile = new Tile(x, y, randomType);
                    grid.SetTile(x, y, tile);
                    SpawnTileView(tile);
                }
            }
        }
        
        private void SpawnBackgroundTile(int x, int y)
        {
            GameObject bgTile = Instantiate(backgroundTilePrefab, transform);
            bgTile.name = $"BG_{x}_{y}";
            bgTile.transform.localPosition = new Vector3(x, y, 0);
        }
        
        private void SpawnTileView(Tile tile)
        {
            if (tilePrefab == null) return;
            
            // Tile'ı BoardView'ın child'ı olarak spawn et (local position kullanacak)
            GameObject tileObj = Instantiate(tilePrefab, transform);
            tileObj.name = $"Tile_{tile.X}_{tile.Y}";
            
            TileView tileView = tileObj.GetComponent<TileView>();
            if (tileView == null) return;
            
            Sprite sprite = GetSpriteForTileType(tile.Type);
            tileView.Initialize(tile, sprite);
            tileViews[tile] = tileView;
        }
        
        private Vector3 GridToWorldPosition(int gridX, int gridY)
        {
            float worldX = gridX + boardOffset.x;
            float worldY = gridY + boardOffset.y;
            return new Vector3(worldX, worldY, 0);
        }
        
        /// <summary>
        /// Tile için sprite al
        /// </summary>
        private Sprite GetSpriteForTileType(TileType type)
        {
            if (tileSprites == null || tileSprites.Length == 0) return null;
            int index = (int)type;
            if (index >= 0 && index < tileSprites.Length)
                return tileSprites[index];
            return null;
        }
        
        /// <summary>
        /// Board'u yeniden doldur - boş column'ları yukarıdan tile spawn et
        /// Gravity sonrası çağrılır
        /// </summary>
        /// <returns>Yeni spawn olan tile view'lar (animation için)</returns>
        public List<TileView> RefillBoard()
        {
            List<TileView> newTiles = new List<TileView>();
            
            // Her column'ı tara
            for (int x = 0; x < grid.Width; x++)
            {
                // Bu column'da kaç boş var?
                int emptyCount = 0;
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.GetTile(x, y) == null)
                    {
                        emptyCount++;
                    }
                }
                
                // Boş varsa yukarıdan doldur
                for (int i = 0; i < emptyCount; i++)
                {
                    int y = grid.Height - 1 - i; // Üstten başla: Y=7, Y=6, Y=5...
                    
                    // Yeni tile oluştur (random type)
                    TileType randomType = (TileType)Random.Range(0, System.Enum.GetValues(typeof(TileType)).Length);
                    Tile newTile = new Tile(x, y, randomType);
                    
                    // Grid'e ekle
                    grid.SetTile(x, y, newTile);
                    
                    // View oluştur
                    GameObject tileObj = Instantiate(tilePrefab, transform);
                    tileObj.name = $"Tile_{x}_{y}";
                    
                    TileView tileView = tileObj.GetComponent<TileView>();
                    Sprite sprite = GetSpriteForTileType(randomType);
                    tileView.Initialize(newTile, sprite);
                    
                    // Dictionary'e ekle
                    tileViews[newTile] = tileView;
                    
                    // Liste'ye ekle (animation için)
                    newTiles.Add(tileView);
                    
                    // Başlangıç pozisyonu yukarıda (animation için)
                    tileView.transform.localPosition = new Vector3(x, grid.Height + i, 0);
                    
                    UnityEngine.Debug.Log($"[BoardView] Spawned new tile at ({x},{y})");
                }
            }
            
            return newTiles;
        }
    }
}
