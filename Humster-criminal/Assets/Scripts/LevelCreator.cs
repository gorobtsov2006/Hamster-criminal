using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementTypes
{
    Empty,
    Hamster,
    Wall, 
    Rock, 
    Flag,
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