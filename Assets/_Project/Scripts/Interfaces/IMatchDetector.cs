using System.Collections.Generic;

namespace Yunus.Match3
{
    /// <summary>
    /// Match detection için interface (Dependency Inversion)
    /// Farklı match detection stratejileri kullanabilirsin
    /// </summary>
    public interface IMatchDetector
    {
        /// <summary>
        /// Tile'ın etrafında match var mı kontrol et
        /// </summary>
        bool HasMatch(Tile tile, Grid grid);
        
        /// <summary>
        /// Match'e giren tüm tile'ları bul
        /// </summary>
        List<Tile> FindMatches(Tile tile, Grid grid);
    }
}

