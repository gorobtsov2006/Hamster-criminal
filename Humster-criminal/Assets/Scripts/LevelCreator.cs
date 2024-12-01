using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementTypes
{
    Empty,
    Hamster,
    Wall, 
    Box, 
    Goal,
    Goop,

    IsWord = 99,
    HamsterWord = 100,
    WallWord,
    GoalWord,
    BoxWord,
    GoopWord,


    YouWord = 200,
    PushWord,
    WinWord,
    StopWord,
    SinkWord,
}



[CreateAssetMenu()][System.Serializable]
public class LevelCreator : ScriptableObject
{

    [SerializeField]
    public List<ElementTypes> level = new List<ElementTypes>();

    public LevelCreator()
    {
        level = new List<ElementTypes>();
    }
}