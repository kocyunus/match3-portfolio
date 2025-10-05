using System;
using UnityEngine;

namespace Yunus.Match3
{
    /// <summary>
    /// Input handling için interface
    /// Farklı input sistemleri (Touch, Mouse, Keyboard) kullanabilirsin
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Swipe event (Tile + Yön)
        /// </summary>
        event Action<TileView, Vector2Int> OnSwipe;
        
        /// <summary>
        /// Tile tıklama event
        /// </summary>
        event Action<TileView> OnTileClick;
        
        /// <summary>
        /// Input sistemini başlat
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Input sistemini temizle
        /// </summary>
        void Cleanup();
    }
}

