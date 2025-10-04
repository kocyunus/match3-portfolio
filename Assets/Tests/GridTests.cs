using NUnit.Framework;
using Yunus.Match3;

/// <summary>
/// Grid sınıfının unit testlerini içerir.
/// EditMode'da çalışır, oyun çalışmadan test edilir.
/// </summary>
public class GridTests
{
    private Grid grid;

    /// <summary>
    /// Her test öncesi çalışır, test ortamını hazırlar.
    /// Unity'de bu Setup pattern'i çok yaygındır.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // 8x8 boyutunda standart bir grid oluştur
        grid = new Grid(8, 8);
    }

    /// <summary>
    /// Grid'in doğru boyutlarda oluşturulduğunu test eder.
    /// </summary>
    [Test]
    public void Constructor_ValidSize_CreatesGridCorrectly()
    {
        // Arrange & Act (Setup'ta yapıldı)
        
        // Assert - Grid'in genişlik ve yüksekliği doğru mu?
        Assert.AreEqual(8, grid.Width, "Grid genişliği yanlış!");
        Assert.AreEqual(8, grid.Height, "Grid yüksekliği yanlış!");
    }

    /// <summary>
    /// Grid'e tile yerleştirme işlemini test eder.
    /// </summary>
    [Test]
    public void SetTile_ValidPosition_StoresTile()
    {
        // Arrange - Test için bir tile oluştur
        Tile tile = new Tile(0, 0, TileType.Red);
        
        // Act - Tile'ı grid'e yerleştir
        grid.SetTile(0, 0, tile);
        
        // Assert - Tile doğru yere yerleşti mi?
        Tile retrievedTile = grid.GetTile(0, 0);
        Assert.AreEqual(tile, retrievedTile, "Tile doğru pozisyona yerleşmedi!");
    }

    /// <summary>
    /// Grid'den tile alma işlemini test eder.
    /// </summary>
    [Test]
    public void GetTile_ValidPosition_ReturnsTile()
    {
        // Arrange
        Tile tile = new Tile(3, 4, TileType.Blue);
        grid.SetTile(3, 4, tile);
        
        // Act
        Tile result = grid.GetTile(3, 4);
        
        // Assert
        Assert.NotNull(result, "Tile null döndü!");
        Assert.AreEqual(TileType.Blue, result.Type, "Tile tipi yanlış!");
        Assert.AreEqual(3, result.X, "Tile X pozisyonu yanlış!");
        Assert.AreEqual(4, result.Y, "Tile Y pozisyonu yanlış!");
    }

    /// <summary>
    /// Geçersiz pozisyon için tile alma işlemini test eder.
    /// </summary>
    [Test]
    public void GetTile_InvalidPosition_ReturnsNull()
    {
        // Act - Grid dışında bir pozisyon iste
        Tile result = grid.GetTile(-1, -1);
        
        // Assert - Null dönmeli
        Assert.IsNull(result, "Geçersiz pozisyon için null dönmeliydi!");
    }

    /// <summary>
    /// Grid'de pozisyon geçerliliğini test eder.
    /// </summary>
    [Test]
    public void IsValidPosition_ValidCoordinates_ReturnsTrue()
    {
        // Act & Assert - Grid içindeki pozisyonlar geçerli olmalı
        Assert.IsTrue(grid.IsValidPosition(0, 0), "Sol üst köşe geçerli olmalı!");
        Assert.IsTrue(grid.IsValidPosition(7, 7), "Sağ alt köşe geçerli olmalı!");
        Assert.IsTrue(grid.IsValidPosition(4, 4), "Merkez geçerli olmalı!");
    }

    /// <summary>
    /// Grid'de geçersiz pozisyonları test eder.
    /// </summary>
    [Test]
    public void IsValidPosition_InvalidCoordinates_ReturnsFalse()
    {
        // Act & Assert - Grid dışındaki pozisyonlar geçersiz olmalı
        Assert.IsFalse(grid.IsValidPosition(-1, 0), "Negatif X geçersiz olmalı!");
        Assert.IsFalse(grid.IsValidPosition(0, -1), "Negatif Y geçersiz olmalı!");
        Assert.IsFalse(grid.IsValidPosition(8, 0), "X sınır dışı geçersiz olmalı!");
        Assert.IsFalse(grid.IsValidPosition(0, 8), "Y sınır dışı geçersiz olmalı!");
    }

    /// <summary>
    /// İki tile'ın yerlerini değiştirme işlemini test eder.
    /// Bu, Match-3 oyunlarında en kritik işlemlerden biridir!
    /// </summary>
    [Test]
    public void SwapTiles_ValidPositions_SwapsTiles()
    {
        // Arrange - İki farklı tile oluştur ve yerleştir
        Tile tile1 = new Tile(0, 0, TileType.Red);
        Tile tile2 = new Tile(0, 1, TileType.Blue);
        grid.SetTile(0, 0, tile1);
        grid.SetTile(0, 1, tile2);
        
        // Act - Tile'ları değiştir (önce grid'den al, sonra swap et)
        Tile retrievedTile1 = grid.GetTile(0, 0);
        Tile retrievedTile2 = grid.GetTile(0, 1);
        grid.SwapTiles(retrievedTile1, retrievedTile2);
        
        // Assert - Pozisyonlar doğru değişti mi?
        Tile resultAt00 = grid.GetTile(0, 0);
        Tile resultAt01 = grid.GetTile(0, 1);
        
        Assert.AreEqual(TileType.Blue, resultAt00.Type, "İlk pozisyonda Blue olmalıydı!");
        Assert.AreEqual(TileType.Red, resultAt01.Type, "İkinci pozisyonda Red olmalıydı!");
    }

    /// <summary>
    /// Komşu tile'ları bulma işlemini test eder.
    /// Match-3'te eşleşmeleri kontrol ederken kullanılır.
    /// </summary>
    [Test]
    public void GetNeighbors_CenterPosition_ReturnsFourNeighbors()
    {
        // Arrange - Merkez pozisyon için tile'lar oluştur
        grid.SetTile(4, 4, new Tile(4, 4, TileType.Red));
        grid.SetTile(3, 4, new Tile(3, 4, TileType.Blue));  // Sol
        grid.SetTile(5, 4, new Tile(5, 4, TileType.Green)); // Sağ
        grid.SetTile(4, 3, new Tile(4, 3, TileType.Yellow)); // Alt
        grid.SetTile(4, 5, new Tile(4, 5, TileType.Purple)); // Üst
        
        // Act - Merkez tile'ın komşularını al
        var neighbors = grid.GetNeighbors(4, 4);
        
        // Assert - 4 komşu olmalı (yukarı, aşağı, sağ, sol)
        Assert.AreEqual(4, neighbors.Count, "Merkez pozisyonda 4 komşu olmalı!");
    }

    /// <summary>
    /// Köşe pozisyonunda komşu tile'ları test eder.
    /// Köşe pozisyonlarında sadece 2 komşu olur.
    /// </summary>
    [Test]
    public void GetNeighbors_CornerPosition_ReturnsTwoNeighbors()
    {
        // Arrange - Sol üst köşe için tile'lar oluştur
        grid.SetTile(0, 0, new Tile(0, 0, TileType.Red));
        grid.SetTile(1, 0, new Tile(1, 0, TileType.Blue));  // Sağ
        grid.SetTile(0, 1, new Tile(0, 1, TileType.Green)); // Üst
        
        // Act
        var neighbors = grid.GetNeighbors(0, 0);
        
        // Assert - Köşede sadece 2 komşu olmalı
        Assert.AreEqual(2, neighbors.Count, "Köşe pozisyonda 2 komşu olmalı!");
    }

    /// <summary>
    /// Grid'i tamamen temizleme işlemini test eder.
    /// Oyun yeniden başlatıldığında kullanılır.
    /// </summary>
    [Test]
    public void Clear_FilledGrid_RemovesAllTiles()
    {
        // Arrange - Grid'i tile'larla doldur
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                grid.SetTile(x, y, new Tile(x, y, TileType.Red));
            }
        }
        
        // Act - Grid'i temizle
        grid.Clear();
        
        // Assert - Tüm pozisyonlar null olmalı
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Assert.IsNull(grid.GetTile(x, y), $"Pozisyon ({x},{y}) temizlenmedi!");
            }
        }
    }
}

