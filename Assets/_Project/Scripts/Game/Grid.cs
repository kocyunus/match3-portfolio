using System.Collections.Generic;

namespace Yunus.Match3
{

/// <summary>
/// Oyun tahtasının grid (ızgara) yapısını yöneten sınıf.
/// 
/// NE İŞE YARAR:
/// 8x8 (veya özelleştirilebilir boyutta) bir ızgara oluşturur ve
/// tile'ların pozisyonlarını takip eder. Tile erişimi, ekleme,
/// çıkarma gibi temel grid işlemlerini yönetir.
/// 
/// NEDEN AYRI BİR CLASS:
/// - Single Responsibility → Grid sadece tile yönetimi yapar
/// - Testability → Unity'siz test edilebilir
/// - Reusability → Farklı board boyutları için kullanılabilir
/// - Maintainability → Grid mantığı tek yerde
/// 
/// UNITY'DEKİ KARŞILIĞI:
/// Normalde BoardManager MonoBehaviour içinde 2D array tutardık.
/// Ama bu yöntemde Grid ayrı bir class, daha temiz ve test edilebilir.
/// 
/// SEKTÖR STANDARDI:
/// Büyük match-3 oyunlarında (Candy Crush, Toon Blast) grid mantığı
/// her zaman ayrı bir data structure olarak tutulur.
/// </summary>
public class Grid
{
    #region Properties

    /// <summary>
    /// Grid'in genişliği (sütun sayısı).
    /// Standart match-3 oyunlarında genellikle 8'dir.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Grid'in yüksekliği (satır sayısı).
    /// Standart match-3 oyunlarında genellikle 8'dir.
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Toplam hücre sayısı (Width x Height).
    /// Performans optimizasyonları için kullanılabilir.
    /// </summary>
    public int TotalCells => Width * Height;

    #endregion

    #region Fields

    // 2D array olarak tile'ları tutan ana veri yapısı
    // tiles[x, y] formatında erişim sağlar
    // Örnek: tiles[0, 0] = sol alt köşe
    //        tiles[7, 7] = sağ üst köşe (8x8 grid'de)
    private Tile[,] tiles;

    #endregion

    #region Constructor

    /// <summary>
    /// Yeni bir Grid oluşturur.
    /// 
    /// PARAMETRE AÇIKLAMALARI:
    /// width: Grid genişliği (sütun sayısı)
    /// height: Grid yüksekliği (satır sayısı)
    /// 
    /// KULLANIM:
    /// var grid = new Grid(8, 8); // Standart 8x8 grid
    /// var grid = new Grid(6, 10); // Özel boyut grid
    /// 
    /// NOT:
    /// Constructor sadece array'i oluşturur, tile'ları OLUŞTURMAZ!
    /// Tile'ları sonradan SetTile() ile eklemelisiniz.
    /// </summary>
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        
        // 2D array oluştur (başlangıçta tüm hücreler null)
        tiles = new Tile[width, height];
    }

    #endregion

    #region Public Methods - Tile Erişimi

    /// <summary>
    /// Belirtilen pozisyondaki tile'ı getirir.
    /// 
    /// PARAMETRE AÇIKLAMALARI:
    /// x: Sütun index'i (0'dan başlar)
    /// y: Satır index'i (0'dan başlar)
    /// 
    /// DÖNÜŞ DEĞERİ:
    /// - Geçerli pozisyonda tile varsa: Tile nesnesi
    /// - Pozisyon geçersizse: null
    /// - Pozisyon boşsa: null
    /// 
    /// KULLANIM:
    /// Tile tile = grid.GetTile(3, 5);
    /// if (tile != null)
    /// {
    ///     Debug.Log($"Tile tipi: {tile.Type}");
    /// }
    /// 
    /// PERFORMANS:
    /// O(1) - Sabit zaman, çok hızlı!
    /// Array index erişimi kullanır.
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        // Pozisyon kontrolü (bounds checking)
        if (!IsValidPosition(x, y))
            return null;

        return tiles[x, y];
    }

    /// <summary>
    /// Belirtilen pozisyona tile yerleştirir.
    /// 
    /// PARAMETRE AÇIKLAMALARI:
    /// x: Sütun index'i
    /// y: Satır index'i
    /// tile: Yerleştirilecek tile (null olabilir, boşluk için)
    /// 
    /// KULLANIM:
    /// var tile = new Tile(3, 5, TileType.Red);
    /// grid.SetTile(3, 5, tile);
    /// 
    /// // Boşluk oluşturmak için:
    /// grid.SetTile(3, 5, null);
    /// 
    /// NOT:
    /// Eğer pozisyon geçersizse işlem yapılmaz.
    /// Tile'ın kendi pozisyonu ile grid pozisyonu farklı olabilir
    /// (örneğin animasyon sırasında).
    /// </summary>
    public void SetTile(int x, int y, Tile tile)
    {
        // Pozisyon kontrolü
        if (!IsValidPosition(x, y))
        {
            UnityEngine.Debug.LogWarning($"[Grid] Geçersiz pozisyon: ({x}, {y})");
            return;
        }

        tiles[x, y] = tile;
        
        // Eğer tile null değilse, tile'ın pozisyonunu güncelle
        if (tile != null)
        {
            tile.SetPosition(x, y);
        }
    }

    /// <summary>
    /// Bir tile'ın referansını bulup getirir.
    /// 
    /// KULLANIM:
    /// Tile myTile = ...;
    /// Tile found = grid.GetTileReference(myTile);
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Tile'ın grid'deki güncel pozisyonunu bulmak için
    /// - Match detection sırasında
    /// 
    /// NOT:
    /// Bu metod tüm grid'i tarar, O(n²) kompleksitesi!
    /// Sık kullanılmamalı, sadece gerektiğinde.
    /// </summary>
    public Tile GetTileReference(Tile targetTile)
    {
        if (targetTile == null)
            return null;

        // Tüm grid'i tara
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

    #region Public Methods - Swap İşlemleri

    /// <summary>
    /// İki tile'ın pozisyonlarını değiştirir (swap).
    /// 
    /// PARAMETRE AÇIKLAMALARI:
    /// tile1: İlk tile
    /// tile2: İkinci tile
    /// 
    /// KULLANIM:
    /// Tile tile1 = grid.GetTile(2, 3);
    /// Tile tile2 = grid.GetTile(3, 3);
    /// grid.SwapTiles(tile1, tile2);
    /// 
    /// NE OLUR:
    /// 1. tile1, tile2'nin pozisyonuna gider
    /// 2. tile2, tile1'in pozisyonuna gider
    /// 3. Her iki tile'ın da pozisyon property'si güncellenir
    /// 
    /// NOT:
    /// Sadece grid verisini değiştirir, ANİMASYON YAPMAZ!
    /// Animasyon için TileView kullanılmalıdır.
    /// </summary>
    public void SwapTiles(Tile tile1, Tile tile2)
    {
        // Null kontrolü
        if (tile1 == null || tile2 == null)
        {
            UnityEngine.Debug.LogWarning("[Grid] Swap için her iki tile da gerekli!");
            return;
        }

        // Pozisyonları al
        int x1 = tile1.X;
        int y1 = tile1.Y;
        int x2 = tile2.X;
        int y2 = tile2.Y;

        // Pozisyon kontrolü
        if (!IsValidPosition(x1, y1) || !IsValidPosition(x2, y2))
        {
            UnityEngine.Debug.LogWarning("[Grid] Swap için geçersiz pozisyonlar!");
            return;
        }

        // Grid'deki referansları değiştir
        tiles[x1, y1] = tile2;
        tiles[x2, y2] = tile1;

        // Tile'ların kendi pozisyonlarını güncelle
        tile1.SetPosition(x2, y2);
        tile2.SetPosition(x1, y1);
    }

    #endregion

    #region Public Methods - Komşuluk İşlemleri

    /// <summary>
    /// Belirtilen pozisyonun tüm komşularını getirir.
    /// 
    /// PARAMETRE AÇIKLAMALARI:
    /// x: Sütun index'i
    /// y: Satır index'i
    /// 
    /// DÖNÜŞ DEĞERİ:
    /// Komşu tile'ların listesi (2-4 adet, köşelerde 2, kenarlarda 3, ortada 4)
    /// 
    /// KOMŞULUK SIRASI:
    /// 1. Üst (y+1)
    /// 2. Alt (y-1)
    /// 3. Sol (x-1)
    /// 4. Sağ (x+1)
    /// 
    /// KULLANIM:
    /// List<Tile> neighbors = grid.GetNeighbors(3, 5);
    /// foreach (var neighbor in neighbors)
    /// {
    ///     Debug.Log($"Komşu: {neighbor}");
    /// }
    /// 
    /// KULLANIM ALANLARI:
    /// - Match detection
    /// - Flood fill algoritması
    /// - Pathfinding
    /// - Bomba patlatma (etrafındaki tile'lar)
    /// </summary>
    public List<Tile> GetNeighbors(int x, int y)
    {
        List<Tile> neighbors = new List<Tile>(4); // Maksimum 4 komşu

        // Üst komşu
        AddNeighborIfValid(neighbors, x, y + 1);
        
        // Alt komşu
        AddNeighborIfValid(neighbors, x, y - 1);
        
        // Sol komşu
        AddNeighborIfValid(neighbors, x - 1, y);
        
        // Sağ komşu
        AddNeighborIfValid(neighbors, x + 1, y);

        return neighbors;
    }

    /// <summary>
    /// Yardımcı metod: Geçerli komşuları listeye ekler.
    /// </summary>
    private void AddNeighborIfValid(List<Tile> neighbors, int x, int y)
    {
        Tile neighbor = GetTile(x, y);
        if (neighbor != null)
        {
            neighbors.Add(neighbor);
        }
    }

    #endregion

    #region Public Methods - Grid Durumu

    /// <summary>
    /// Belirtilen pozisyon grid sınırları içinde mi kontrol eder.
    /// 
    /// KULLANIM:
    /// if (grid.IsValidPosition(10, 5))
    /// {
    ///     // 8x8 grid'de bu false döner (10 > 7)
    /// }
    /// 
    /// NEDEN ÖNEMLİ:
    /// Array bounds exception'ını önler!
    /// Her tile erişiminden önce mutlaka kontrol edilmelidir.
    /// </summary>
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    /// <summary>
    /// Belirtilen pozisyon boş mu kontrol eder.
    /// 
    /// KULLANIM:
    /// if (grid.IsEmpty(3, 5))
    /// {
    ///     // Bu pozisyona yeni tile yerleştirilebilir
    /// }
    /// 
    /// KULLANIM ALANLARI:
    /// - Gravity uygulanırken (boşlukları bul)
    /// - Tile spawn kontrolü
    /// - Match sonrası temizleme
    /// </summary>
    public bool IsEmpty(int x, int y)
    {
        return IsValidPosition(x, y) && tiles[x, y] == null;
    }

    /// <summary>
    /// Grid'in tamamının dolu olup olmadığını kontrol eder.
    /// 
    /// KULLANIM:
    /// if (grid.IsFull())
    /// {
    ///     // Grid tamamen dolu, oyun başlatılabilir
    /// }
    /// 
    /// KULLANIM ALANLARI:
    /// - Board initialization kontrolü
    /// - Game over kontrolü (hareket kalmadı mı?)
    /// </summary>
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

    /// <summary>
    /// Grid'deki tüm boş pozisyonların listesini döndürür.
    /// 
    /// KULLANIM:
    /// var emptyPositions = grid.GetEmptyPositions();
    /// foreach (var pos in emptyPositions)
    /// {
    ///     // Bu pozisyonlara yeni tile spawn edilebilir
    /// }
    /// 
    /// KULLANIM ALANLARI:
    /// - Gravity sonrası tile spawn
    /// - Shuffle işlemi
    /// - Debug/visualization
    /// </summary>
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

    #region Public Methods - Temizleme

    /// <summary>
    /// Tüm grid'i temizler (tüm tile referanslarını null yapar).
    /// 
    /// KULLANIM:
    /// grid.Clear();
    /// 
    /// NE ZAMAN KULLANILIR:
    /// - Oyun yeniden başlarken
    /// - Level değiştirken
    /// - Test senaryolarında
    /// 
    /// NOT:
    /// Bu metod sadece referansları temizler.
    /// TileView GameObject'lerini YOK ETMEZ!
    /// Object pool'a iade etmeyi unutmayın.
    /// </summary>
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

    #region Debug Methods

    /// <summary>
    /// Grid'in string formatında gösterimi (debug için).
    /// 
    /// KULLANIM:
    /// Debug.Log(grid.ToString());
    /// 
    /// OUTPUT ÖRNEĞİ:
    /// Grid 8x8:
    /// R B G Y P R B G
    /// B G Y P R B G Y
    /// ...
    /// 
    /// (R=Red, B=Blue, G=Green, Y=Yellow, P=Purple)
    /// </summary>
    public override string ToString()
    {
        string result = $"Grid {Width}x{Height}:\n";

        // Yukarıdan aşağıya yaz (Y'yi tersine)
        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile = tiles[x, y];
                if (tile == null)
                {
                    result += "_ "; // Boş hücre
                }
                else
                {
                    // Tile tipinin ilk harfi
                    result += tile.Type.ToString()[0] + " ";
                }
            }
            result += "\n";
        }

        return result;
    }

    #endregion
}

/*
 * ===== KULLANIM KILAVUZU =====
 * 
 * --- GRİD OLUŞTURMA ---
 * 
 * var grid = new Grid(8, 8);
 * 
 * --- TİLE EKLEME ---
 * 
 * var tile = new Tile(3, 5, TileType.Red);
 * grid.SetTile(3, 5, tile);
 * 
 * --- TİLE ERİŞİMİ ---
 * 
 * Tile tile = grid.GetTile(3, 5);
 * if (tile != null)
 * {
 *     Debug.Log(tile.Type);
 * }
 * 
 * --- SWAP İŞLEMİ ---
 * 
 * Tile tile1 = grid.GetTile(2, 3);
 * Tile tile2 = grid.GetTile(3, 3);
 * grid.SwapTiles(tile1, tile2);
 * 
 * --- KOMŞULARI BULMA ---
 * 
 * List<Tile> neighbors = grid.GetNeighbors(3, 5);
 * 
 * ===== PERFORMANS NOTLARI =====
 * 
 * - GetTile/SetTile: O(1) - Çok hızlı
 * - GetNeighbors: O(1) - Sadece 4 erişim
 * - IsFull/Clear: O(n²) - Tüm grid taranır
 * - GetTileReference: O(n²) - Sık kullanılmamalı
 * 
 * ===== SEKTÖR BİLGİSİ =====
 * 
 * Bu grid yapısı industry standard'dır:
 * - 2D array kullanımı (cache-friendly)
 * - Bounds checking (güvenli)
 * - Pure C# (Unity'siz test edilebilir)
 * - Single responsibility (sadece tile yönetimi)
 * 
 * Candy Crush, Toon Blast gibi oyunlar benzer yapılar kullanır.
 */

} // namespace Yunus.Match3

