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
    bool isDangerous;
    int currentRow, currentCol;
    Vector2 lastDirection = Vector2.right;

    // Для движения при зажатии клавиши
    private float moveDelay = 0.2f; // Задержка между шагами (в секундах)
    private float lastMoveTime; // Время последнего шага

    // Для отмены действия
    private struct MoveState
    {
        public List<(GameObject obj, int row, int col)> positions; // Позиции всех объектов перед движением
    }
    private Stack<MoveState> moveHistory = new Stack<MoveState>(); // История движений

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
            isStop = true;
        if (e == ElementTypes.Hamster)
        {
            isPlayer = true;
            spriteRenderer.sortingOrder = 100;
        }
        if (e == ElementTypes.Rat)
            isDangerous = true;
    }

    public void Initialize()
    {
        isPushable = false;
        destroysObject = false;
        isWin = false;
        isPlayer = false;
        isStop = false;
        isDangerous = false;

        if ((int)element >= 99)
            isPushable = true;
        if (element == ElementTypes.Rat)
            isDangerous = true;
    }

    public void ChangeSprite()
    {
        if (element == ElementTypes.Hamster)
            spriteRenderer.sprite = GridMaker.instance.ReturnSpriteOf(element, lastDirection);
        else
            spriteRenderer.sprite = GridMaker.instance.ReturnSpriteOf(element, Vector2.zero);

        if (isPlayer || isPushable)
            spriteRenderer.sortingOrder = 100;
        else
            spriteRenderer.sortingOrder = 10;
    }

    public void ChangeObject(CellProperty c)
    {
        element = c.element;
        isPushable = c.isPushable;
        destroysObject = c.destroysObject;
        isWin = c.isWin;
        isPlayer = c.isPlayer;
        isStop = c.IsStop;
        isDangerous = c.isDangerous;
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
            CheckDanger();

            // Движение при зажатии клавиши
            if (Time.time - lastMoveTime >= moveDelay)
            {
                // Сохраняем текущее состояние перед движением
                SaveState();

                if (Input.GetKey(KeyCode.RightArrow) && currentCol + 1 < GridMaker.instance.Cols && !GridMaker.instance.IsStop(currentRow, currentCol + 1, Vector2.right))
                {
                    lastDirection = Vector2.right;
                    ChangeSprite();
                    List<GameObject> movingObject = new List<GameObject>();
                    movingObject.Add(this.gameObject);
                    for (int c = currentCol + 1; c < GridMaker.instance.Cols - 1; c++)
                    {
                        if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                            movingObject.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                        else
                            break;
                    }
                    foreach (GameObject g in movingObject)
                    {
                        g.transform.position = new Vector3(g.transform.position.x + 1, g.transform.position.y, g.transform.position.z);
                        g.GetComponent<CellProperty>().currentCol++;
                    }
                    GridMaker.instance.CompileRules();
                    CheckWin();
                    lastMoveTime = Time.time;
                }
                else if (Input.GetKey(KeyCode.LeftArrow) && currentCol - 1 >= 0 && !GridMaker.instance.IsStop(currentRow, currentCol - 1, Vector2.left))
                {
                    lastDirection = Vector2.left;
                    ChangeSprite();
                    List<GameObject> movingObject = new List<GameObject>();
                    movingObject.Add(this.gameObject);
                    for (int c = currentCol - 1; c > 0; c--)
                    {
                        if (GridMaker.instance.IsTherePushableObjectAt(currentRow, c))
                            movingObject.Add(GridMaker.instance.GetPushableObjectAt(currentRow, c));
                        else
                            break;
                    }
                    foreach (GameObject g in movingObject)
                    {
                        g.transform.position = new Vector3(g.transform.position.x - 1, g.transform.position.y, g.transform.position.z);
                        g.GetComponent<CellProperty>().currentCol--;
                    }
                    GridMaker.instance.CompileRules();
                    CheckWin();
                    lastMoveTime = Time.time;
                }
                else if (Input.GetKey(KeyCode.UpArrow) && currentRow + 1 < GridMaker.instance.Rows && !GridMaker.instance.IsStop(currentRow + 1, currentCol, Vector2.up))
                {
                    lastDirection = Vector2.up;
                    ChangeSprite();
                    List<GameObject> movingObject = new List<GameObject>();
                    movingObject.Add(this.gameObject);
                    for (int r = currentRow + 1; r < GridMaker.instance.Rows - 1; r++)
                    {
                        if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                            movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                        else
                            break;
                    }
                    foreach (GameObject g in movingObject)
                    {
                        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y + 1, g.transform.position.z);
                        g.GetComponent<CellProperty>().currentRow++;
                    }
                    GridMaker.instance.CompileRules();
                    CheckWin();
                    lastMoveTime = Time.time;
                }
                else if (Input.GetKey(KeyCode.DownArrow) && currentRow - 1 >= 0 && !GridMaker.instance.IsStop(currentRow - 1, currentCol, Vector2.down))
                {
                    lastDirection = Vector2.down;
                    ChangeSprite();
                    List<GameObject> movingObject = new List<GameObject>();
                    movingObject.Add(this.gameObject);
                    for (int r = currentRow - 1; r >= 0; r--)
                    {
                        if (GridMaker.instance.IsTherePushableObjectAt(r, currentCol))
                            movingObject.Add(GridMaker.instance.GetPushableObjectAt(r, currentCol));
                        else
                            break;
                    }
                    foreach (GameObject g in movingObject)
                    {
                        g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y - 1, g.transform.position.z);
                        g.GetComponent<CellProperty>().currentRow--;
                    }
                    GridMaker.instance.CompileRules();
                    CheckWin();
                    lastMoveTime = Time.time;
                }
            }
        }
    }

    // Сохранение состояния перед движением
    private void SaveState()
    {
        MoveState state = new MoveState();
        state.positions = new List<(GameObject, int, int)>();

        // Сохраняем позицию игрока
        state.positions.Add((this.gameObject, currentRow, currentCol));

        // Сохраняем позиции всех толкаемых объектов
        foreach (GameObject g in GridMaker.instance.cells)
        {
            if (g != null && g.GetComponent<CellProperty>().IsPushable)
            {
                CellProperty cp = g.GetComponent<CellProperty>();
                state.positions.Add((g, cp.CurrentRow, cp.CurrentCol));
            }
        }

        moveHistory.Push(state);
    }

    // Отмена последнего движения
    private void UndoMove()
    {
        MoveState lastState = moveHistory.Pop();

        foreach (var (obj, row, col) in lastState.positions)
        {
            CellProperty cp = obj.GetComponent<CellProperty>();
            cp.currentRow = row;
            cp.currentCol = col;
            obj.transform.position = new Vector3(col, row, obj.transform.position.z);
        }

        // Восстанавливаем направление хомяка
        if (lastDirection == Vector2.right) lastDirection = Vector2.left;
        else if (lastDirection == Vector2.left) lastDirection = Vector2.right;
        else if (lastDirection == Vector2.up) lastDirection = Vector2.down;
        else if (lastDirection == Vector2.down) lastDirection = Vector2.up;
        ChangeSprite();

        GridMaker.instance.CompileRules();
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
                normalObject = true;
            if (g.GetComponent<CellProperty>().destroysObject)
                destroys = true;
        }

        if (destroys && normalObject)
        {
            foreach (GameObject g in objectsAtPosition)
                Destroy(g);
        }
    }

    public void CheckDanger()
    {
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
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    return;
                }
            }
        }
    }
}