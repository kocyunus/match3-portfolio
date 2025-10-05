using UnityEngine;
using DG.Tweening;

namespace Yunus.Match3
{
    /// <summary>
    /// Tile swap işlemi için Command Pattern
    /// Execute: Swap yap
    /// Undo: Geri al (match yoksa)
    /// </summary>
    public class SwapCommand
    {
        private TileView tile1;
        private TileView tile2;
        private Grid grid;
        
        // Orijinal pozisyonlar (undo için)
        private Vector3 tile1OriginalPos;
        private Vector3 tile2OriginalPos;
        
        public SwapCommand(TileView tile1, TileView tile2, Grid grid)
        {
            this.tile1 = tile1;
            this.tile2 = tile2;
            this.grid = grid;
            
            // Orijinal pozisyonları Tile DATA'dan al (tek kaynak!)
            tile1OriginalPos = new Vector3(tile1.Tile.X, tile1.Tile.Y, 0);
            tile2OriginalPos = new Vector3(tile2.Tile.X, tile2.Tile.Y, 0);
        }
        
        /// <summary>
        /// Swap işlemini yap (data + visual)
        /// </summary>
        public void Execute()
        {
            // 1. ÖNCE Grid swap yap (Tile.X ve Tile.Y değişecek!)
            int tile1OrigX = tile1.Tile.X;
            int tile1OrigY = tile1.Tile.Y;
            int tile2OrigX = tile2.Tile.X;
            int tile2OrigY = tile2.Tile.Y;
            
            grid.SwapTiles(tile1.Tile, tile2.Tile);
            
            // 2. Hedef pozisyonlar = swap ÖNCESI X,Y koordinatları (Tile data'dan!)
            Vector3 tile1Target = new Vector3(tile2OrigX, tile2OrigY, 0);
            Vector3 tile2Target = new Vector3(tile1OrigX, tile1OrigY, 0);
            
            tile1.MoveTo(tile1Target, 0.3f);
            tile2.MoveTo(tile2Target, 0.3f);
        }
        
        /// <summary>
        /// Swap'i geri al (match yoksa)
        /// Sadece ilk tile shake olur (user feedback)
        /// </summary>
        public void Undo()
        {
            // 1. Data geri al
            grid.SwapTiles(tile1.Tile, tile2.Tile);
            
            // 2. DOTween Sequence: Shake → sonra geri dön (Tile data pozisyonlarına!)
            Sequence undoSequence = DOTween.Sequence();
            
            // Önce shake (sadece tile1)
            undoSequence.Append(tile1.transform.DOShakePosition(0.4f, 0.12f, 10, 90, false, true));
            
            // Sonra her ikisi de orijinal pozisyonlarına dön (paralel)
            undoSequence.Append(tile1.transform.DOLocalMove(tile1OriginalPos, 0.25f).SetEase(Ease.OutCubic));
            undoSequence.Join(tile2.transform.DOLocalMove(tile2OriginalPos, 0.25f).SetEase(Ease.OutCubic));
        }
    }
}

