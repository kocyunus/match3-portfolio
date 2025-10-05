using System.Collections.Generic;

namespace Yunus.Match3
{
    /// <summary>
    /// Match bulma stratejisi (Strategy Pattern)
    /// Yeni match türleri eklemek için bu interface'i implement et
    /// Örnek: LineMatch, LShapeMatch, TShapeMatch
    /// </summary>
    public interface IMatchStrategy
    {
        /// <summary>
        /// Strateji adı (debug için)
        /// </summary>
        string StrategyName { get; }
        
        /// <summary>
        /// Bu stratejiyle match var mı?
        /// </summary>
        bool HasMatch(Tile tile, Grid grid);
        
        /// <summary>
        /// Match'e giren tile'ları bul
        /// </summary>
        List<Tile> FindMatches(Tile tile, Grid grid);
    }
}

