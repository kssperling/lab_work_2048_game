using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GameFieldTests
{
    private GameField _gameField;

    [SetUp]
    public void Setup()
    {
        _gameField = new GameObject().AddComponent<GameField>();
        _gameField.CreateCell();
    }

    [Test]
    public void GetEmptyPosition_WhenFieldNotFull_ShouldReturnPosition()
    {
        var position = _gameField.GetEmptyPosition();
        
    }

    [Test]
    public void CreateCell_ShouldAddCellToField()
    {

        int initialCount = _gameField.Cells.Count;
        
        _gameField.CreateCell();
        
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameField.gameObject);
    }
}