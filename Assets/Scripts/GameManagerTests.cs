using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GameManagerTests
{
    private GameManager gameManager;
    private GameField _gameField;

    [SetUp]
    public void Setup()
    {
        _gameField = new GameObject().AddComponent<GameField>();
        gameManager = new GameObject().AddComponent<GameManager>();
        gameManager.SetTestMode(_gameField);
    }

    [Test]
    public void MoveCells_WhenValidMove_ShouldChangePositions()
    {
        _gameField.CreateCell(new Vector2Int(0, 0), 1);
        
    }

    [Test]
    public void CheckGameOver_WhenNoMoves_ShouldSetGameOver()
    {
        for (int i = 0; i < _gameField.FieldSize; i++)
        {
            for (int j = 0; j < _gameField.FieldSize; j++)
            {
                _gameField.CreateCell(new Vector2Int(i, j), (i + j) % 2 + 1);
            }
        }
        
        gameManager.GameOver();
        
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gameManager.gameObject);
        Object.DestroyImmediate(_gameField.gameObject);
    }
}