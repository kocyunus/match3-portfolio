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
        private GameStateService gameStateService;
        
        private void Awake()
        {
            // Dependency Injection
            matchDetector = new MatchDetector();

            InitializeGameStateService();
        }
        
        private void Start()
        {
            InitializeGameStateService();
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

        private void OnDestroy()
        {
            if (gameStateService != null)
            {
                gameStateService.OnStateChanged -= HandleGameStateChanged;
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

            TrySetGameState(GameStateType.Ready);
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

            if (gameStateService != null && !gameStateService.IsPlayerInputAllowed)
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

            if (gameStateService != null && !gameStateService.IsPlayerInputAllowed)
            {
                return;
            }
            
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
            TrySetGameState(GameStateType.Processing);

            SwapCommand swapCommand = new SwapCommand(view1, view2, grid);
            swapCommand.Execute();
            
            // KRİTİK: Swap sonrası dictionary güncelle!
            // Çünkü Tile.GetHashCode() X,Y'ye bağlı, değişince hash değişir!
            UpdateTileViewsDictionary();
            
            StartCoroutine(CheckMatchAfterSwap(swapCommand, view1, view2));
        }
        
        /// <summary>
        /// Dictionary'i yeniden oluştur (Tile pozisyonları değiştiğinde gerekli)
        /// </summary>
        private void UpdateTileViewsDictionary()
        {
            // Mevcut TileView'ları sakla
            var allViews = new System.Collections.Generic.List<TileView>(tileViews.Values);
            
            // Dictionary'i temizle
            tileViews.Clear();
            
            // Yeniden ekle (güncel Tile referansları ile)
            foreach (var view in allViews)
            {
                if (view != null && view.Tile != null)
                {
                    tileViews[view.Tile] = view;
                }
            }
            
            Debug.Log($"[UpdateDict] Dictionary updated: {tileViews.Count} tiles");
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
                
                // DEBUG: Match sayılarını logla
                Debug.Log($"[DEBUG] Tile1 ({tile1.X},{tile1.Y}) matches: {matches1.Count}");
                Debug.Log($"[DEBUG] Tile2 ({tile2.X},{tile2.Y}) matches: {matches2.Count}");
                
                // İki listeyi birleştir (duplicate önle)
                HashSet<Tile> allMatches = new HashSet<Tile>(matches1);
                foreach (var tile in matches2)
                {
                    allMatches.Add(tile);
                }
                
                // DEBUG: Toplam unique tile sayısı
                Debug.Log($"[DEBUG] Total unique matches: {allMatches.Count}");
                
                // Destroy et
                yield return StartCoroutine(DestroyMatchedTiles(new List<Tile>(allMatches)));
                
                isProcessingSwap = false;
            }
            else
            {
                Debug.Log("No match - reverting ❌");
                command.Undo();

                // KRİTİK: Undo sonrası da dictionary güncelle!
                UpdateTileViewsDictionary();

                yield return new WaitForSeconds(0.7f);
                isProcessingSwap = false;
                TrySetGameState(GameStateType.Ready);
            }
        }
        
        /// <summary>
        /// Match olan tile'ları yok et (animasyon ile)
        /// </summary>
        private System.Collections.IEnumerator DestroyMatchedTiles(List<Tile> matchedTiles)
        {
            Debug.Log($"Destroying {matchedTiles.Count} tiles...");
            
            // GEÇİCİ LİSTE: Yok olan tile'ların pozisyonları (X,Y)
            List<(int x, int y)> destroyedPositions = new List<(int x, int y)>();
            
            int destroyedCount = 0;
            
            foreach (var tile in matchedTiles)
            {
                Debug.Log($"[Destroy] Checking tile at ({tile.X},{tile.Y}), Type: {tile.Type}");
                
                if (tileViews.ContainsKey(tile))
                {
                    TileView view = tileViews[tile];
                    
                    Debug.Log($"[Destroy] ✅ DESTROYING tile at ({tile.X},{tile.Y})");
                    
                    // GEÇİCİ LİSTEYE EKLE!
                    destroyedPositions.Add((tile.X, tile.Y));
                    
                    // Animasyon başlat (DOTween)
                    view.DestroyWithAnimation(0.25f);
                    
                    // Dictionary'den çıkar
                    tileViews.Remove(tile);
                    
                    // Grid'den çıkar
                    grid.RemoveTile(tile.X, tile.Y);
                    
                    destroyedCount++;
                }
                else
                {
                    Debug.LogError($"[Destroy] ❌ TILE NOT FOUND IN DICTIONARY! ({tile.X},{tile.Y})");
                }
            }
            
            // Tüm animasyonlar bitsin
            yield return new WaitForSeconds(0.3f);
            
            Debug.Log($"Tiles destroyed! ✅ ({destroyedCount}/{matchedTiles.Count})");
            
            // GRAVITY UYGULA (sadece destroy edilen pozisyonlara!)
            yield return StartCoroutine(ProcessGravityOptimized(destroyedPositions));
        }
        
        /// <summary>
        /// OPTİMİZE EDİLMİŞ GRAVITY: Column Compacting Algorithm
        /// Sadece etkilenen column'ları işle - garantili çalışır!
        /// </summary>
        private System.Collections.IEnumerator ProcessGravityOptimized(List<(int x, int y)> destroyedPositions)
        {
            Debug.Log($"[Gravity-Compacting] Processing {destroyedPositions.Count} destroyed positions...");
            
            // ADIM 1: Etkilenen column'ları bul (HashSet = unique X'ler)
            HashSet<int> affectedColumns = new HashSet<int>();
            foreach (var pos in destroyedPositions)
            {
                affectedColumns.Add(pos.x);
            }
            
            Debug.Log($"[Gravity-Compacting] Affected columns: {affectedColumns.Count}");
            
            // ADIM 2: Her column için gravity uygula (COLUMN COMPACTING)
            foreach (int x in affectedColumns)
            {
                Debug.Log($"[Gravity-Compacting] ═══ Processing Column X={x} ═══");
                
                // Column'daki tüm DOLU tile'ları topla (alttan üste)
                List<Tile> solidTiles = new List<Tile>();
                
                for (int y = 0; y < grid.Height; y++)
                {
                    Tile tile = grid.GetTile(x, y);
                    if (tile != null)
                    {
                        solidTiles.Add(tile);
                        Debug.Log($"[Gravity-Compacting]   Found solid tile at ({x},{y}), Type: {tile.Type}");
                    }
                }
                
                Debug.Log($"[Gravity-Compacting] Column X={x} has {solidTiles.Count} solid tiles");
                
                // Column'ı temizle
                for (int y = 0; y < grid.Height; y++)
                {
                    grid.SetTile(x, y, null);
                }
                
                // Dolu tile'ları ALTTAN BAŞLAYARAK yerleştir
                for (int i = 0; i < solidTiles.Count; i++)
                {
                    Tile tile = solidTiles[i];
                    int oldY = tile.Y;
                    int newY = i;  // Y=0'dan başla (alt), Y=1, Y=2...
                    
                    // KRİTİK: TileView'ı ÖNCE BUL (tile.SetPosition öncesi!)
                    TileView view = null;
                    if (tileViews.ContainsKey(tile))
                    {
                        view = tileViews[tile];
                    }
                    
                    // Grid'e yerleştir
                    grid.SetTile(x, newY, tile);
                    
                    // Tile pozisyonunu güncelle (HASH DEĞİŞİR!)
                    tile.SetPosition(x, newY);
                    
                    // Animasyon (sadece hareket ettiyse)
                    if (oldY != newY && view != null)
                    {
                        view.AnimateFall(newY, 0.3f);
                        Debug.Log($"[Gravity-Compacting]   ✅ Falling: {tile.Type} Y={oldY}→{newY}");
                    }
                }
                
                Debug.Log($"[Gravity-Compacting] ═══ Column X={x} Complete ═══");
            }
            
            // Animasyonlar bitsin
            yield return new WaitForSeconds(0.35f);
            
            // KRİTİK: Dictionary güncelle (X,Y değişti, hash değişti!)
            UpdateTileViewsDictionary();
            Debug.Log("[Gravity-Compacting] Complete! Dictionary updated ✅");
            
            // CASCADE KONTROLÜ: Gravity sonrası yeni match var mı?
            yield return StartCoroutine(CheckForCascade());
        }
        
        /// <summary>
        /// CASCADE SYSTEM: Gravity sonrası yeni match var mı kontrol et
        /// Varsa tekrar patlat (LOOP!), yoksa Refill yap
        /// </summary>
        private System.Collections.IEnumerator CheckForCascade()
        {
            Debug.Log("[CASCADE] Checking for new matches after gravity...");
            
            // TÜM BOARD'U TARA! (Yeni matchler olabilir)
            List<Tile> allMatches = matchDetector.FindAllMatches(grid);
            
            if (allMatches.Count > 0)
            {
                Debug.Log($"[CASCADE] ✅ NEW MATCHES FOUND! {allMatches.Count} tiles");
                Debug.Log("[CASCADE] 🔄 CONTINUING CASCADE LOOP...");
                
                // YENİ MATCH VAR! Tekrar patlat → Gravity → Kontrol (RECURSIVE!)
                yield return StartCoroutine(DestroyMatchedTiles(allMatches));
                
                // DestroyMatchedTiles zaten Gravity'yi çağırıyor,
                // Gravity de CheckForCascade'i çağırıyor → LOOP! 🔄
            }
            else
            {
                Debug.Log("[CASCADE] ❌ No new matches found");
                Debug.Log("[CASCADE] → Proceeding to REFILL");
                
                // MATCH YOK! Refill yap ve bitir
                yield return StartCoroutine(ProcessRefill());
            }
        }
        
        /// <summary>
        /// ESKİ GRAVITY (FULL SCAN) - Şimdilik sakla
        /// </summary>
        private System.Collections.IEnumerator ProcessGravity()
        {
            Debug.Log("Applying gravity...");
            
            // Grid'de gravity hesapla
            var moves = grid.ApplyGravity();
            
            if (moves.Count > 0)
            {
                Debug.Log($"Gravity: {moves.Count} tiles falling");
                
                // Animasyonları başlat
                foreach (var move in moves)
                {
                    if (tileViews.ContainsKey(move.tile))
                    {
                        TileView view = tileViews[move.tile];
                        view.AnimateFall(move.newY, 0.3f);
                    }
                }
                
                // Animasyonlar bitsin
                yield return new WaitForSeconds(0.35f);
                
                Debug.Log("Gravity complete! ✅");
                
                // KRİTİK: Gravity sonrası dictionary güncelle!
                // Çünkü tile'ların X,Y değişti, hash code değişti!
                UpdateTileViewsDictionary();
                Debug.Log("[STEP 2/3] Gravity Complete → Dictionary Updated ✅");
            }
            
            // BOARD'U YENİDEN DOLDUR! (ŞİMDİLİK KAPALI - GRAVITY TESTİ)
            // yield return StartCoroutine(ProcessRefill());
            
            Debug.Log("═══════════════════════════════════════");
            Debug.Log("GRAVITY TEST COMPLETE! (Refill disabled)");
        }
        
        /// <summary>
        /// Board'u yeniden doldur - yeni tile'lar spawn et
        /// </summary>
        private System.Collections.IEnumerator ProcessRefill()
        {
            Debug.Log("Refilling board...");
            
            // Yeni tile'lar spawn et
            List<TileView> newTiles = boardView.RefillBoard();
            
            if (newTiles.Count == 0)
            {
                Debug.Log("No refill needed");
                yield break;
            }
            
            Debug.Log($"Refill: {newTiles.Count} new tiles");
            
            // Yeni tile'ları düşür (animation)
            foreach (var tileView in newTiles)
            {
                tileView.AnimateFall(tileView.Y, 0.4f);
            }
            
            // Animasyonlar bitsin
            yield return new WaitForSeconds(0.45f);
            
            Debug.Log("Refill complete! ✅");
            
            // KRİTİK: Refill sonrası dictionary güncelle!
            // Yeni tile'lar eklendi, hash'ler güncel olmalı
            UpdateTileViewsDictionary();
            Debug.Log("[Refill] Dictionary Updated ✅");
            
            Debug.Log("═══════════════════════════════════════");
            Debug.Log("🎮 MATCH-3 GAME LOOP COMPLETE! 🎮");
            Debug.Log("Player can move again.");
            Debug.Log("═══════════════════════════════════════");

            TrySetGameState(GameStateType.Ready);
        }

        private void InitializeGameStateService()
        {
            if (gameStateService != null)
            {
                return;
            }

            if (ServiceLocator.IsRegistered<GameStateService>())
            {
                gameStateService = ServiceLocator.Get<GameStateService>();
                gameStateService.OnStateChanged += HandleGameStateChanged;

                HandleGameStateChanged(new GameStateChangedEventArgs(
                    gameStateService.PreviousState,
                    gameStateService.CurrentState,
                    GameStateTransitionSource.System,
                    gameStateService.IsPlayerInputAllowed));
            }
            else
            {
                Debug.LogWarning("[BoardController] GameStateService bulunamadı. Input kilitleme devre dışı kalacak.");
            }
        }

        private void HandleGameStateChanged(GameStateChangedEventArgs args)
        {
            if (!args.AllowsPlayerInput)
            {
                DeselectTile();
            }

            if (inputHandler != null)
            {
                inputHandler.enabled = args.AllowsPlayerInput;
            }
        }

        private bool TrySetGameState(GameStateType state)
        {
            if (gameStateService == null)
            {
                return false;
            }

            if (state == GameStateType.Ready && gameStateService.CurrentState == GameStateType.Booting)
            {
                gameStateService.TrySetState(GameStateType.Loading, GameStateTransitionSource.GameplayLoop);
            }

            if (!gameStateService.TrySetState(state, GameStateTransitionSource.GameplayLoop))
            {
                Debug.LogWarning($"[BoardController] Game state güncellenemedi: {state}");
                return false;
            }

            return true;
        }
    }
}

