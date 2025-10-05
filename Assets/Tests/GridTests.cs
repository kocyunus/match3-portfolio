using NUnit.Framework;
using Yunus.Match3;

namespace Yunus.Match3.Tests
{
    /// <summary>
    /// Grid class için unit testler
    /// Test Coverage: Initialization, CRUD operations, Validation, Swapping
    /// </summary>
    public class GridTests
    {
        private Grid grid;
        
        [SetUp]
        public void Setup()
        {
            grid = new Grid(8, 8);
        }
        
        [TearDown]
        public void Teardown()
        {
            grid = null;
        }
        
        [Test]
        public void Constructor_ValidDimensions_CreatesGrid()
        {
            // Arrange & Act
            Grid testGrid = new Grid(5, 7);
            
            // Assert
            Assert.AreEqual(5, testGrid.Width);
            Assert.AreEqual(7, testGrid.Height);
        }
        
        [Test]
        public void SetTile_ValidPosition_SetsTile()
        {
            // Arrange
            Tile tile = new Tile(2, 3, TileType.Red);
            
            // Act
            grid.SetTile(2, 3, tile);
            Tile retrieved = grid.GetTile(2, 3);
            
            // Assert
            Assert.AreEqual(tile, retrieved);
        }
        
        [Test]
        public void GetTile_InvalidPosition_ReturnsNull()
        {
            // Act
            Tile tile = grid.GetTile(-1, 5);
            
            // Assert
            Assert.IsNull(tile);
        }
        
        [Test]
        public void GetTile_OutOfBounds_ReturnsNull()
        {
            // Act
            Tile tile = grid.GetTile(100, 100);
            
            // Assert
            Assert.IsNull(tile);
        }
        
        [Test]
        public void IsValidPosition_ValidCoordinates_ReturnsTrue()
        {
            // Act
            bool isValid = grid.IsValidPosition(3, 4);
            
            // Assert
            Assert.IsTrue(isValid);
        }
        
        [Test]
        public void IsValidPosition_InvalidCoordinates_ReturnsFalse()
        {
            // Act
            bool isValid = grid.IsValidPosition(-1, 10);
            
            // Assert
            Assert.IsFalse(isValid);
        }
        
        [Test]
        public void SwapTiles_ValidTiles_SwapsTiles()
        {
            // Arrange
            Tile tile1 = new Tile(0, 0, TileType.Red);
            Tile tile2 = new Tile(1, 0, TileType.Blue);
            grid.SetTile(0, 0, tile1);
            grid.SetTile(1, 0, tile2);
            
            // Act
            grid.SwapTiles(tile1, tile2);
            
            // Assert
            Assert.AreEqual(tile1, grid.GetTile(1, 0));
            Assert.AreEqual(tile2, grid.GetTile(0, 0));
            Assert.AreEqual(1, tile1.X);
            Assert.AreEqual(0, tile1.Y);
            Assert.AreEqual(0, tile2.X);
            Assert.AreEqual(0, tile2.Y);
        }
        
        [Test]
        public void GetNeighbors_CenterTile_ReturnsFourNeighbors()
        {
            // Arrange
            Tile centerTile = new Tile(4, 4, TileType.Red);
            grid.SetTile(4, 4, centerTile);
            
            Tile top = new Tile(4, 5, TileType.Blue);
            Tile bottom = new Tile(4, 3, TileType.Blue);
            Tile left = new Tile(3, 4, TileType.Blue);
            Tile right = new Tile(5, 4, TileType.Blue);
            
            grid.SetTile(4, 5, top);
            grid.SetTile(4, 3, bottom);
            grid.SetTile(3, 4, left);
            grid.SetTile(5, 4, right);
            
            // Act
            var neighbors = grid.GetNeighbors(centerTile.X, centerTile.Y);
            
            // Assert
            Assert.AreEqual(4, neighbors.Count);
        }
        
        [Test]
        public void GetNeighbors_CornerTile_ReturnsTwoNeighbors()
        {
            // Arrange
            Tile cornerTile = new Tile(0, 0, TileType.Red);
            grid.SetTile(0, 0, cornerTile);
            
            Tile right = new Tile(1, 0, TileType.Blue);
            Tile top = new Tile(0, 1, TileType.Blue);
            
            grid.SetTile(1, 0, right);
            grid.SetTile(0, 1, top);
            
            // Act
            var neighbors = grid.GetNeighbors(cornerTile.X, cornerTile.Y);
            
            // Assert
            Assert.AreEqual(2, neighbors.Count);
        }
        
        [Test]
        public void Clear_FilledGrid_ClearsAllTiles()
        {
            // Arrange
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    grid.SetTile(x, y, new Tile(x, y, TileType.Red));
                }
            }
            
            // Act
            grid.Clear();
            
            // Assert
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Assert.IsNull(grid.GetTile(x, y));
                }
            }
        }
    }
}

