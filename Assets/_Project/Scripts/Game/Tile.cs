using System;

namespace Yunus.Match3
{

/// <summary>
/// Tile data modeli (Pure C#, MonoBehaviour değil)
/// Pozisyon ve tip bilgisi tutar, görsel işlemi yapmaz
/// </summary>
public class Tile
{
    #region Properties

    public int X { get; private set; }
    public int Y { get; private set; }
    public TileType Type { get; private set; }
    public bool IsMatchable { get; set; } = true;
    public bool IsMovable { get; set; } = true;

    #endregion

    #region Constructor
    public Tile(int x, int y, TileType type)
    {
        X = x;
        Y = y;
        Type = type;
    }

    #endregion

    #region Public Methods

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetType(TileType type)
    {
        Type = type;
    }

    /// <summary>
    /// Komşu mu? (Yatay veya dikey, çapraz değil)
    /// </summary>
    public bool IsNeighbor(Tile other)
    {
        if (other == null) return false;

        int deltaX = Math.Abs(X - other.X);
        int deltaY = Math.Abs(Y - other.Y);

        return (deltaX == 1 && deltaY == 0) || (deltaX == 0 && deltaY == 1);
    }

    /// <summary>
    /// Eşleşebilir mi? (Aynı tip ve her ikisi de matchable)
    /// </summary>
    public bool CanMatchWith(Tile other)
    {
        if (other == null) return false;
        if (!IsMatchable || !other.IsMatchable) return false;
        return Type == other.Type;
    }

    /// <summary>
    /// Manhattan mesafesi (|X1-X2| + |Y1-Y2|)
    /// </summary>
    public int GetManhattanDistance(Tile other)
    {
        if (other == null) return int.MaxValue;
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
    }

    #endregion

    #region Override Methods

    public override string ToString() => $"Tile({X},{Y}) - {Type}";

    public override bool Equals(object obj)
    {
        if (obj is Tile other)
            return X == other.X && Y == other.Y;
        return false;
    }

    public override int GetHashCode() => (X * 397) ^ Y;

    #endregion
}

/// <summary>
/// Tile tipleri
/// </summary>
public enum TileType
{
    Red, Blue, Green, Yellow, Purple, Orange
}

} // namespace Yunus.Match3

