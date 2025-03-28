using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public List<CellSaveData> cells;
    public int currentScore;
    public int bestScore;
}

