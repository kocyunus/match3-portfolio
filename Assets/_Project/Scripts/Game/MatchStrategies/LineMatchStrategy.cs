using System.Collections.Generic;

namespace Yunus.Match3
{
    /// <summary>
    /// Yatay/Dikey 3+ eşleşme stratejisi
    /// En temel match türü (Candy Crush standart)
    /// </summary>
    public class LineMatchStrategy : IMatchStrategy
    {
        public string StrategyName => "Line Match (3+)";
        
        public bool HasMatch(Tile tile, Grid grid)
        {
            if (tile == null) return false;
            
            // Yatay kontrol
            int horizontalCount = CountMatchesInDirection(tile, grid, 1, 0) + CountMatchesInDirection(tile, grid, -1, 0) + 1;
            if (horizontalCount >= 3) return true;
            
            // Dikey kontrol
            int verticalCount = CountMatchesInDirection(tile, grid, 0, 1) + CountMatchesInDirection(tile, grid, 0, -1) + 1;
            return verticalCount >= 3;
        }
        
        public List<Tile> FindMatches(Tile tile, Grid grid)
        {
            List<Tile> matches = new List<Tile>();
            if (tile == null) return matches;
            
            // Yatay match'leri bul
            List<Tile> horizontalMatches = FindMatchesInLine(tile, grid, 1, 0, -1, 0);
            if (horizontalMatches.Count >= 3)
            {
                matches.AddRange(horizontalMatches);
            }
            
            // Dikey match'leri bul
            List<Tile> verticalMatches = FindMatchesInLine(tile, grid, 0, 1, 0, -1);
            if (verticalMatches.Count >= 3)
            {
                matches.AddRange(verticalMatches);
            }
            
            return matches;
        }
        
        /// <summary>
        /// Belirli yönde kaç eşleşme var?
        /// </summary>
        private int CountMatchesInDirection(Tile tile, Grid grid, int dirX, int dirY)
        {
            int count = 0;
            int x = tile.X + dirX;
            int y = tile.Y + dirY;
            
            while (grid.IsValidPosition(x, y))
            {
                Tile neighbor = grid.GetTile(x, y);
                if (neighbor != null && neighbor.CanMatchWith(tile))
                {
                    count++;
                    x += dirX;
                    y += dirY;
                }
                else break;
            }
            
            return count;
        }
        
        /// <summary>
        /// Bir çizgideki tüm eşleşmeleri bul (2 yön)
        /// </summary>
        private List<Tile> FindMatchesInLine(Tile tile, Grid grid, int dir1X, int dir1Y, int dir2X, int dir2Y)
        {
            List<Tile> matches = new List<Tile> { tile };  // Kendisi
            
            // Yön 1 (örn: sağ)
            AddMatchesInDirection(matches, tile, grid, dir1X, dir1Y);
            
            // Yön 2 (örn: sol)
            AddMatchesInDirection(matches, tile, grid, dir2X, dir2Y);
            
            return matches;
        }
        
        /// <summary>
        /// Belirli yöndeki tüm eşleşmeleri listeye ekle
        /// </summary>
        private void AddMatchesInDirection(List<Tile> matches, Tile tile, Grid grid, int dirX, int dirY)
        {
            int x = tile.X + dirX;
            int y = tile.Y + dirY;
            
            while (grid.IsValidPosition(x, y))
            {
                Tile neighbor = grid.GetTile(x, y);
                if (neighbor != null && neighbor.CanMatchWith(tile))
                {
                    matches.Add(neighbor);
                    x += dirX;
                    y += dirY;
                }
                else break;
            }
        }
    }
}

