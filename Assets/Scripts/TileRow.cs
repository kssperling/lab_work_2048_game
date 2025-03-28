using UnityEngine;

public class TileRow : MonoBehaviour
{
    public Cell[] cells { get; private set; }

    private void Awake()
    {
        cells = GetComponentsInChildren<Cell>();
    }
}
