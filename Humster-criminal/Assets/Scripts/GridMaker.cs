using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    int rows , cols ;
    public GameObject cellHolder;
    public LevelCreator currentLevel;
    List<List<GameObject>> cells = new List<List<GameObject>>();
    public List<SpriteLibrary> spriteLibrary = new List<SpriteLibrary>();

    private void Start()
    {
        rows = currentLevel.level.Count / 2;
        cols = currentLevel.level.Count / 2;
        CreateGrid();
    }

    public void CreateGrid()
    {
        for(int r = 0; r < rows; r++)
        {
            cells.Add(new List<GameObject>());
            for(int c = 0; c < cols; c++)
            {
                GameObject g = Instantiate(cellHolder, new Vector3(c, r, 0), Quaternion.identity);
                cells[r].Add(g);
              // spriteLibrary.Find(x->x.element == currentLevel.level[r][c]); нужно исправить ошибку
            }
        }
    }
}

public class SpriteLibrary
{
    public ElementTypes element;
    public Sprite sprite;
}
