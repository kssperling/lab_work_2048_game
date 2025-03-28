using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int coordinates { get; set; } // позиция
    public CellView CellView { get; set; } // здесь текущее значение, а также остальные показатели

    public bool empty => CellView == null;
    public bool occupied => CellView != null;
    
    public event Action<Cell> OnValueChanged;
    public event Action<Cell> OnPositionChanged;
    
    public Cell(Vector2Int position, int value)
    {
        coordinates = position;
        CellView.number = value;
    }
    
}
