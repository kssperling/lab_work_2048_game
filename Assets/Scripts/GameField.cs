using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;


public class GameField : MonoBehaviour
{
    public GameManager gameManager;
    [FormerlySerializedAs("tilePrefab")] public CellView cellViewPrefab;
    public TileState[] tileStates;
    private TileGrid grid;
    private List<CellView> tiles;
    
    [SerializeField] private int _fieldSize = 4;
    [SerializeField] private CellView _cellPrefab;
    [SerializeField] private Transform _cellsContainer;

    private List<Cell> _cells = new List<Cell>();

    public int FieldSize => _fieldSize;
    public IReadOnlyList<Cell> Cells => _cells;

    private bool waiting;
    
    private Vector2 swipeStartPos;
    private bool isSwiping;
    private const float swipeThreshold = 50f;
    
    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<CellView>(16);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.CellView = null;
        }
        
        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        
        tiles.Clear();
    }
    
    public Vector2Int? GetEmptyPosition()
    {
        var allPositions = new List<Vector2Int>();
        
        // Генерируем все возможные позиции на поле
        for (int x = 0; x < _fieldSize; x++)
        {
            for (int y = 0; y < _fieldSize; y++)
            {
                allPositions.Add(new Vector2Int(x, y));
            }
        }

        // Исключаем занятые позиции
        var occupiedPositions = _cells.Select(c => c.coordinates).ToList();
        var emptyPositions = allPositions.Except(occupiedPositions).ToList();

        if (emptyPositions.Count == 0)
            return null;

        // Возвращаем случайную пустую позицию
        return emptyPositions[Random.Range(0, emptyPositions.Count)];
    }
    
    public void CreateCell()
    {
        var emptyPosition = GetEmptyPosition();
        // 90% - значение 1, 10% - значение 2
        int value = Random.Range(0, 100) < 90 ? 1 : 2;
        CellView cellView = Instantiate(cellViewPrefab, grid.transform);
        cellView.SetState(tileStates[value - 1], value);
        cellView.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(cellView);
    }
    
    public void CreateCell(Vector2Int? specificPosition = null, int? specificValue = null)
    {
        Vector2Int position = specificPosition ?? GetEmptyPosition() ?? Vector2Int.zero;
        int value = specificValue ?? (Random.Range(0, 100) < 20 ? 2 : 1);
        
        CellView cellView = Instantiate(cellViewPrefab, grid.transform);
        cellView.SetState(tileStates[value - 1], value);
        Cell newCell = grid.GetCell(position);
        newCell.CellView.number = value;
        cellView.Spawn(newCell);
        tiles.Add(cellView);
    }

    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                Debug.Log("Input: up");
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
                Debug.Log("Input: down");
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
                Debug.Log("Input: left");
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
                Debug.Log("Input: right");
            }
        }
    }
    
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        Touch.onFingerDown += OnFingerDown;
        Touch.onFingerMove += OnFingerMove;
        Touch.onFingerUp += OnFingerUp;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= OnFingerDown;
        Touch.onFingerMove -= OnFingerMove;
        Touch.onFingerUp -= OnFingerUp;
        EnhancedTouchSupport.Disable();
    }
    
    private void OnFingerDown(Finger finger)
    {
        swipeStartPos = finger.screenPosition;
        isSwiping = true;
    }
    
    private void OnFingerMove(Finger finger)
    {
        if (!isSwiping) return;
        
        Vector2 currentPos = finger.screenPosition;
        Vector2 delta = currentPos - swipeStartPos;

        if (delta.magnitude > swipeThreshold)
        {
            isSwiping = false;
            
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                {
                    Debug.Log("Swipe: right");
                    MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
                }
                else
                {
                    Debug.Log("Swipe: left");
                    MoveTiles(Vector2Int.left, 1, 1, 0, 1);
                }
            }
            else
            {
                if (delta.y > 0)
                {
                    Debug.Log("Swipe: up");
                    MoveTiles(Vector2Int.down, 0, 1, grid.height - 2, -1);
                }
                else
                {
                    Debug.Log("Swipe: down");
                    MoveTiles(Vector2Int.up, 0, 1, 1, 1);
                }
            }
        }
    }

    private void OnFingerUp(Finger finger) => isSwiping = false;
    

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                Cell cell = grid.GetCell(x, y);

                if (cell.occupied)
                {
                    changed = MoveTile(cell.CellView, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(CellView cellView, Vector2Int direction)
    {
        Cell newCell = null;
        Cell adjacent = grid.GetAdjacentCell(cellView.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(cellView, adjacent.CellView))
                {
                    Merge(cellView, adjacent.CellView);
                    return true;
                }
                break;
            }
            
            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            cellView.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(CellView a, CellView b)
    {
        return a.number == b.number && !b.locked;
    }
    
    private void Merge(CellView a, CellView b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;
        
        b.SetState(tileStates[index], number);
        
        gameManager.InsreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;
        
        yield return new WaitForSeconds(0.1f);
        
        waiting = false;

        foreach (var tile in tiles)
        {
            tile.locked = false;
        }

        if (tiles.Count != grid.size)
        {
            CreateCell();
        }

        if (CheckForGameOver())
        {
            gameManager.GameOver();
        }
        
    }

    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            Cell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            Cell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            Cell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            Cell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.CellView))
            {
                return false;
            }
            
            if (down != null && CanMerge(tile, down.CellView))
            {
                return false;
            }
            
            if (left != null && CanMerge(tile, left.CellView))
            {
                return false;
            }
            
            if (right != null && CanMerge(tile, right.CellView))
            {
                return false;
            }
        }

        return true;
    }
    
}
