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
            
            // Orijinal pozisyonları GÖRSEL olarak kaydet (current transform position)
            tile1OriginalPos = tile1.transform.localPosition;
            tile2OriginalPos = tile2.transform.localPosition;
        }
        
        /// <summary>
        /// Swap işlemini yap (data + visual)
        /// </summary>
        public void Execute()
        {
            // 1. ÖNCE Grid swap yap (Tile.X ve Tile.Y değişecek!)
            grid.SwapTiles(tile1.Tile, tile2.Tile);
            
            // 2. Görsel pozisyonları swap et (tile2'nin görsel pozisyonuna git)
            Vector3 tile1Target = tile2OriginalPos;
            Vector3 tile2Target = tile1OriginalPos;
            
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

