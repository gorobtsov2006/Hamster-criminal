using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    int rows, cols;
    public GameObject cellHolder;
    public LevelCreator levelHolder;
    public List<GameObject> cells = new List<GameObject>();
    public List<SpriteLibrary> spriteLibrary = new List<SpriteLibrary>();
    public static GridMaker instance;
    public int Rows { get { return rows; } }
    public int Cols { get { return cols; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    private void Start()
    {
        float count = levelHolder.level.Count;
        rows = (int)Mathf.Sqrt(count);
        cols = rows;
        CreateGrid();
    }

    public void CreateGrid()
    {
        for (int i = 0; i < levelHolder.level.Count; i++)
        {
            if (levelHolder.level[i] != ElementTypes.Empty)
            {
                GameObject g = Instantiate(cellHolder, new Vector3(i % cols, i / rows, 0), Quaternion.identity);

                ElementTypes currentElement = levelHolder.level[i];
                Sprite s = spriteLibrary.Find(x => x.element == currentElement).sprite;

                g.GetComponent<SpriteRenderer>().sprite = s;
                //g.GetComponent<CellProperty>().AssignInfo(i % cols, i / rows, currentElement);
            }
        }
    }


    public void Rule(ElementTypes a, ElementTypes b)
    {

    }
}


[System.Serializable]
public class SpriteLibrary
{
    public ElementTypes element;
    public Sprite sprite;
 }

