using System.Collections.Generic;

namespace Yunus.Match3
{
    /// <summary>
    /// Match detection sistemi (Strategy Pattern kullanır)
    /// Yeni match stratejileri ekleyerek extend edilebilir (Open/Closed Principle)
    /// </summary>
    public class MatchDetector : IMatchDetector
    {
        private List<IMatchStrategy> strategies = new List<IMatchStrategy>();
        
        public MatchDetector()
        {
            // Varsayılan strateji: Line Match
            strategies.Add(new LineMatchStrategy());
            
            // Gelecekte eklenebilir:
            // strategies.Add(new LShapeMatchStrategy());
            // strategies.Add(new TShapeMatchStrategy());
        }
        
        /// <summary>
        /// Custom strategy ekle
        /// </summary>
        public void AddStrategy(IMatchStrategy strategy)
        {
            if (!strategies.Contains(strategy))
            {
                strategies.Add(strategy);
            }
        }
        
        /// <summary>
        /// Herhangi bir strateji ile match var mı?
        /// </summary>
        public bool HasMatch(Tile tile, Grid grid)
        {
            if (tile == null || grid == null) return false;
            
            foreach (var strategy in strategies)
            {
                if (strategy.HasMatch(tile, grid))
                {
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Tüm stratejilerden match'leri topla (tek tile için)
        /// </summary>
        public List<Tile> FindMatches(Tile tile, Grid grid)
        {
            List<Tile> allMatches = new List<Tile>();
            if (tile == null || grid == null) return allMatches;
            
            foreach (var strategy in strategies)
            {
                List<Tile> matches = strategy.FindMatches(tile, grid);
                
                // Duplicate'leri önle
                foreach (var match in matches)
                {
                    if (!allMatches.Contains(match))
                    {
                        allMatches.Add(match);
                    }
                }
            }
            
            return allMatches;
        }
        
        /// <summary>
        /// Tüm board'u tara ve tüm match'leri bul
        /// Cascade detection için kullanılır
        /// </summary>
        public List<Tile> FindAllMatches(Grid grid)
        {
            if (grid == null) return new List<Tile>();
            
            HashSet<Tile> allMatchedTiles = new HashSet<Tile>(); // Duplicate önleme için HashSet
            
            // Tüm grid'i tara
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    Tile tile = grid.GetTile(x, y);
                    if (tile == null) continue;
                    
                    // Bu tile match yapıyor mu?
                    List<Tile> matches = FindMatches(tile, grid);
                    
                    // Match'leri set'e ekle (otomatik duplicate önler)
                    foreach (var match in matches)
                    {
                        allMatchedTiles.Add(match);
                    }
                }
            }
            
            return new List<Tile>(allMatchedTiles);
        }
    }
}

