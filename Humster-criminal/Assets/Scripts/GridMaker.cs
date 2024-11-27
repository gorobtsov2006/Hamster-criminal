using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{
    public int rows , cols ;
    public GameObject cellHolder;
    List<List<GameObject>> cells = new List<List<GameObject>>();
    private void Start()
    {
        CreateGrid();
    }

    public void CreateGrid()
    {
        for(int r = 0; r < rows; r++)
        {
            cells.Add(new List<GameObject>());
            for(int c = 0; c < cols; c++)
            {
                GameObject gameObject = Instantiate(cellHolder, new Vector3(c, r), Quaternion.identity);
                cells[r].Add(gameObject);
            }
        }
    }
}
