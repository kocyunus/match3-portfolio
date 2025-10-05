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
            if (tile == null) return new List<Tile>();
            
            // DAHA SAĞLAM: HashSet kullan (duplicate önleme garantisi!)
            HashSet<Tile> matchSet = new HashSet<Tile>();
            
            // Yatay match'leri bul
            List<Tile> horizontalMatches = FindMatchesInLine(tile, grid, 1, 0, -1, 0);
            if (horizontalMatches.Count >= 3)
            {
                UnityEngine.Debug.Log($"[LineMatch] Horizontal match at ({tile.X},{tile.Y}): {horizontalMatches.Count} tiles");
                foreach (var t in horizontalMatches)
                {
                    matchSet.Add(t); // HashSet otomatik duplicate önler
                }
            }
            
            // Dikey match'leri bul
            List<Tile> verticalMatches = FindMatchesInLine(tile, grid, 0, 1, 0, -1);
            if (verticalMatches.Count >= 3)
            {
                UnityEngine.Debug.Log($"[LineMatch] Vertical match at ({tile.X},{tile.Y}): {verticalMatches.Count} tiles");
                foreach (var t in verticalMatches)
                {
                    matchSet.Add(t); // HashSet otomatik duplicate önler
                }
            }
            
            UnityEngine.Debug.Log($"[LineMatch] Total unique matches: {matchSet.Count}");
            
            return new List<Tile>(matchSet);
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
            int beforeCount = matches.Count;
            AddMatchesInDirection(matches, tile, grid, dir1X, dir1Y);
            UnityEngine.Debug.Log($"[FindMatchesInLine] Dir1({dir1X},{dir1Y}) added {matches.Count - beforeCount} tiles");
            
            // Yön 2 (örn: sol)
            beforeCount = matches.Count;
            AddMatchesInDirection(matches, tile, grid, dir2X, dir2Y);
            UnityEngine.Debug.Log($"[FindMatchesInLine] Dir2({dir2X},{dir2Y}) added {matches.Count - beforeCount} tiles");
            
            UnityEngine.Debug.Log($"[FindMatchesInLine] Total line matches: {matches.Count}");
            
            return matches;
        }
        
        /// <summary>
        /// Belirli yöndeki tüm eşleşmeleri listeye ekle
        /// BEST PRACTICE: Contains kontrolü ile duplicate önleme!
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
                    UnityEngine.Debug.Log($"[AddMatches] Found match at ({x},{y}), Type: {neighbor.Type}");
                    
                    // BEST PRACTICE: Duplicate kontrolü!
                    if (!matches.Contains(neighbor))
                    {
                        matches.Add(neighbor);
                        UnityEngine.Debug.Log($"[AddMatches] Added to list (now {matches.Count} tiles)");
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning($"[AddMatches] DUPLICATE! Tile at ({x},{y}) already in list!");
                    }
                    
                    x += dirX;
                    y += dirY;
                }
                else 
                {
                    if (neighbor == null)
                        UnityEngine.Debug.Log($"[AddMatches] Stopped at ({x},{y}): NULL tile");
                    else
                        UnityEngine.Debug.Log($"[AddMatches] Stopped at ({x},{y}): Different type ({neighbor.Type} vs {tile.Type})");
                    break;
                }
            }
        }
    }
}

