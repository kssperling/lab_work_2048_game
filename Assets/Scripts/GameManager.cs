using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
public class GameManager : MonoBehaviour
{
    public GameField board;
    public CanvasGroup gameOver;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public int score;
    private string savePath;
    private int bestScore;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        hiscoreText.text = LoadHiscore().ToString();
        
        gameOver.alpha = 0f;
        gameOver.interactable = false;
        
        board.ClearBoard();
        board.CreateCell();
        board.CreateCell();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void InsreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        
        scoreText.text = score.ToString();

        SaveHiscore();
    }

    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();

        if (score > hiscore)
        {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }
    
    
    

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "game2048_save.dat");
        Debug.Log(savePath);
        LoadGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
    
    public void SaveGame()
    {
        var saveData = new GameSaveData
        {
            cells = board.Cells.Select(c => new CellSaveData
            {
                x = c.coordinates.x,
                y = c.coordinates.y,
                value = c.CellView.number
            }).ToList(),
            currentScore = score,
            bestScore = bestScore
        };
    
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Create))
        {
            formatter.Serialize(stream, saveData);
        }
    }
    
    private void UpdateScore()
    {
        int hiscore = LoadHiscore();
        score = board.Cells.Sum(cell => (int)Mathf.Pow(2, cell.CellView.number));
        if (score > hiscore)
        {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }
    
    public void LoadGame()
    {
        if (!File.Exists(savePath)) return;
    
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Open))
        {
            GameSaveData saveData = (GameSaveData)formatter.Deserialize(stream);
            
            // Обновляем лучший счет
            bestScore = saveData.bestScore;
            
            // // Восстанавливаем игровое поле
            // board.ClearBoard();
            // foreach (var cellData in saveData.cells)
            // {
            //     board.CreateCell(
            //         new Vector2Int(cellData.x, cellData.y),
            //         cellData.value);
            // }
            
            // Восстанавливаем текущий счет
            score = saveData.currentScore;
            UpdateScore();
        }
    }
    
    public void SetTestMode(GameField testField)
    {
        board = testField;
    }

    public void SetTestSavePath(string path)
    {
        savePath = path;
    }
    
    
}
