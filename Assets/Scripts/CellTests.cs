using NUnit.Framework;
using UnityEngine; 

[TestFixture]
public class CellTests
{
    [Test]
    public void Cell_WhenValueChanged_ShouldTriggerEvent()
    {
        var cell = new Cell(new Vector2Int(0, 0), 1);
        bool eventTriggered = false;
        cell.OnValueChanged += _ => eventTriggered = true;
        
        cell.CellView.number = 2;
        
    }

    [Test]
    public void Cell_WhenPositionChanged_ShouldTriggerEvent()
    {
        // Arrange
        var cell = new Cell(new Vector2Int(0, 0), 1);
        bool eventTriggered = false;
        cell.OnPositionChanged += _ => eventTriggered = true;

        // Act
        cell.coordinates = new Vector2Int(1, 1);
        
    }
}