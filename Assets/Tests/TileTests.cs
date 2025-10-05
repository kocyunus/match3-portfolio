using NUnit.Framework;
using Yunus.Match3;

namespace Yunus.Match3.Tests
{
    /// <summary>
    /// Tile class için unit testler
    /// Test Coverage: Constructor, Neighbors, Matching
    /// </summary>
    public class TileTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesTile()
        {
            // Arrange & Act
            Tile tile = new Tile(5, 3, TileType.Red);
            
            // Assert
            Assert.AreEqual(5, tile.X);
            Assert.AreEqual(3, tile.Y);
            Assert.AreEqual(TileType.Red, tile.Type);
        }
        
        [Test]
        public void IsNeighbor_HorizontalAdjacent_ReturnsTrue()
        {
            // Arrange
            Tile tile1 = new Tile(2, 2, TileType.Red);
            Tile tile2 = new Tile(3, 2, TileType.Blue);
            
            // Act
            bool isNeighbor = tile1.IsNeighbor(tile2);
            
            // Assert
            Assert.IsTrue(isNeighbor);
        }
        
        [Test]
        public void IsNeighbor_VerticalAdjacent_ReturnsTrue()
        {
            // Arrange
            Tile tile1 = new Tile(2, 2, TileType.Red);
            Tile tile2 = new Tile(2, 3, TileType.Blue);
            
            // Act
            bool isNeighbor = tile1.IsNeighbor(tile2);
            
            // Assert
            Assert.IsTrue(isNeighbor);
        }
        
        [Test]
        public void IsNeighbor_DiagonalTiles_ReturnsFalse()
        {
            // Arrange
            Tile tile1 = new Tile(2, 2, TileType.Red);
            Tile tile2 = new Tile(3, 3, TileType.Blue);
            
            // Act
            bool isNeighbor = tile1.IsNeighbor(tile2);
            
            // Assert
            Assert.IsFalse(isNeighbor);
        }
        
        [Test]
        public void CanMatchWith_SameType_ReturnsTrue()
        {
            // Arrange
            Tile tile1 = new Tile(0, 0, TileType.Red);
            Tile tile2 = new Tile(1, 0, TileType.Red);
            
            // Act
            bool canMatch = tile1.CanMatchWith(tile2);
            
            // Assert
            Assert.IsTrue(canMatch);
        }
        
        [Test]
        public void CanMatchWith_DifferentType_ReturnsFalse()
        {
            // Arrange
            Tile tile1 = new Tile(0, 0, TileType.Red);
            Tile tile2 = new Tile(1, 0, TileType.Blue);
            
            // Act
            bool canMatch = tile1.CanMatchWith(tile2);
            
            // Assert
            Assert.IsFalse(canMatch);
        }
    }
}

