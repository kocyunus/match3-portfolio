using UnityEngine;
using System.Collections.Generic;

namespace Yunus.Match3
{
    /// <summary>
    /// Board game logic controller (Single Responsibility)
    /// Sadece game logic'ten sorumlu (swap, match, events)
    /// Görsel işler BoardView'da
    /// </summary>
    public class BoardController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private BoardView boardView;
        [SerializeField] private MouseInputHandler inputHandler;
        
        private Grid grid;
        private IMatchDetector matchDetector;
        private Dictionary<Tile, TileView> tileViews;
        private TileView selectedTile = null;
        private bool isProcessingSwap = false;
        
        private void Awake()
        {
            // Dependency Injection
            matchDetector = new MatchDetector();
        }
        
        private void Start()
        {
            InitializeBoard();
        }
        
        private void OnEnable()
        {
            // Input events
            if (inputHandler != null)
            {
                inputHandler.OnSwipe += HandleSwipe;
                inputHandler.OnTileClick += HandleTileClick;
            }
        }
        
        private void OnDisable()
        {
            if (inputHandler != null)
            {
                inputHandler.OnSwipe -= HandleSwipe;
                inputHandler.OnTileClick -= HandleTileClick;
            }
        }
        
        /// <summary>
        /// Board'u başlat
        /// </summary>
        private void InitializeBoard()
        {
            // Auto-find references (Inspector'da unutulursa)
            if (boardView == null)
            {
                boardView = GetComponent<BoardView>();
            }
            
            if (inputHandler == null)
            {
                inputHandler = GetComponent<MouseInputHandler>();
            }
            
            if (boardView == null)
            {
                Debug.LogError("[BoardController] BoardView bulunamadı!");
                return;
            }
            
            // BoardView'dan grid ve tileViews al
            grid = boardView.Grid;
            tileViews = boardView.TileViews;
            
            // InputHandler başlat
            if (inputHandler != null)
            {
                inputHandler.Initialize();
                Debug.Log("[BoardController] Input handler başlatıldı ✅");
            }
            else
            {
                Debug.LogWarning("[BoardController] InputHandler bulunamadı!");
            }
            
            Debug.Log("[BoardController] Board başlatıldı ✅");
        }
        
        /// <summary>
        /// Swipe event handler
        /// </summary>
        private void HandleSwipe(TileView tile, Vector2Int direction)
        {
            // GUARD: Zaten bir swap devam ediyor mu?
            if (isProcessingSwap)
            {
                return;
            }
            
            // Komşu tile bul
            int targetX = tile.X + direction.x;
            int targetY = tile.Y + direction.y;
            
            Tile targetTile = grid.GetTile(targetX, targetY);
            if (targetTile == null) return;
            
            if (!tileViews.ContainsKey(targetTile)) return;
            
            TileView targetTileView = tileViews[targetTile];
            
            // LOCK ve Swap
            isProcessingSwap = true;
            SwapTiles(tile, targetTileView);
        }
        
        /// <summary>
        /// Tile click event handler
        /// </summary>
        private void HandleTileClick(TileView clickedTile)
        {
            if (isProcessingSwap) return;
            
            if (selectedTile == null)
            {
                SelectTile(clickedTile);
            }
            else
            {
                TrySwapTiles(selectedTile, clickedTile);
            }
        }
        
        private void SelectTile(TileView tile)
        {
            selectedTile = tile;
            tile.Select();
        }
        
        private void DeselectTile()
        {
            if (selectedTile != null)
            {
                selectedTile.Deselect();
                selectedTile = null;
            }
        }
        
        private void TrySwapTiles(TileView tile1, TileView tile2)
        {
            if (tile1 == tile2)
            {
                DeselectTile();
                return;
            }
            
            if (tile1.Tile.IsNeighbor(tile2.Tile))
            {
                isProcessingSwap = true;
                SwapTiles(tile1, tile2);
            }
            
            DeselectTile();
        }
        
        /// <summary>
        /// Swap işlemini başlat
        /// </summary>
        private void SwapTiles(TileView view1, TileView view2)
        {
            SwapCommand swapCommand = new SwapCommand(view1, view2, grid);
            swapCommand.Execute();
            
            StartCoroutine(CheckMatchAfterSwap(swapCommand, view1, view2));
        }
        
        /// <summary>
        /// Swap sonrası match kontrolü
        /// </summary>
        private System.Collections.IEnumerator CheckMatchAfterSwap(SwapCommand command, TileView tile1, TileView tile2)
        {
            // Animasyon bitsin
            yield return new WaitForSeconds(0.35f);
            
            // Match var mı? (IMatchDetector kullan!)
            bool hasMatch = matchDetector.HasMatch(tile1.Tile, grid) || matchDetector.HasMatch(tile2.Tile, grid);
            
            if (hasMatch)
            {
                Debug.Log("Match found! ✅");
                
                // Match'leri bul ve yok et
                List<Tile> matches1 = matchDetector.FindMatches(tile1.Tile, grid);
                List<Tile> matches2 = matchDetector.FindMatches(tile2.Tile, grid);
                
                // İki listeyi birleştir (duplicate önle)
                HashSet<Tile> allMatches = new HashSet<Tile>(matches1);
                foreach (var tile in matches2)
                {
                    allMatches.Add(tile);
                }
                
                // Destroy et
                yield return StartCoroutine(DestroyMatchedTiles(new List<Tile>(allMatches)));
                
                isProcessingSwap = false;
            }
            else
            {
                Debug.Log("No match - reverting ❌");
                command.Undo();
                
                yield return new WaitForSeconds(0.7f);
                isProcessingSwap = false;
            }
        }
        
        /// <summary>
        /// Match olan tile'ları yok et (animasyon ile)
        /// </summary>
        private System.Collections.IEnumerator DestroyMatchedTiles(List<Tile> matchedTiles)
        {
            Debug.Log($"Destroying {matchedTiles.Count} tiles...");
            
            foreach (var tile in matchedTiles)
            {
                if (tileViews.ContainsKey(tile))
                {
                    TileView view = tileViews[tile];
                    
                    // Animasyon başlat (DOTween)
                    view.DestroyWithAnimation(0.25f);
                    
                    // Dictionary'den çıkar
                    tileViews.Remove(tile);
                    
                    // Grid'den çıkar
                    grid.RemoveTile(tile.X, tile.Y);
                }
            }
            
            // Tüm animasyonlar bitsin
            yield return new WaitForSeconds(0.3f);
            
            Debug.Log("Tiles destroyed! ✅");
        }
    }
}

