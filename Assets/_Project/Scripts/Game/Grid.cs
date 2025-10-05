using System.Collections.Generic;

namespace Yunus.Match3
{

/// <summary>
/// Grid (ızgara) yapısı - Tile'ların pozisyonlarını yönetir
/// Pure C# class (Unity'ye bağımlı değil, test edilebilir)
/// </summary>
public class Grid
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int TotalCells => Width * Height;

    private Tile[,] tiles;

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        tiles = new Tile[width, height];
    }

    #region Tile Erişimi

    public Tile GetTile(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return null;
        return tiles[x, y];
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if (!IsValidPosition(x, y))
        {
            UnityEngine.Debug.LogWarning($"[Grid] Geçersiz pozisyon: ({x}, {y})");
            return;
        }

        tiles[x, y] = tile;
        
        if (tile != null)
        {
            tile.SetPosition(x, y);
        }
    }

    public Tile GetTileReference(Tile targetTile)
    {
        if (targetTile == null) return null;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (tiles[x, y] == targetTile)
                    return tiles[x, y];
            }
        }

        return null;
    }

    #endregion

    #region Swap İşlemleri

    public void SwapTiles(Tile tile1, Tile tile2)
    {
        if (tile1 == null || tile2 == null)
        {
            UnityEngine.Debug.LogWarning("[Grid] Swap için her iki tile da gerekli!");
            return;
        }

        int x1 = tile1.X;
        int y1 = tile1.Y;
        int x2 = tile2.X;
        int y2 = tile2.Y;

        if (!IsValidPosition(x1, y1) || !IsValidPosition(x2, y2))
        {
            UnityEngine.Debug.LogWarning("[Grid] Swap için geçersiz pozisyonlar!");
            return;
        }

        tiles[x1, y1] = tile2;
        tiles[x2, y2] = tile1;

        tile1.SetPosition(x2, y2);
        tile2.SetPosition(x1, y1);
    }

    #endregion

    #region Komşuluk İşlemleri

    /// <summary>
    /// Pozisyonun tüm komşularını getirir (üst, alt, sol, sağ)
    /// </summary>
    public List<Tile> GetNeighbors(int x, int y)
    {
        List<Tile> neighbors = new List<Tile>(4);

        AddNeighborIfValid(neighbors, x, y + 1); // Üst
        AddNeighborIfValid(neighbors, x, y - 1); // Alt
        AddNeighborIfValid(neighbors, x - 1, y); // Sol
        AddNeighborIfValid(neighbors, x + 1, y); // Sağ

        return neighbors;
    }

    private void AddNeighborIfValid(List<Tile> neighbors, int x, int y)
    {
        Tile neighbor = GetTile(x, y);
        if (neighbor != null)
        {
            neighbors.Add(neighbor);
        }
    }

    #endregion

    #region Grid Durumu

    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsEmpty(int x, int y)
    {
        return IsValidPosition(x, y) && tiles[x, y] == null;
    }

    public bool IsFull()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (tiles[x, y] == null)
                    return false;
            }
        }
        return true;
    }

    public List<(int x, int y)> GetEmptyPositions()
    {
        List<(int x, int y)> emptyPositions = new List<(int x, int y)>();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (tiles[x, y] == null)
                {
                    emptyPositions.Add((x, y));
                }
            }
        }

        return emptyPositions;
    }

    /// <summary>
    /// Grid'den tile'ı kaldır (null yap)
    /// Match sonrası tile'ları yok etmek için kullanılır
    /// </summary>
    public void RemoveTile(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            tiles[x, y] = null;
        }
    }

    #endregion

    #region Gravity System

    /// <summary>
    /// Gravity uygula - boş hücrelere tile'lar düşsün
    /// Column-based algorithm (mobile-optimized)
    /// </summary>
    /// <returns>Hareket eden tile'ların listesi (animation için)</returns>
    public List<(Tile tile, int oldY, int newY)> ApplyGravity()
    {
        List<(Tile, int, int)> moves = new List<(Tile, int, int)>();
        
        UnityEngine.Debug.Log($"[ApplyGravity] Starting... Grid: {Width}x{Height}");
        
        // Her column'ı ayrı işle
        for (int x = 0; x < Width; x++)
        {
            ApplyGravityToColumn(x, moves);
        }
        
        UnityEngine.Debug.Log($"[ApplyGravity] Complete! Total moves: {moves.Count}");
        
        return moves;
    }

    /// <summary>
    /// Tek column'a gravity uygula (internal method)
    /// ALGORITHM: Top-to-Bottom Scan (Y büyük=ÜST, Y küçük=ALT)
    /// </summary>
    private void ApplyGravityToColumn(int x, List<(Tile, int, int)> moves)
    {
        UnityEngine.Debug.Log($"[Gravity-X={x}] ═══ START COLUMN ═══");
        
        // YUKARIDAN AŞAĞIYA TAR! (Y=Height-1 üstten Y=0 alta)
        for (int y = Height - 1; y >= 0; y--)
        {
            // Bu pozisyon boşsa atla
            if (tiles[x, y] == null)
            {
                UnityEngine.Debug.Log($"[Gravity-X={x}] Y={y} → Empty (skip)");
                continue;
            }
            
            Tile currentTile = tiles[x, y];
            UnityEngine.Debug.Log($"[Gravity-X={x}] Y={y} → Checking tile: {currentTile.Type}");
            
            // AŞAĞIDA KAÇ TANE BOŞ VAR? BUL! (Y-1, Y-2, Y-3...)
            int fallDistance = 0;
            for (int checkY = y - 1; checkY >= 0; checkY--)
            {
                if (tiles[x, checkY] == null)
                {
                    fallDistance++;
                    UnityEngine.Debug.Log($"[Gravity-X={x}]   ↓ Y={checkY} empty → fallDistance={fallDistance}");
                }
                else
                {
                    UnityEngine.Debug.Log($"[Gravity-X={x}]   ✋ Y={checkY} blocked by {tiles[x, checkY].Type}");
                    break;  // Dolu tile bulduk, dur!
                }
            }
            
            // DÜŞÜRECEK YER VARSA DÜŞ!
            if (fallDistance > 0)
            {
                int newY = y - fallDistance;  // ← Y AZALIR! (Alta düşme)
                
                UnityEngine.Debug.Log($"[Gravity-X={x}] ✅ FALLING: {currentTile.Type} ({x},{y}) → ({x},{newY})");
                
                // Eski yerden çıkar
                tiles[x, y] = null;
                
                // Yeni yere yerleştir
                tiles[x, newY] = currentTile;
                currentTile.SetPosition(x, newY);
                
                // Move kaydı ekle (animation için)
                moves.Add((currentTile, y, newY));
            }
            else
            {
                UnityEngine.Debug.Log($"[Gravity-X={x}] ⏸️ NO FALL: {currentTile.Type} stays at Y={y}");
            }
        }
        
        UnityEngine.Debug.Log($"[Gravity-X={x}] ═══ END COLUMN ═══");
    }

    #endregion

    #region Temizleme

    public void Clear()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x, y] = null;
            }
        }
    }

    #endregion

    #region Debug

    public override string ToString()
    {
        string result = $"Grid {Width}x{Height}:\n";

        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile = tiles[x, y];
                if (tile == null)
                {
                    result += "_ ";
                }
                else
                {
                    result += tile.Type.ToString()[0] + " ";
                }
            }
            result += "\n";
        }

        return result;
    }

    #endregion
}

} // namespace Yunus.Match3
