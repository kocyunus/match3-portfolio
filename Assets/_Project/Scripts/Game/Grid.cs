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
