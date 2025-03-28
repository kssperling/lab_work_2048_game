using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class SaveLoadTests
{
    private GameManager _gameController;
    private string _testSavePath;

    [SetUp]
    public void Setup()
    {
        _gameController = new GameObject().AddComponent<GameManager>();
        _testSavePath = Path.Combine(Application.persistentDataPath, "test_save.dat");
        _gameController.SetTestSavePath(_testSavePath);
    }

    [Test]
    public void SaveGame_ShouldCreateSaveFile()
    {

        _gameController.SaveGame();
        
    }

    [Test]
    public void LoadGame_ShouldRestoreGameState()
    {
        _gameController.SaveGame();
        int initialScore = _gameController.score;
        
        _gameController.LoadGame();
        
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_testSavePath))
            File.Delete(_testSavePath);
        Object.DestroyImmediate(_gameController.gameObject);
    }
}