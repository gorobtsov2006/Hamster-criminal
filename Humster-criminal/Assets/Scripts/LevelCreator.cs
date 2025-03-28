using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementTypes
{
    Empty = 0,
    Hamster,
    Wall,
    Box,
    Goal,
    Rat,

    IsWord = 99,
    HamsterWord = 100,
    WallWord,
    GoalWord,
    BoxWord,
    RatWord,

    YouWord = 200,
    PushWord,
    WinWord,
    StopWord,
}

[CreateAssetMenu(menuName = "Level Creator")]
[System.Serializable]
public class LevelCreator : ScriptableObject
{
    [SerializeField]
    public int gridWidth = 5;

    [SerializeField]
    public int gridHeight = 5;

    [SerializeField]
    public List<ElementTypes> level = new List<ElementTypes>();

    public void GenerateLevel()
    {
        int newSize = gridWidth * gridHeight;
        if (level.Count != newSize)
        {
            level.Clear();
            for (int i = 0; i < newSize; i++)
            {
                level.Add(ElementTypes.Empty);
            }
        }
    }

    private void OnValidate()
    {
        int newSize = gridWidth * gridHeight;
        if (level.Count != newSize)
        {
            GenerateLevel();
        }
    }
}
