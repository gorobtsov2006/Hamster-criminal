using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CellProperty : MonoBehaviour
{
    ElementTypes element;
    bool isPushable;
    bool destroysObject;
    bool isWin;
    bool isPlayer;
    bool isStop;
    bool isDangerous; // Добавляем свойство для крыс
    int currentRow, currentCol;

    public ElementTypes Element
    {
        get { return element; }
    }
    public bool IsStop
    {
        get { return isStop; }
    }
    public bool IsPushable
    {
        get { return isPushable; }
    }
    public int CurrentRow
    {
        get { return currentRow; }
    }
    public int CurrentCol
    {
        get { return currentCol; }
    }

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void AssignInfo(int r, int c, ElementTypes e)
    {
        currentRow = r;
        currentCol = c;
        element = e;
        ChangeSprite();
        if (e == ElementTypes.Wall)
        {
            isStop = true;
        }
        if (e == ElementTypes.Hamster)
        {
            isPlayer = true;
            spriteRenderer.sortingOrder = 100;
        }
        if (e == ElementTypes.Rat) // Крыса опасна
        {
            isDangerous = true;
        }
    }

    public void Initialize()
    {
        isPushable = false;
        destroysObject = false;
        isWin = false;
        isPlayer = false;
        isStop = false;
        isDangerous = false; // Сбрасываем опасность

        if ((int)element >= 99)
        {
            isPushable = true;
        }
        if (element == ElementTypes.Rat) // Крыса остается опасной
        {
            isDangerous = true;
        }
    }

    public void ChangeSprite()
    {
        Sprite s = GridMaker.instance.spriteLibrary.Find(x => x.element == element).sprite;
        spriteRenderer.sprite = s;

        if (isPlayer || isPushable)
        {
            spriteRenderer.sortingOrder = 100;
        }
        else
        {
            spriteRenderer.sortingOrder = 10;
        }
    }

    public void ChangeObject(CellProperty c)
    {
        element = c.element;
        isPushable = c.isPushable;
        destroysObject = c.destroysObject;
        isWin = c.isWin;
        isPlayer = c.isPlayer;
        isStop = c.IsStop;
        isDangerous = c.isDangerous; // Копируем свойство опасности
        ChangeSprite();
    }

    public void IsPlayer(bool isP)
    {
        isPlayer = isP;
    }

    public void IsItStop(bool isS)
    {
        isStop = isS;
    }

    public void IsItWin(bool isW)
    {
        isWin = isW;
    }
    public void IsItPushable(bool isPush)
    {
        isPushable = isPush;
    }
    public void IsItDestroy(bool isD)
    {
        destroysObject = isD;
    }
    public void IsItDangerous(bool isDang)
    {
        isDangerous = isDang;
    }

    void Update()
    {
        CheckDestroy();
        if (isPlayer)
        {
            CheckDanger(); // Проверяем опасность от крыс

            if (Input.GetKeyDown(KeyCode.RightArrow) && currentCol + 1 < GridMaker.instance.Cols && !GridMaker.instance.IsStop(currentRow, currentCol + 1, Vector2.right))
            {
                List<GameObject> movingObject = new List<GameObject>();
                movingObject.Add(this.gameObject);

                for (int c = currentCol + 1; c < GridMaker.instance.Cols - 1; c++)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x + 1, g.transform.position.y, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentCol++;
                }
                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentCol - 1 >= 0 && !GridMaker.instance.IsStop(currentRow, currentCol - 1, Vector2.left))
            {
                List<GameObject> movingObject = new List<GameObject>();
                movingObject.Add(this.gameObject);

                for (int c = currentCol - 1; c > 0; c--)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x - 1, g.transform.position.y, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentCol--;
                }
                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) && currentRow + 1 < GridMaker.instance.Rows && !GridMaker.instance.IsStop(currentRow + 1, currentCol, Vector2.up))
            {
                List<GameObject> movingObject = new List<GameObject>();
                movingObject.Add(this.gameObject);

                for (int r = currentRow + 1; r < GridMaker.instance.Rows - 1; r++)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y + 1, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentRow++;
                }
                GridMaker.instance.CompileRules();
                CheckWin();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentRow - 1 >= 0 && !GridMaker.instance.IsStop(currentRow - 1, currentCol, Vector2.down))
            {
                List<GameObject> movingObject = new List<GameObject>();
                movingObject.Add(this.gameObject);

                for (int r = currentRow - 1; r >= 0; r--)
                {
                    if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                    {
                        movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (GameObject g in movingObject)
                {
                    g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y - 1, g.transform.position.z);
                    g.GetComponent<CellProperty>().currentRow--;
                }
                GridMaker.instance.CompileRules();
                CheckWin();
            }
        }
    }

    public void CheckWin()
    {
        List<GameObject> objectsAtPlayerPosition = GridMaker.instance.FindObjectsAt(currentRow, currentCol);

        foreach (GameObject g in objectsAtPlayerPosition)
        {
            if (g.GetComponent<CellProperty>().isWin)
            {
                Debug.Log("Player Won!");
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
                GridMaker.instance.NextLevel();
            }
        }
    }

    public void CheckDestroy()
    {
        List<GameObject> objectsAtPosition = GridMaker.instance.FindObjectsAt(currentRow, currentCol);
        bool destroys = false;
        bool normalObject = false;
        foreach (GameObject g in objectsAtPosition)
        {
            if (!g.GetComponent<CellProperty>().destroysObject)
            {
                normalObject = true;
            }
            if (g.GetComponent<CellProperty>().destroysObject)
            {
                destroys = true;
            }
        }

        if (destroys && normalObject)
        {
            foreach (GameObject g in objectsAtPosition)
            {
                Destroy(g);
            }
        }
    }

    public void CheckDanger()
    {
        // Проверяем соседние клетки на наличие крыс
        List<Vector2> directions = new List<Vector2> { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (Vector2 dir in directions)
        {
            int checkRow = currentRow + (int)dir.y;
            int checkCol = currentCol + (int)dir.x;
            List<GameObject> objectsAtPosition = GridMaker.instance.FindObjectsAt(checkRow, checkCol);
            foreach (GameObject g in objectsAtPosition)
            {
                if (g.GetComponent<CellProperty>().isDangerous)
                {
                    Debug.Log("Player Died!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапуск уровня
                    return;
                }
            }
        }
    }
}