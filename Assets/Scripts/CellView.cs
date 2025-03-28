using Unity.VisualScripting;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    public TileState state { get; private set; }
    public Cell cell { get; private set; }
    public int number {get; set;}
    public bool locked {get; set;}

    private Image background;
    private TextMeshProUGUI text;
    
    private void Awake()
    {
        background = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;
        
        background.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = number.ToString();
    }

    public void Spawn(Cell cell)
    {
        if (this.cell != null)
        {
            this.cell.CellView = null;
        }
        
        this.cell = cell;
        this.cell.CellView = this;
        
        transform.position = cell.transform.position;
    }

    public void MoveTo(Cell cell)
    {
        if (this.cell != null)
        {
            this.cell.CellView = null;
        }
        
        this.cell = cell;
        this.cell.CellView = this;

        StartCoroutine(Animate(cell.transform.position, false));
        
    }

    public void Merge(Cell cell)
    {
        if (this.cell != null)
        {
            this.cell.CellView = null;
        }

        this.cell = null;
        cell.CellView.locked = true;
        
        StartCoroutine(Animate(cell.transform.position, true));
        
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;
        
        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = to;

        if (merging)
        {
            Destroy(gameObject);
        }
    }
    
    public void Init(Cell cell)
    {
        this.cell = cell;
        UpdateValue();
        UpdatePosition();

        cell.OnValueChanged += OnCellValueChanged;
        cell.OnPositionChanged += OnCellPositionChanged;
    }

    private void OnCellValueChanged(Cell cell)
    {
        UpdateValue();
    }

    private void OnCellPositionChanged(Cell cell)
    {
        UpdatePosition();
    }

    private void UpdateValue()
    {
        // Вычисляем значение как 2 в степени cell.Value
        int displayValue = (int)Mathf.Pow(2, cell.CellView.number);
        text.text = displayValue.ToString();

        // Можно добавить изменение цвета в зависимости от значения
        // Пример:
        float hue = Mathf.Clamp01(cell.CellView.number / 15f);
        background.color = Color.HSVToRGB(hue, 0.7f, 1f);
    }

    private void UpdatePosition()
    {
        // Преобразуем координаты клетки в мировые координаты
        // Предполагаем, что поле начинается в (0,0) и каждая клетка имеет размер 1 юнит
        transform.localPosition = new Vector3(cell.coordinates.x, cell.coordinates.y, 0);
    }
    
    private void OnDestroy()
    {
        if (cell != null)
        {
            cell.OnValueChanged -= OnCellValueChanged;
            cell.OnPositionChanged -= OnCellPositionChanged;
        }
    }

}
