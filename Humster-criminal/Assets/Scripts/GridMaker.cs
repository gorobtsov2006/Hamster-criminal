using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridMaker : MonoBehaviour
{
    int rows, cols;
    public GameObject cellHolder;
    public List<LevelCreator> levelHolder = new List<LevelCreator>();
    public List<GameObject> cells = new List<GameObject>();
    public List<SpriteLibrary> spriteLibrary = new List<SpriteLibrary>();
    public static GridMaker instance = null;
    public GameObject boundary;
    int currentLevel = 0;
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
        if (!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
        }
        currentLevel = PlayerPrefs.GetInt("Level");

        float count = levelHolder[currentLevel].level.Count;
        rows = (int)Mathf.Sqrt(count);
        cols = rows;
        CreateGrid();
        CompileRules();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void CreateGrid()
    {
        for (int gI = -1; gI <= rows; gI += 1)
        {
            for (int gJ = -1; gJ <= rows; gJ += 1)
            {
                if (gI == -1 || gJ == -1 || gI == rows || gJ == rows)
                    Instantiate(boundary, new Vector3(gI, gJ, 0), Quaternion.identity);
            }
        }


        int counter = 0;
        for (int i = 0; i < levelHolder[currentLevel].level.Count; i++)
        {
            if (levelHolder[currentLevel].level[i] != ElementTypes.Empty)
            {
                GameObject g = Instantiate(cellHolder, new Vector3(counter % cols, counter / rows, 0), Quaternion.identity);
                cells.Add(g);
                ElementTypes currentElement = levelHolder[currentLevel].level[i];

                g.GetComponent<CellProperty>().AssignInfo(counter / rows, counter % cols, currentElement);



            }
            counter++;
        }
    }

    public Sprite ReturnSpriteOf(ElementTypes e)
    {
        return spriteLibrary.Find(x => x.element == e).sprite;
    }

    public Vector2 Return2D(int i)
    {
        return new Vector2(i % cols, i / rows);
    }



    public bool IsStop(int r, int c, Vector2 dir)
    {
        return true;
    }



    public void CompileRules()
    {

    }

    public ElementTypes GetActualObjectFromWord(ElementTypes e)
    {
        if (e == ElementTypes.HamsterWord)
        {
            return ElementTypes.Hamster;
        }
        else if (e == ElementTypes.GoalWord)
        {
            return ElementTypes.Goal;
        }
        else if (e == ElementTypes.BoxWord)
        {
            return ElementTypes.Box;
        }
        else if (e == ElementTypes.WallWord)
        {
            return ElementTypes.Wall;
        }
        return ElementTypes.Empty;

    }

    public void Rule(ElementTypes a, ElementTypes b)
    {

    }
    public void ResetData()
    {

    }


    public CellProperty GetCellOf(ElementTypes e)
    {
        foreach (GameObject g in cells)
        {
            if (g != null && g.GetComponent<CellProperty>().Element == e)
            {
                return g.GetComponent<CellProperty>();
            }
        }
        return null;
    }

    public List<CellProperty> GetAllCellsOf(ElementTypes e)
    {
        List<CellProperty> cellProp = new List<CellProperty>();

        foreach (GameObject g in cells)
        {
            if (g != null && g.GetComponent<CellProperty>().Element == e)
            {
                cellProp.Add(g.GetComponent<CellProperty>());
            }
        }
        return cellProp;

    }

    public bool IsElementStartingWord(ElementTypes e)//
    {
        return true;
    }

    public List<GameObject> FindObjectsAt(int r, int c)
    {
        return cells.FindAll(x => x != null && x.GetComponent<CellProperty>().CurrentRow == r && x.GetComponent<CellProperty>().CurrentCol == c);
    }

    public ElementTypes ReturnWordAt(int r, int c)
    {
        List<GameObject> l = FindObjectsAt(r, c);

        foreach (GameObject g in l)
        {
            ElementTypes e = g.GetComponent<CellProperty>().Element;
            if ((int)e >= 100)
            {
                return e;
            }

        }
        return ElementTypes.Empty;
    }
    public bool DoesListContainElement(List<GameObject> l, ElementTypes e) //
    {
        return true;
    }

    public bool DoesListContainWord(List<GameObject> l) //
    {
       
        return true;
    }



    public bool IsElementIsWord(ElementTypes e)
    {
        if ((int)e == 99)
        {
            return true;
        }
        return false;
    }

    public void NextLevel()
    {
        if (PlayerPrefs.GetInt("Level") >= levelHolder.Count)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    [System.Serializable]
    public class SpriteLibrary
    {
        public ElementTypes element;
        public Sprite sprite;
    }
}

