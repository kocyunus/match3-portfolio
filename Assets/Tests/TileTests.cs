using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Yunus.Match3;

public class TileTests
{
    // Constructor testi
    [Test]
    public void Constructor_ValidParameters_CreatesTileCorrectly()
    {
        // Arrange
        int expectedX = 5;
        int expectedY = 3;
        TileType expectedType = TileType.Red;

        // Act
        Tile tile = new Tile(expectedX, expectedY, expectedType);

        // Assert
        Assert.AreEqual(expectedX, tile.X);
        Assert.AreEqual(expectedY, tile.Y);
        Assert.AreEqual(expectedType, tile.Type);
    }

    // Yatay komşu testi
    [Test]
    public void IsNeighbor_HorizontalNeighbor_ReturnsTrue()
    {
        // Arrange
        Tile tile1 = new Tile(2, 3, TileType.Red);
        Tile tile2 = new Tile(3, 3, TileType.Blue);

        // Act
        bool result = tile1.IsNeighbor(tile2);

        // Assert
        Assert.IsTrue(result);
    }

    // Dikey komşu testi
    [Test]
    public void IsNeighbor_VerticalNeighbor_ReturnsTrue()
    {
        // Arrange
        Tile tile1 = new Tile(2, 3, TileType.Red);
        Tile tile2 = new Tile(2, 4, TileType.Blue);

        // Act
        bool result = tile1.IsNeighbor(tile2);

        // Assert
        Assert.IsTrue(result);
    }

    // Çapraz komşu testi (false olmalı)
    [Test]
    public void IsNeighbor_DiagonalTile_ReturnsFalse()
    {
        // Arrange
        Tile tile1 = new Tile(2, 3, TileType.Red);
        Tile tile2 = new Tile(3, 4, TileType.Blue);

        // Act
        bool result = tile1.IsNeighbor(tile2);

        // Assert
        Assert.IsFalse(result);
    }

    // Aynı tip eşleşme testi
    [Test]
    public void CanMatchWith_SameType_ReturnsTrue()
    {
        // Arrange
        Tile tile1 = new Tile(0, 0, TileType.Red);
        Tile tile2 = new Tile(1, 0, TileType.Red);

        // Act
        bool result = tile1.CanMatchWith(tile2);

        // Assert
        Assert.IsTrue(result);
    }

    // Farklı tip eşleşme testi
    [Test]
    public void CanMatchWith_DifferentType_ReturnsFalse()
    {
        // Arrange
        Tile tile1 = new Tile(0, 0, TileType.Red);
        Tile tile2 = new Tile(1, 0, TileType.Blue);

        // Act
        bool result = tile1.CanMatchWith(tile2);

        // Assert
        Assert.IsFalse(result);
    }
}
